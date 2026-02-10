using AssetOperations.Domain.Enums;

namespace AssetOperations.Domain.ValueObjects;

public class MaintenancePeriod
{
    public int Value { get; }
    public PeriodUnit Unit { get; }

    public MaintenancePeriod(int value, PeriodUnit unit)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.");

        Value = value;
        Unit = unit;
    }

    public DateTime AddTo(DateTime date)
    {
        return Unit switch
        {
            PeriodUnit.Day => date.AddDays(Value),
            PeriodUnit.Week => date.AddDays(7 * Value),
            PeriodUnit.Month => date.AddMonths(Value),
            PeriodUnit.Year => date.AddYears(Value),
            _ => throw new InvalidOperationException()
        };
    }
}
