using AlvTime.Business.Holidays;
using AlvTime.Business.Interfaces;
using AlvTime.Business.Models;
using AlvTime.Business.Options;
using AlvTime.Business.TimeEntries;
using AlvTime.Business.TimeRegistration;
using AlvTime.Business.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AlvTime.Business.InvoiceRate.InvoiceStatisticsDto;

namespace AlvTime.Business.InvoiceRate;

public class InvoiceRateService
{
    private readonly ITimeRegistrationStorage _timeRegistrationStorage;
    private readonly IRedDaysService _redDaysService;
    private readonly TimeEntryOptions _timeEntryOptions;
    private readonly IUserContext _userContext;

    public InvoiceRateService(ITimeRegistrationStorage timeRegistrationStorage,
        IRedDaysService redDaysService,
        IOptionsMonitor<TimeEntryOptions> optionsMonitor,
        IUserContext userContext
    )
    {
        _timeRegistrationStorage = timeRegistrationStorage;
        _redDaysService = redDaysService;
        _timeEntryOptions = optionsMonitor.CurrentValue;
        _userContext = userContext;
    }

    public async Task<decimal> GetEmployeeInvoiceRateForPeriod(DateTime fromDate, DateTime toDate)
    {
        var user = await _userContext.GetCurrentUser();
        var adjustedFromDate = GetUserStartDateOrFromDate(fromDate, user);
        var userTasks = await _timeRegistrationStorage.GetTimeEntriesWithCustomer(user.Id, adjustedFromDate, toDate);
        var availableHoursWithoutVacation = GetUserAvailableHours(adjustedFromDate, toDate);

        var taskDictionary = userTasks.GroupBy(GetTaskType)
                                      .ToDictionary(taskGroup => taskGroup.Key);

        decimal billableHours = taskDictionary.ContainsKey(TaskType.BILLABLE) ? taskDictionary[TaskType.BILLABLE].Sum(task => task.Value) : 0;
        decimal vacationHours = taskDictionary.ContainsKey(TaskType.VACATION) ? taskDictionary[TaskType.VACATION].Sum(task => task.Value) : 0;

        var availableHours = availableHoursWithoutVacation - vacationHours;

        if (availableHours <= 0)
        {
            return billableHours / 1;
        }

        return billableHours / availableHours;
    }

    public async Task<IEnumerable<InvoiceStatisticsDto>> GetEmployeeInvoiceStatisticsByPeriod(DateTime fromDate, DateTime toDate, InvoicePeriods invoicePeriod, ExtendPeriod extendPeriod)
    {
        var user = await _userContext.GetCurrentUser();
        var invoicePeriodStart = GetInvoicePeriodStart(fromDate, invoicePeriod, extendPeriod);
        var invoicePeriodEnd = GetInvoicePeriodEnd(toDate, invoicePeriod, extendPeriod);

        var userTasks = await _timeRegistrationStorage.GetTimeEntriesWithCustomer(user.Id, invoicePeriodStart, invoicePeriodEnd);
        var taskPeriodGrouping = GroupTasksByInvoicePeriod(userTasks, invoicePeriod);

        return taskPeriodGrouping.Select(grouping =>
        {
            var billableHours = grouping.Where(timeEntry => GetTaskType(timeEntry) == TaskType.BILLABLE).Sum(timeEntry => timeEntry.Value);
            var nonBillableHours = grouping.Where(timeEntry => GetTaskType(timeEntry) == TaskType.NON_BILLABLE).Sum(timeEntry => timeEntry.Value);
            var vacationHours = grouping.Where(timeEntry => GetTaskType(timeEntry) == TaskType.VACATION).Sum(timeEntry => timeEntry.Value);
            return new InvoiceStatisticsDto
            {
                Start = grouping.Key.periodStart,
                End = grouping.Key.periodEnd,
                BillableHours = billableHours,
                InvoiceRate = GetInvoiceRateForPeriod(billableHours, vacationHours, grouping.Key.periodStart, grouping.Key.periodEnd),
                NonBillableHours = nonBillableHours,
                NonBillableInvoiceRate = GetInvoiceRateForPeriod(nonBillableHours, vacationHours, grouping.Key.periodStart, grouping.Key.periodEnd),
                VacationHours = vacationHours
            };
        });
    }

