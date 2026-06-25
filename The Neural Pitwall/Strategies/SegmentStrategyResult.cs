namespace The_Neural_Pitwall.Strategies;

public sealed class SegmentStrategyResult
{
    public int Lap { get; init; }
    public int SegmentId { get; init; }
    public string SegmentType { get; init; } = string.Empty;
    public string WeatherCondition { get; init; } = string.Empty;
    public double TargetSpeedMps { get; init; }
    public double BrakingDistanceM { get; init; }
    public double TyreHealthBefore { get; init; }
    public double TyreHealthAfter { get; init; }
}
