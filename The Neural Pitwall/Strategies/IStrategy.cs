using The_Neural_Pitwall.Models;

namespace The_Neural_Pitwall.Strategies;

public interface IStrategy
{
    IReadOnlyList<SegmentStrategyResult> Execute(CarState carState, RaceConfig config, TyreCompound compound);
}
