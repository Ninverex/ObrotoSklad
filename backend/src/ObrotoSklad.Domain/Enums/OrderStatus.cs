using System.ComponentModel;

namespace ObrotoSklad.Domain;

public enum OrderStatus
{
    [Description("draft")]
    Draft,
    [Description("confirmed")]
    Confirmed,
    [Description("stockReserved")]
    StockReserved,
    [Description("onhold")]
    OnHold,
    [Description("fullfiled")]
    Fullfiled,
    [Description("invoiced")]
    Invoiced,
    [Description("canceled")]
    Canceled

}
