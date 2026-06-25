using The_Neural_Pitwall;

namespace The_Neural_Pitwall.Models;

public sealed class RaceConfig
{
    public CarProperties Car { get; init; } = new();
    public RaceProperties Race { get; init; } = new();
    public TrackManager Track { get; init; } = new();
    public TyreCatalog Tyres { get; init; } = new();
    public IReadOnlyList<AvailableTyreSet> AvailableSets { get; init; } = [];
    public WeatherCatalog Weather { get; init; } = new();

    public static RaceConfig FromDto(RaceInputDto dto)
    {
        var tyres = TyreCatalog.FromDto(dto.Tyres, dto.AvailableSets);
        var weather = WeatherCatalog.FromDto(dto.Weather);
        var car = CarProperties.FromDto(dto.Car);
        var track = TrackManager.FromDto(dto.Track, tyres, weather, car, dto.Race.StartingWeatherConditionId);

        return new RaceConfig
        {
            Car = car,
            Race = RaceProperties.FromDto(dto.Race),
            Track = track,
            Tyres = tyres,
            AvailableSets = tyres.AvailableSets,
            Weather = weather
        };
    }
}

public sealed class CarProperties
{
    public double MaxSpeedMps { get; init; }
    public double AccelMSe2 { get; init; }
    public double BrakeMSe2 { get; init; }
    public double LimpConstantMps { get; init; }
    public double CrawlConstantMps { get; init; }
    public double FuelTankCapacityL { get; init; }
    public double InitialFuelL { get; init; }
    public double FuelConsumptionLPerM { get; init; }

    public static CarProperties FromDto(CarDto dto) => new()
    {
        MaxSpeedMps = dto.MaxSpeedMps,
        AccelMSe2 = dto.AccelMSe2,
        BrakeMSe2 = dto.BrakeMSe2,
        LimpConstantMps = dto.LimpConstantMps,
        CrawlConstantMps = dto.CrawlConstantMps,
        FuelTankCapacityL = dto.FuelTankCapacityL,
        InitialFuelL = dto.InitialFuelL,
        FuelConsumptionLPerM = dto.FuelConsumptionLPerM
    };
}

public sealed class RaceProperties
{
    public string Name { get; init; } = string.Empty;
    public int Laps { get; init; }
    public double BasePitStopTimeS { get; init; }
    public double PitTyreSwapTimeS { get; init; }
    public double PitRefuelRateLPerS { get; init; }
    public double CornerCrashPenaltyS { get; init; }
    public double PitExitSpeedMps { get; init; }
    public double FuelSoftCapLimitL { get; init; }
    public int StartingWeatherConditionId { get; init; }
    public double TimeReferenceS { get; init; }

    public static RaceProperties FromDto(RaceDto dto) => new()
    {
        Name = dto.Name,
        Laps = dto.Laps,
        BasePitStopTimeS = dto.BasePitStopTimeS,
        PitTyreSwapTimeS = dto.PitTyreSwapTimeS,
        PitRefuelRateLPerS = dto.PitRefuelRateLPerS,
        CornerCrashPenaltyS = dto.CornerCrashPenaltyS,
        PitExitSpeedMps = dto.PitExitSpeedMps,
        FuelSoftCapLimitL = dto.FuelSoftCapLimitL,
        StartingWeatherConditionId = dto.StartingWeatherConditionId,
        TimeReferenceS = dto.TimeReferenceS
    };
}

public sealed class TyreCatalog
{
    public Dictionary<string, TyreCompound> ByName { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<AvailableTyreSet> AvailableSets { get; init; } = [];

    public TyreCompound this[string compoundName] => ByName[compoundName];

    public static TyreCatalog FromDto(TyresDto dto, List<AvailableTyreSetDto> availableSets)
    {
        var byName = new Dictionary<string, TyreCompound>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, compoundDto) in dto.Properties)
        {
            byName[name] = TyreCompound.FromDto(name, compoundDto);
        }

        var sets = availableSets.Select(AvailableTyreSet.FromDto).ToArray();

