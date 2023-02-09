using System;
using Xunit;
using AlvTime.Business.Options;
using Moq;
using Microsoft.Extensions.Options;
using AlvTime.Business.Holidays;
using AlvTime.Business.InvoiceRate;
using System.Collections.Generic;
using AlvTime.Business.TimeEntries;
using AlvTime.Business.TimeRegistration;
using AlvTime.Business.Interfaces;
using System.Linq;
using static AlvTime.Business.InvoiceRate.InvoiceStatisticsDto;
using AlvTime.Persistence.Repositories;
using AlvTime.Persistence.DatabaseModels;

namespace Tests.UnitTests.InvoiceRate;



public class InvoiceRateServiceTest
{

    private readonly IOptionsMonitor<TimeEntryOptions> _options;
    private readonly IRedDaysService _redDaysService;
    private readonly Mock<IUserContext> _userContextMock;
    public InvoiceRateServiceTest()
    {
        var entryOptions = new TimeEntryOptions
        {
            PaidHolidayTask = 10,
        };

        _options = Mock.Of<IOptionsMonitor<TimeEntryOptions>>(options => options.CurrentValue == entryOptions);

        _redDaysService = new RedDaysService();

        _userContextMock = new Mock<IUserContext>();

        var user = new AlvTime.Business.Models.User
        {
            Id = 1,
            Email = "someone@alv.no",
            Name = "Someone"
        };
        _userContextMock.Setup(context => context.GetCurrentUser()).Returns(System.Threading.Tasks.Task.FromResult(user));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetInvoicePercentage_Without_Vacation()
    {
        var hours = new List<Hours>
        {
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 01, 03),
                DayNumber = (short)new DateTime(2022, 01, 03).DayOfYear,
                Id = 1,
                Locked = false,
                TaskId = 3,
                Value = 1.5m,
                Year = (short)new DateTime(2022, 01, 03).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 01, 03),
                DayNumber = (short)new DateTime(2022, 01, 03).DayOfYear,
                Id = 2,
                Locked = false,
                TaskId = 1,
                Value = 6m,
                Year = (short)new DateTime(2022, 01, 03).Year
            }
        };

        var service = CreateInvoiceRateService(hours);

        decimal invoiceRate = await service.GetEmployeeInvoiceRateForPeriod(new DateTime(2022, 01, 03), new DateTime(2022, 01, 03));

        Assert.Equal(0.8m, invoiceRate);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetInvoicePercentage_DuringEaster_WithVacation()
    {
        // One vacation-day on wednesday in easter and works half days monday and tuesday with billable
        var hours = new List<Hours>
        {
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 04, 11),
                DayNumber = (short)new DateTime(2022, 04, 11).DayOfYear,
                Id = 1,
                Locked = false,
                TaskId = 3,
                Value = 3m,
                Year = (short)new DateTime(2022, 04, 11).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 04, 11),
                DayNumber = (short)new DateTime(2022, 04, 11).DayOfYear,
                Id = 2,
                Locked = false,
                TaskId = 1,
                Value = 4.5m,
                Year = (short)new DateTime(2022, 04, 11).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 04, 12),
                DayNumber = (short)new DateTime(2022, 04, 12).DayOfYear,
                Id = 3,
                Locked = false,
                TaskId = 3,
                Value = 3m,
                Year = (short)new DateTime(2022, 04, 12).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 04, 12),
                DayNumber = (short)new DateTime(2022, 04, 12).DayOfYear,
                Id = 4,
                Locked = false,
                TaskId = 1,
                Value = 4.5m,
                Year = (short)new DateTime(2022, 04, 12).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 04, 13),
                DayNumber = (short)new DateTime(2022, 04, 13).DayOfYear,
                Id = 5,
                Locked = false,
                TaskId = 10,
                Value = 7.5m,
                Year = (short)new DateTime(2022, 04, 13).Year
            }
        };

        var service = CreateInvoiceRateService(hours);

        decimal invoiceRate = await service.GetEmployeeInvoiceRateForPeriod(new DateTime(2022, 4, 11), new DateTime(2022, 4, 17));

        Assert.Equal(0.6m, invoiceRate);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetInvoicePercentage_With_Weekend()
    {
        var hours = new List<Hours>
        {
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 12, 12),
                DayNumber = (short)new DateTime(2022, 12, 12).DayOfYear,
                Id = 1,
                Locked = false,
                TaskId = 1,
                Value = 7.5m,
                Year = (short)new DateTime(2022, 12, 12).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 12, 13),
                DayNumber = (short)new DateTime(2022, 12, 13).DayOfYear,
                Id = 2,
                Locked = false,
                TaskId = 1,
                Value = 7.5m,
                Year = (short)new DateTime(2022, 12, 13).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 12, 14),
                DayNumber = (short)new DateTime(2022, 12, 14).DayOfYear,
                Id = 3,
                Locked = false,
                TaskId = 1,
                Value = 7.5m,
                Year = (short)new DateTime(2022, 12, 14).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 04, 15),
                DayNumber = (short)new DateTime(2022, 04, 15).DayOfYear,
                Id = 4,
                Locked = false,
                TaskId = 1,
                Value = 7.5m,
                Year = (short)new DateTime(2022, 04, 15).Year
            },
            new Hours
            {
                User = 1,
                Date = new DateTime(2022, 12, 16),
                DayNumber = (short)new DateTime(2022, 12, 16).DayOfYear,
                Id = 5,
                Locked = false,
                TaskId = 1,
                Value = 7.5m,
                Year = (short)new DateTime(2022, 12, 16).Year
            } 
        };

        var service = CreateInvoiceRateService(hours);

        decimal invoiceRate = await service.GetEmployeeInvoiceRateForPeriod(new DateTime(2022, 12, 12), new DateTime(2022, 12, 18));

        Assert.Equal(1m, invoiceRate);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEmployeeInvoiceStatisticsByDaysForJanuary_Expect21Items()
    {
        var startDate = new DateTime(2022, 1, 1);
        var endDate = new DateTime(2022, 1, 31);

        var hours = new List<Hours>();

        for (int i = 0; i < 59; i++)
        {
            var newDate = startDate.AddDays(i);
            if (newDate.DayOfWeek == DayOfWeek.Sunday || newDate.DayOfWeek == DayOfWeek.Saturday)
            {
                continue;
            }

            hours.Add(new Hours
            {
                User = 1,
                Date = newDate,
                DayNumber = (short)newDate.DayOfYear,
                Id = i + 1,
                Locked = false,
                TaskId = 1,
                Value = 7.5m,
                Year = (short)newDate.Year
            });
        }

        var service = CreateInvoiceRateService(hours);

        var statistics = await service.GetEmployeeInvoiceStatisticsByPeriod(startDate, endDate, InvoicePeriods.Daily, ExtendPeriod.None);

        Assert.Equal(21, statistics.Count());

        Assert.Equal(7.5m, statistics.First().BillableHours);
        Assert.Equal(0m, statistics.First().VacationHours);
        Assert.Equal(0m, statistics.First().NonBillableHours);
        Assert.Equal(1m, statistics.First().InvoiceRate);
        Assert.Equal(0m, statistics.First().NonBillableInvoiceRate);
        Assert.Equal(new DateTime(2022, 1, 3), statistics.First().Start);
        Assert.Equal(new DateTime(2022, 1, 3).Date, statistics.First().End.Date);

        Assert.Equal(7.5m, statistics.Last().BillableHours);
        Assert.Equal(0m, statistics.Last().VacationHours);
        Assert.Equal(0m, statistics.Last().NonBillableHours);
        Assert.Equal(1m, statistics.Last().InvoiceRate);
        Assert.Equal(0m, statistics.Last().NonBillableInvoiceRate);
        Assert.Equal(endDate, statistics.Last().Start.Date);
        Assert.Equal(endDate.Date, statistics.Last().End.Date);
    }

    private InvoiceRateService CreateInvoiceRateService(List<Hours> hours)
    {
        var context = new AlvTimeDbContextBuilder()
            .CreateDbContext();
        PopulateContext(context, hours);
        ITimeRegistrationStorage storage = new TimeRegistrationStorage(context);
        return new InvoiceRateService(storage, _redDaysService, _options, _userContextMock.Object);
    }

    private void PopulateContext(AlvTime_dbContext context, List<Hours> hours)
    {
        foreach (var hour in hours)
            context.Hours.Add(hour);

        context.HourRate.Add(new HourRate
        {
            Id = 1,
            FromDate = new DateTime(2019, 01, 01),
            Rate = 1000,
            TaskId = 1
        });

        context.Customer.Add(new Customer
        {
            Id = 1,
            Name = "Alv"
        });
        context.Customer.Add(new Customer
        {
            Id = 2,
            Name = "Evil inc"
        });

        context.Project.Add(new Project
        {
            Id = 1,
            Name = "Internal",
            Customer = 1
        });
        context.Project.Add(new Project
        {
            Id = 2,
            Name = "Money Maker",
            Customer = 2
        });
        context.Project.Add(new Project
        {
            Id = 9,
            Name = "Absence Project",
            Customer = 1
        });

        context.Task.Add(new AlvTime.Persistence.DatabaseModels.Task
        {
            Id = 1,
            Description = "",
            Project = 2,
            Name = "Print Money"
        });

        context.Task.Add(new AlvTime.Persistence.DatabaseModels.Task
        {
            Id = 2,
            Description = "",
            Project = 1,
            Name = "Slave Labor"
        });

        context.Task.Add(new AlvTime.Persistence.DatabaseModels.Task
        {
            Id = 3,
            Description = "",
            Project = 1,
            Name = "Make Lunch"
        });

        context.CompensationRate.Add(new CompensationRate
        {
            TaskId = 1,
            Value = 1.5M,
            FromDate = new DateTime(2019, 01, 01)
        });
        context.CompensationRate.Add(new CompensationRate
        {
            TaskId = 2,
            Value = 1.0M,
            FromDate = new DateTime(2019, 01, 01)
        });
        context.CompensationRate.Add(new CompensationRate
        {
            TaskId = 3,
            Value = 0.5M,
            FromDate = new DateTime(2019, 01, 01)
        });

        context.Task.Add(new AlvTime.Persistence.DatabaseModels.Task
        {
            Id = 10,
            Description = "",
            Project = 9,
            Name = "PaidHoliday",
            Locked = false
        });
        context.Task.Add(new AlvTime.Persistence.DatabaseModels.Task
        {
            Id = 11,
            Description = "",
            Project = 9,
            Name = "SickDay",
            Locked = false
        });

        context.CompensationRate.Add(new CompensationRate
        {
            TaskId = 10,
            Value = 1.0M,
            FromDate = new DateTime(2019, 01, 01)
        });
        context.CompensationRate.Add(new CompensationRate
        {
            TaskId = 11,
            Value = 1.0M,
            FromDate = new DateTime(2019, 01, 01)
        });

        context.SaveChanges();
    }
}
