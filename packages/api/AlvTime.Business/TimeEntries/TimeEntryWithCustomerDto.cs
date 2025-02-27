using System;

namespace AlvTime.Business.TimeEntries;

public class TimeEntryWithCustomerDto
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string CustomerName { get; set; }
    public int TaskId { get; set; }
}