        return new TyreCatalog
        {
            ByName = byName,
            AvailableSets = sets
        };
    }
}

public sealed class TyreCompound
{
    public string Name { get; init; } = string.Empty;
    public int LifeSpan { get; init; }
    public double BaseFriction { get; init; }
    public double DryFrictionMultiplier { get; init; }
    public double ColdFrictionMultiplier { get; init; }
    public double LightRainFrictionMultiplier { get; init; }
    public double HeavyRainFrictionMultiplier { get; init; }
    public double DryDegradation { get; init; }
    public double ColdDegradation { get; init; }
    public double LightRainDegradation { get; init; }
    public double HeavyRainDegradation { get; init; }

    public double GetFrictionMultiplier(string weatherCondition) =>
        weatherCondition.ToLowerInvariant() switch
        {
            "cold"        => ColdFrictionMultiplier,
            "light_rain"  => LightRainFrictionMultiplier,
            "heavy_rain"  => HeavyRainFrictionMultiplier,
            _             => DryFrictionMultiplier
        };

    public static TyreCompound FromDto(string name, TyreCompoundDto dto) => new()
    {
        Name = name,
        LifeSpan = dto.LifeSpan,
        BaseFriction = dto.BaseFriction,
        DryFrictionMultiplier = dto.DryFrictionMultiplier,
        ColdFrictionMultiplier = dto.ColdFrictionMultiplier,
        LightRainFrictionMultiplier = dto.LightRainFrictionMultiplier,
        HeavyRainFrictionMultiplier = dto.HeavyRainFrictionMultiplier,
        DryDegradation = dto.DryDegradation,
        ColdDegradation = dto.ColdDegradation,
        LightRainDegradation = dto.LightRainDegradation,
        HeavyRainDegradation = dto.HeavyRainDegradation
    };
}

public sealed class AvailableTyreSet
{
    public IReadOnlyList<int> Ids { get; init; } = [];
    public string Compound { get; init; } = string.Empty;

    public static AvailableTyreSet FromDto(AvailableTyreSetDto dto) => new()
    {
        Ids = dto.Ids.ToArray(),
        Compound = dto.Compound
    };
}

public sealed class WeatherCatalog
{
    public Dictionary<int, WeatherCondition> ById { get; init; } = new();
    public IReadOnlyList<WeatherCondition> OrderedConditions { get; init; } = [];

    public WeatherCondition this[int id] => ById[id];

    public static WeatherCatalog FromDto(WeatherDto dto)
    {
        var ordered = dto.Conditions
            .Select(WeatherCondition.FromDto)
            .OrderBy(c => c.Id)
            .ToArray();

        var byId = ordered.ToDictionary(c => c.Id);

        return new WeatherCatalog
        {
            ById = byId,
            OrderedConditions = ordered
        };
    }
}

public sealed class WeatherCondition
{
    public int Id { get; init; }
    public string Condition { get; init; } = string.Empty;
    public double DurationS { get; init; }
    public double AccelerationMultiplier { get; init; }
    public double DecelerationMultiplier { get; init; }

    public static WeatherCondition FromDto(WeatherConditionDto dto) => new()
    {
        Id = dto.Id,
        Condition = dto.Condition,
        DurationS = dto.DurationS,
        AccelerationMultiplier = dto.AccelerationMultiplier,
        DecelerationMultiplier = dto.DecelerationMultiplier
    };
}

public sealed class TrackManager
{
    public IReadOnlyList<TrackSegment> Segments { get; init; } = [];
    public Dictionary<int, TrackSegment> ById { get; init; } = new();
    public IReadOnlyList<TrackSegment> Corners { get; init; } = [];

