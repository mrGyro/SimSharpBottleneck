using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public class BottleneckFixedScenario : ScenarioBase
{
    public BottleneckFixedScenario() 
        : base("BottleneckFixed", "Improved Assembly (bottleneck) by 20% (12 min → 9.6 min)")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        return new CustomProductionLine(sim, metrics,
            cuttingTime: TimeSpan.FromMinutes(8),
            assemblyTime: TimeSpan.FromMinutes(9.6),   // Improved bottleneck by 20%!
            testingTime: TimeSpan.FromMinutes(10),
            packagingTime: TimeSpan.FromMinutes(6)
        );
    }
}