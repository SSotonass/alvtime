using System;

namespace AlvTime.Business.InvoiceRate;

public class InvoiceStatisticsDto
{
    public decimal BillableHours { get; set; }
    public decimal NonBillableHours { get; set; }
    public decimal VacationHours { get; set; }
    public decimal InvoiceRate { get; set; }
    public decimal NonBillableInvoiceRate { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    
    public enum InvoicePeriods
    {
        Daily,
        Weekly,
        Monthly,
        Annualy
    }

    [Flags]
    public enum ExtendPeriod
    { 
        None = 0,
        Start = 1,
        End = 2
    }
}
