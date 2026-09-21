using System.ComponentModel;

namespace ObrotoSklad.Domain;

public enum MovementType
{
    [Description("in")]
    In,
    [Description("out")]
    Out,
    [Description("reserved")]
    Reserved,
    [Description("released")]
    Released
}
