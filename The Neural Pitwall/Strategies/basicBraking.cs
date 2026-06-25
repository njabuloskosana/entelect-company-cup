using The_Neural_Pitwall.Models;

namespace The_Neural_Pitwall.Strategies;

public sealed class basicBraking : IStrategy
{
    public IReadOnlyList<SegmentStrategyResult> Execute(CarState carState, RaceConfig config, TyreCompound compound)
    {
        var results = new List<SegmentStrategyResult>();

        for (var lap = 1; lap <= config.Race.Laps; lap++)
        {
            foreach (var segment in config.Track.Segments)
            {
                var tyreHealthBefore = carState.CurrentTyreHealth;

                var targetSpeed = segment.IsCorner
                    ? Math.Max(0, segment.MaxCornerSpeedMps ?? 0)
                    : config.Car.MaxSpeedMps;

                var brakingDistance = 0d;
                if (!segment.IsCorner)
                {
                    var nextCorner = FindNextCorner(segment);
                    if (nextCorner is not null)
                    {
                        var nextCornerTargetSpeed = Math.Max(0, nextCorner.MaxCornerSpeedMps ?? 0);
                        brakingDistance = Formulas.BrakingDistance(targetSpeed, nextCornerTargetSpeed, config.Car.BrakeMSe2);
                    }
                }

                carState.CurrentVelocityMps = targetSpeed;
                carState.ElapsedRaceTimeS += segment.LengthM / Math.Max(1, targetSpeed);

                results.Add(new SegmentStrategyResult
                {
                    Lap = lap,
                    SegmentId = segment.Id,
                    SegmentType = segment.Type,
                    WeatherCondition = "Dry",
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
}
