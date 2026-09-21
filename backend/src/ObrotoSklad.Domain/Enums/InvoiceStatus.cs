using System.ComponentModel;

namespace ObrotoSklad.Domain.Enums;

public enum InvoiceStatus
{
    [Description("issued")]
    Issued,
    [Description("paid")]
    Paid,
    [Description("overdue")]
    Overdue,
    [Description("cancelled")]
    Cancelled

}