    public static TrackManager FromDto(TrackDto dto, TyreCatalog tyres, WeatherCatalog weather, CarProperties car, int startingWeatherConditionId = 0)
    {
        var segments = dto.Segments
            .Select(TrackSegment.FromDto)
            .ToArray();

        var byId = segments.ToDictionary(s => s.Id);

        TrackSegment? next = null;
        for (var i = segments.Length - 1; i >= 0; i--)
        {
            var segment = segments[i];
            segment.Next = next;
            if (next is not null)
            {
                next.Previous = segment;
            }

            next = segment;
        }

        // Wire each segment's NextCorner for O(1) look-ahead during braking-point passes.
        TrackSegment? nextCorner = null;
        for (var i = segments.Length - 1; i >= 0; i--)
        {
            if (segments[i].IsCorner)
                nextCorner = segments[i];
            segments[i].NextCorner = nextCorner;
        }

        // Resolve the starting weather condition to pick the right tyre friction multiplier.
        weather.ById.TryGetValue(startingWeatherConditionId, out var startingWeather);
        var weatherCondition = startingWeather?.Condition ?? "dry";
        var initialTyre = tyres.ByName.Values.FirstOrDefault();
        var tyreFriction = initialTyre is null
            ? 1d
            : Formulas.TyreFriction(initialTyre.BaseFriction, 0, initialTyre.GetFrictionMultiplier(weatherCondition));

        foreach (var segment in segments)
        {
            if (segment.IsCorner)
                segment.MaxCornerSpeedMps = Formulas.MaxCornerSpeed(segment.RadiusM ?? 0, tyreFriction, car.CrawlConstantMps);
        }

        return new TrackManager
        {
            Segments = segments,
            ById = byId,
            Corners = segments.Where(s => s.IsCorner).ToArray()
        };
    }

    public TrackSegment? GetNextSegment(int currentId)
        => ById.TryGetValue(currentId, out var segment) ? segment.Next : null;

    public TrackSegment? GetPreviousSegment(int currentId)
        => ById.TryGetValue(currentId, out var segment) ? segment.Previous : null;

    /// <summary>
    /// Distance (m) required to decelerate from <paramref name="fromSpeedMps"/> to
    /// <paramref name="toSpeedMps"/> under constant braking of <paramref name="brakeMSe2"/>.
    /// Returns 0 when no braking is needed.
    /// Uses kinematics: s = (u² - v²) / (2a)
    /// </summary>
    public static double GetBrakingDistanceM(double fromSpeedMps, double toSpeedMps, double brakeMSe2)
    {
        if (fromSpeedMps <= toSpeedMps || brakeMSe2 <= 0)
            return 0;

        return (fromSpeedMps * fromSpeedMps - toSpeedMps * toSpeedMps) / (2 * brakeMSe2);
    }

}

public sealed class TrackSegment
{
    public int Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public double LengthM { get; init; }
    public double? RadiusM { get; init; }
    public bool IsCorner => Type.Equals("corner", StringComparison.OrdinalIgnoreCase);
    public double? MaxCornerSpeedMps { get; set; }
    public TrackSegment? Next { get; set; }
    public TrackSegment? Previous { get; set; }
    public TrackSegment? NextCorner { get; set; }

    public static TrackSegment FromDto(TrackSegmentDto dto) => new()
    {
        Id = dto.Id,
        Type = dto.Type,
        LengthM = dto.LengthM,
        RadiusM = dto.RadiusM
    };
}

public sealed class CarState
{
    public double CurrentVelocityMps { get; set; }
    public double CurrentFuelL { get; set; }
    public double CurrentTyreHealth { get; set; }
    public double ElapsedRaceTimeS { get; set; }

    public static CarState CreateInitial(CarProperties car, TyreCompound compound)
        => new()
        {
            CurrentVelocityMps = 0,
            CurrentFuelL = car.InitialFuelL,
            CurrentTyreHealth = compound.LifeSpan,
            ElapsedRaceTimeS = 0
        };
}

public sealed class ActionBuffer
{
    public List<LapAction> Actions { get; } = [];

    public void Add(LapAction action) => Actions.Add(action);
}

public sealed class LapAction
{
    public int Lap { get; init; }
    public int SegmentId { get; init; }
    public string Action { get; init; } = string.Empty;
}
