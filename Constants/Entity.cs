using System.ComponentModel;

namespace HotChocolateV14.Constants;

public enum Entity
{
    [Description("accounts")]
    Account,
    [Description("customers")]
    Customer,
    [Description("bills")]
    Bill
}
