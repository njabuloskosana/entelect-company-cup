using System.Text.Json;
using The_Neural_Pitwall.Models;

var inputPath = Path.Combine(AppContext.BaseDirectory, "Input", "input.json");
var json = await File.ReadAllTextAsync(inputPath);

var dto = JsonSerializer.Deserialize<RaceInputDto>(json, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

if (dto is null)
{
    throw new InvalidOperationException("Failed to deserialize race input.");
}

var config = RaceConfig.FromDto(dto);
var carState = CarState.CreateInitial(config.Car, config.Tyres.ByName["Soft"]);
var actionBuffer = new ActionBuffer();

Console.WriteLine("=== Race Config Loaded ===");
Console.WriteLine();

Console.WriteLine("Car");
Console.WriteLine($"  Max speed: {config.Car.MaxSpeedMps} m/s");
Console.WriteLine($"  Accel: {config.Car.AccelMSe2} m/s^2");
Console.WriteLine($"  Brake: {config.Car.BrakeMSe2} m/s^2");
Console.WriteLine($"  Limp constant: {config.Car.LimpConstantMps} m/s");
Console.WriteLine($"  Crawl constant: {config.Car.CrawlConstantMps} m/s");
Console.WriteLine($"  Fuel tank capacity: {config.Car.FuelTankCapacityL} L");
Console.WriteLine($"  Initial fuel: {config.Car.InitialFuelL} L");
Console.WriteLine($"  Fuel consumption: {config.Car.FuelConsumptionLPerM} L/m");
Console.WriteLine();

Console.WriteLine("Race");
Console.WriteLine($"  Name: {config.Race.Name}");
Console.WriteLine($"  Laps: {config.Race.Laps}");
Console.WriteLine($"  Base pit stop time: {config.Race.BasePitStopTimeS} s");
Console.WriteLine($"  Pit tyre swap time: {config.Race.PitTyreSwapTimeS} s");
Console.WriteLine($"  Pit refuel rate: {config.Race.PitRefuelRateLPerS} L/s");
Console.WriteLine($"  Corner crash penalty: {config.Race.CornerCrashPenaltyS} s");
Console.WriteLine($"  Pit exit speed: {config.Race.PitExitSpeedMps} m/s");
Console.WriteLine($"  Fuel soft cap limit: {config.Race.FuelSoftCapLimitL} L");
Console.WriteLine($"  Starting weather condition ID: {config.Race.StartingWeatherConditionId}");
Console.WriteLine($"  Time reference: {config.Race.TimeReferenceS} s");
Console.WriteLine();

Console.WriteLine("Track");
Console.WriteLine($"  Name: {dto.Track.Name}");
Console.WriteLine($"  Segments: {config.Track.Segments.Count}");
foreach (var segment in config.Track.Segments)
{
    var radiusText = segment.RadiusM is null ? string.Empty : $", Radius: {segment.RadiusM} m";
    var cornerSpeedText = segment.MaxCornerSpeedMps is null ? string.Empty : $", Max corner speed: {segment.MaxCornerSpeedMps:0.00} m/s";
    Console.WriteLine($"  - Id: {segment.Id}, Type: {segment.Type}, Length: {segment.LengthM} m{radiusText}{cornerSpeedText}");
}
Console.WriteLine();

Console.WriteLine("Tyres");
foreach (var compound in config.Tyres.ByName.Values)
{
    Console.WriteLine($"  - {compound.Name}");
    Console.WriteLine($"    Life span: {compound.LifeSpan}");
    Console.WriteLine($"    Base friction: {compound.BaseFriction}");
    Console.WriteLine($"    Dry multiplier: {compound.DryFrictionMultiplier}");
    Console.WriteLine($"    Cold multiplier: {compound.ColdFrictionMultiplier}");
    Console.WriteLine($"    Light rain multiplier: {compound.LightRainFrictionMultiplier}");
    Console.WriteLine($"    Heavy rain multiplier: {compound.HeavyRainFrictionMultiplier}");
}
Console.WriteLine();

Console.WriteLine("Weather");
foreach (var condition in config.Weather.OrderedConditions)
{
    Console.WriteLine($"  - Id: {condition.Id}, Condition: {condition.Condition}, Duration: {condition.DurationS} s");
    Console.WriteLine($"    Acceleration multiplier: {condition.AccelerationMultiplier}");
    Console.WriteLine($"    Deceleration multiplier: {condition.DecelerationMultiplier}");
}
Console.WriteLine();

Console.WriteLine("Initial Runtime State");
Console.WriteLine($"  Current velocity: {carState.CurrentVelocityMps} m/s");
Console.WriteLine($"  Current fuel: {carState.CurrentFuelL} L");
Console.WriteLine($"  Current tyre health: {carState.CurrentTyreHealth}");
Console.WriteLine($"  Elapsed race time: {carState.ElapsedRaceTimeS} s");
Console.WriteLine();

Console.WriteLine($"Action buffer ready: {actionBuffer.Actions.Count} actions");
