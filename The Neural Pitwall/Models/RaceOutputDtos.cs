using System.Text.Json.Serialization;

namespace The_Neural_Pitwall.Models;

public sealed class RaceSubmission
{
    [JsonPropertyName("initial_tyre_id")]
    public int InitialTyreId { get; set; }

    [JsonPropertyName("laps")]
    public List<LapSubmission> Laps { get; set; } = [];
}

public sealed class LapSubmission
{
    [JsonPropertyName("lap")]
    public int Lap { get; set; }

    [JsonPropertyName("segments")]
    public List<SegmentSubmission> Segments { get; set; } = [];

    [JsonPropertyName("pit")]
    public PitSubmission Pit { get; set; } = new();
}

public sealed class SegmentSubmission
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("target_m/s")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TargetMps { get; set; }

    [JsonPropertyName("brake_start_m_before_next")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? BrakeStartMBeforeNext { get; set; }
}

public sealed class PitSubmission
{
    [JsonPropertyName("enter")]
    public bool Enter { get; set; }

    [JsonPropertyName("tyre_change_set_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TyreChangeSetId { get; set; }

    [JsonPropertyName("fuel_refuel_amount_l")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? FuelRefuelAmountL { get; set; }
}
