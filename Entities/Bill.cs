using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotChocolateV14.Entities;

[Table("bills")]
public class Bill
{
    [Key]
    public int Id { get; set; }

    public int Number { get; set; }

    public Month Month { get; set; }

    public bool IsActive { get; set; }

    public Status Status { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal Amount { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; }
}

public enum Status
{
    Paid,
    NotPaid
}

public enum Month
{
    January = 1,
    February,
    March,
    April,
    May,
    June,
    July,
    August,
    September,
    October,
    November,
    December
}
