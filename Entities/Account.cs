using System.ComponentModel.DataAnnotations.Schema;

namespace HotChocolateV14.Entities;

[Table("accounts")]
public class Account
{
    public int Id { get; set; }

    public string Number { get; set; }

    public bool IsActive { get; set; }

    public AccountType Type { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; }

    [UseFiltering]
    [UseSorting]
    public virtual ICollection<Bill> Bills { get; set; }
}

public enum AccountType
{
    Domestic,
    Commercial
}