    private IEnumerable<IGrouping<(DateTime periodStart, DateTime periodEnd), TimeEntryWithCustomerDto>> GroupTasksByInvoicePeriod(List<TimeEntryWithCustomerDto> userTasks, InvoicePeriods invoicePeriod)
    {
        return invoicePeriod switch
        {
            InvoicePeriods.Daily => userTasks.GroupBy(x => (x.Date.Date, new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, 23, 59, 59))),
            InvoicePeriods.Weekly => userTasks.GroupBy(x => (GetStartOfWeek(x.Date).Date, GetEndOfWeek(x.Date))),
            InvoicePeriods.Monthly => userTasks.GroupBy(x => (new DateTime(x.Date.Year, x.Date.Month, 1), new DateTime(x.Date.Year, x.Date.Month, DateTime.DaysInMonth(x.Date.Year, x.Date.Month), 23, 59, 59))),
            InvoicePeriods.Annualy => userTasks.GroupBy(x => (new DateTime(x.Date.Year, 1, 1), new DateTime(x.Date.Year, 12, 31, 23, 59, 59))),
            _ => throw new NotImplementedException()
        };
    }

    private DateTime GetInvoicePeriodStart(DateTime fromDate, InvoicePeriods invoicePeriod, ExtendPeriod extendperiod)
    {
        if (!extendperiod.HasFlag(ExtendPeriod.Start))
            return new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);

        return invoicePeriod switch
        {
            InvoicePeriods.Daily => new DateTime(fromDate.Year, fromDate.Month, fromDate.Day),
            InvoicePeriods.Weekly => GetStartOfWeek(fromDate).Date,
            InvoicePeriods.Monthly => new DateTime(fromDate.Year, fromDate.Month, 1),
            InvoicePeriods.Annualy => new DateTime(fromDate.Year, 1, 1),
            _ => throw new NotImplementedException()
        };
    }


    private DateTime GetInvoicePeriodEnd(DateTime toDate, InvoicePeriods invoicePeriod, ExtendPeriod extendperiod)
    {
        if (!extendperiod.HasFlag(ExtendPeriod.End))
            return new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

        return invoicePeriod switch
        {
            InvoicePeriods.Daily => new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59),
            InvoicePeriods.Weekly => GetEndOfWeek(toDate),
            InvoicePeriods.Monthly => new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month), 23, 59, 59),
            InvoicePeriods.Annualy => new DateTime(toDate.Year, 12, 31, 23, 59, 59),
            _ => throw new NotImplementedException()
        };
    }

    private static DateTime GetStartOfWeek(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Sunday ? date.AddDays(-6) : date.AddDays(1 - (int)date.DayOfWeek).Date;
    }

    private static DateTime GetEndOfWeek(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Sunday ? date.AddSeconds(86399) : date.Date.AddDays(7 - (int)date.DayOfWeek).Date.AddSeconds(86399);
    }

    private decimal GetInvoiceRateForPeriod(decimal billableHours, decimal vacationHours, DateTime fromDate, DateTime toDate)
    {
        var availableHours = GetUserAvailableHours(fromDate, toDate) - vacationHours;
        return billableHours / (availableHours > 0 ? availableHours : 1);
    }

    private TaskType GetTaskType(TimeEntryWithCustomerDto entry)
    {
        if (entry.TaskId == _timeEntryOptions.PaidHolidayTask || entry.TaskId == _timeEntryOptions.UnpaidHolidayTask)
            return TaskType.VACATION;
        if (entry.CustomerName.ToLower() == "alv")
            return TaskType.NON_BILLABLE;
        return TaskType.BILLABLE;
    }

    private decimal GetUserAvailableHours(DateTime fromDate, DateTime toDate)
    {
        decimal redDayHours = GetNonWorkingDays(fromDate, toDate).Count() * 7.5m;
        var workingHours = 7.5m + (toDate - fromDate).Days * 7.5m;
        return workingHours - redDayHours;
    }


    private IEnumerable<DateTime> GetNonWorkingDays(DateTime fromDate, DateTime toDate)
    {
        var redDays = _redDaysService.GetRedDaysFromYears(fromDate.Year, toDate.Year)
            .Select(dateString => DateTime.Parse(dateString))
            .Where(date => date >= fromDate && date <= toDate);
        var weekendDays = DateUtils.GetWeekendDays(fromDate, toDate).Where(day => !redDays.Any(redDay => redDay == day));
        return weekendDays.Concat(redDays);
    }

    private DateTime GetUserStartDateOrFromDate(DateTime fromDate, User user)
    {
        if (fromDate < user.StartDate)
            return user.StartDate;
        return fromDate;
    }
}
