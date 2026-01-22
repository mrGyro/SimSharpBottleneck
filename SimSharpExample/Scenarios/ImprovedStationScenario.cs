using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public class ImprovedStationScenario : ScenarioBase
{
    public ImprovedStationScenario() 
        : base("ImprovedCutting", "Improved Cutting by 20% (8 min → 6.4 min), but this is NOT a bottleneck!")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        var line = new ProductionLine(sim, metrics);
        
        // Recreate stations with new parameters
        return new CustomProductionLine(sim, metrics,
            cuttingTime: TimeSpan.FromMinutes(6.4),   // Improved by 20%!
            assemblyTime: TimeSpan.FromMinutes(12),    // No changes (bottleneck)
            testingTime: TimeSpan.FromMinutes(10),
            packagingTime: TimeSpan.FromMinutes(6)
        );
    }
}

// Custom production line with configurable times
public class CustomProductionLine : ProductionLine
{
    public CustomProductionLine(Simulation sim, MetricsCollector metrics,
        TimeSpan cuttingTime, TimeSpan assemblyTime, 
        TimeSpan testingTime, TimeSpan packagingTime)
        : base(sim, metrics)
    {
        // Recreate stations with new times
        CuttingStation = new Station(sim, "Cutting", 1, cuttingTime);
        AssemblyStation = new Station(sim, "Assembly", 1, assemblyTime);
        TestingStation = new Station(sim, "Testing", 1, testingTime);
        PackagingStation = new Station(sim, "Packaging", 1, packagingTime);
    }
}