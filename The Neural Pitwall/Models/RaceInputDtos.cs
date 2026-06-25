using System.Text.Json.Serialization;

namespace The_Neural_Pitwall.Models;

public sealed class RaceInputDto
{
    [JsonPropertyName("car")]
    public CarDto Car { get; set; } = new();

    [JsonPropertyName("race")]
    public RaceDto Race { get; set; } = new();

    [JsonPropertyName("track")]
    public TrackDto Track { get; set; } = new();

    [JsonPropertyName("tyres")]
    public TyresDto Tyres { get; set; } = new();

    [JsonPropertyName("available_sets")]
    public List<AvailableTyreSetDto> AvailableSets { get; set; } = [];

    [JsonPropertyName("weather")]
    public WeatherDto Weather { get; set; } = new();
}

public sealed class CarDto
{
    [JsonPropertyName("max_speed_m/s")]
    public double MaxSpeedMps { get; set; }

    [JsonPropertyName("accel_m/se2")]
    public double AccelMSe2 { get; set; }

    [JsonPropertyName("brake_m/se2")]
    public double BrakeMSe2 { get; set; }

    [JsonPropertyName("limp_constant_m/s")]
    public double LimpConstantMps { get; set; }

    [JsonPropertyName("crawl_constant_m/s")]
    public double CrawlConstantMps { get; set; }

    [JsonPropertyName("fuel_tank_capacity_l")]
    public double FuelTankCapacityL { get; set; }

    [JsonPropertyName("initial_fuel_l")]
    public double InitialFuelL { get; set; }

    [JsonPropertyName("fuel_consumption_l/m")]
    public double FuelConsumptionLPerM { get; set; }
}

public sealed class RaceDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("laps")]
    public int Laps { get; set; }

    [JsonPropertyName("base_pit_stop_time_s")]
    public double BasePitStopTimeS { get; set; }

    [JsonPropertyName("pit_tyre_swap_time_s")]
    public double PitTyreSwapTimeS { get; set; }

    [JsonPropertyName("pit_refuel_rate_l/s")]
    public double PitRefuelRateLPerS { get; set; }

    [JsonPropertyName("corner_crash_penalty_s")]
    public double CornerCrashPenaltyS { get; set; }

    [JsonPropertyName("pit_exit_speed_m/s")]
    public double PitExitSpeedMps { get; set; }

    [JsonPropertyName("fuel_soft_cap_limit_l")]
    public double FuelSoftCapLimitL { get; set; }

    [JsonPropertyName("starting_weather_condition_id")]
    public int StartingWeatherConditionId { get; set; }

    [JsonPropertyName("time_reference_s")]
    public double TimeReferenceS { get; set; }
}

public sealed class TrackDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("segments")]
    public List<TrackSegmentDto> Segments { get; set; } = [];
}

public sealed class TrackSegmentDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("length_m")]
    public double LengthM { get; set; }

    [JsonPropertyName("radius_m")]
    public double? RadiusM { get; set; }
}

public sealed class TyresDto
{
    [JsonPropertyName("properties")]
    public Dictionary<string, TyreCompoundDto> Properties { get; set; } = new();
}

public sealed class TyreCompoundDto
{
    [JsonPropertyName("life_span")]
    public int LifeSpan { get; set; }

    [JsonPropertyName("base_friction")]
    public double BaseFriction { get; set; }

    [JsonPropertyName("dry_friction_multiplier")]
    public double DryFrictionMultiplier { get; set; }

    [JsonPropertyName("cold_friction_multiplier")]
    public double ColdFrictionMultiplier { get; set; }

    [JsonPropertyName("light_rain_friction_multiplier")]
    public double LightRainFrictionMultiplier { get; set; }

    [JsonPropertyName("heavy_rain_friction_multiplier")]
    public double HeavyRainFrictionMultiplier { get; set; }

    [JsonPropertyName("dry_degradation")]
    public double DryDegradation { get; set; }

    [JsonPropertyName("cold_degradation")]
    public double ColdDegradation { get; set; }

    [JsonPropertyName("light_rain_degradation")]
    public double LightRainDegradation { get; set; }

    [JsonPropertyName("heavy_rain_degradation")]
    public double HeavyRainDegradation { get; set; }
}

public sealed class AvailableTyreSetDto
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = [];

    [JsonPropertyName("compound")]
    public string Compound { get; set; } = string.Empty;
}

public sealed class WeatherDto
{
    [JsonPropertyName("conditions")]
    public List<WeatherConditionDto> Conditions { get; set; } = [];
}

public sealed class WeatherConditionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonPropertyName("duration_s")]
    public double DurationS { get; set; }

    [JsonPropertyName("acceleration_multiplier")]
    public double AccelerationMultiplier { get; set; }

    [JsonPropertyName("deceleration_multiplier")]
    public double DecelerationMultiplier { get; set; }
}
