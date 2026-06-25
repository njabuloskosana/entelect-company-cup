using The_Neural_Pitwall.Models;

namespace The_Neural_Pitwall.Strategies;

public sealed class MaximizingCornerSpeed : IStrategy
{
    private const double KStraight = 0.0000166;
    private const double KBraking = 0.0398;
    private const double KCorner = 0.000265;

    public IReadOnlyList<SegmentStrategyResult> Execute(CarState carState, RaceConfig config, TyreCompound compound)
    {
        var results = new List<SegmentStrategyResult>();

        for (var lap = 1; lap <= config.Race.Laps; lap++)
        {
            foreach (var segment in config.Track.Segments)
            {
                var weather = ResolveWeather(config.Weather.OrderedConditions, carState.ElapsedRaceTimeS);
                var tyreHealthBefore = carState.CurrentTyreHealth;

                var weatherFrictionMultiplier = GetWeatherFrictionMultiplier(compound, weather.Condition);
                var weatherDegradationRate = GetWeatherDegradationRate(compound, weather.Condition);
                var tyreWearRatio = GetTyreWearRatio(carState.CurrentTyreHealth, compound.LifeSpan);

                var targetSpeed = config.Car.MaxSpeedMps;
                var brakingDistance = 0d;
                var degradation = 0d;

                if (segment.IsCorner)
                {
                    var tyreFriction = Formulas.TyreFriction(compound.BaseFriction, tyreWearRatio, weatherFrictionMultiplier);
                    var crawlConstant = Formulas.CrawlConstant(config.Car.CrawlConstantMps, tyreWearRatio, weatherFrictionMultiplier);
                    targetSpeed = Math.Max(0, Formulas.MaxCornerSpeed(segment.RadiusM ?? 0, tyreFriction, crawlConstant));

                    degradation += Formulas.CornerDegradation(
                        KCorner,
                        weatherDegradationRate,
                        Math.Max(1, segment.RadiusM ?? 1),
                        targetSpeed);
                }
                else
                {
                    targetSpeed = config.Car.MaxSpeedMps;
                    var nextCorner = FindNextCorner(segment);
                    if (nextCorner is not null)
                    {
                        var nextCornerTyreFriction = Formulas.TyreFriction(compound.BaseFriction, tyreWearRatio, weatherFrictionMultiplier);
                        var nextCornerCrawl = Formulas.CrawlConstant(config.Car.CrawlConstantMps, tyreWearRatio, weatherFrictionMultiplier);
                        var nextCornerTargetSpeed = Math.Max(0, Formulas.MaxCornerSpeed(nextCorner.RadiusM ?? 0, nextCornerTyreFriction, nextCornerCrawl));

                        brakingDistance = Formulas.BrakingDistance(targetSpeed, nextCornerTargetSpeed, config.Car.BrakeMSe2);
                        degradation += Formulas.BrakeDegradation(KBraking, weatherDegradationRate, targetSpeed, nextCornerTargetSpeed);
                    }

                    degradation += Formulas.StraightDegradation(KStraight, weatherDegradationRate, segment.LengthM);
                }

                carState.CurrentTyreHealth = Math.Max(0, carState.CurrentTyreHealth - degradation);
                carState.CurrentVelocityMps = targetSpeed;

                var sectionTime = segment.LengthM / Math.Max(1, targetSpeed);
                carState.ElapsedRaceTimeS += sectionTime;

                results.Add(new SegmentStrategyResult
                {
                    Lap = lap,
                    SegmentId = segment.Id,
                    SegmentType = segment.Type,
                    WeatherCondition = weather.Condition,
                    TargetSpeedMps = targetSpeed,
                    BrakingDistanceM = brakingDistance,
                    TyreHealthBefore = tyreHealthBefore,
                    TyreHealthAfter = carState.CurrentTyreHealth
                });
            }
        }

        return results;
    }

    private static TrackSegment? FindNextCorner(TrackSegment segment)
    {
        var cursor = segment.Next;
        while (cursor is not null)
        {
            if (cursor.IsCorner)
            {
                return cursor;
            }

            cursor = cursor.Next;
        }

        return null;
    }

    private static WeatherCondition ResolveWeather(IReadOnlyList<WeatherCondition> weather, double elapsedRaceTimeS)
    {
        if (weather.Count == 0)
        {
            return new WeatherCondition { Id = -1, Condition = "Dry", DurationS = double.MaxValue };
        }

        var cycleDuration = weather.Sum(w => w.DurationS);
        var adjustedTime = cycleDuration > 0 ? elapsedRaceTimeS % cycleDuration : 0;

        var cumulative = 0d;
        foreach (var condition in weather)
        {
            cumulative += condition.DurationS;
            if (adjustedTime <= cumulative)
            {
                return condition;
            }
        }

        return weather[^1];
    }

    private static double GetTyreWearRatio(double tyreHealth, int lifeSpan)
    {
        if (lifeSpan <= 0)
        {
            return 1;
        }

        var ratio = 1d - (tyreHealth / lifeSpan);
        return Math.Clamp(ratio, 0, 1);
    }

    private static double GetWeatherFrictionMultiplier(TyreCompound compound, string weatherCondition)
        => weatherCondition.ToLowerInvariant() switch
        {
            "dry" => compound.DryFrictionMultiplier,
            "cold" => compound.ColdFrictionMultiplier,
            "light rain" => compound.LightRainFrictionMultiplier,
            "heavy rain" => compound.HeavyRainFrictionMultiplier,
            _ => compound.DryFrictionMultiplier
        };

    private static double GetWeatherDegradationRate(TyreCompound compound, string weatherCondition)
        => weatherCondition.ToLowerInvariant() switch
        {
            "dry" => compound.DryDegradation,
            "cold" => compound.ColdDegradation,
            "light rain" => compound.LightRainDegradation,
            "heavy rain" => compound.HeavyRainDegradation,
            _ => compound.DryDegradation
        };
}
