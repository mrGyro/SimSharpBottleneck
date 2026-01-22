using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public class BottleneckFixedScenario : ScenarioBase
{
    public BottleneckFixedScenario() 
        : base("BottleneckFixed", "Ускорили Assembly (bottleneck) на 20% (12 мин → 9.6 мин)")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        return new CustomProductionLine(sim, metrics,
            cuttingTime: TimeSpan.FromMinutes(8),
            assemblyTime: TimeSpan.FromMinutes(9.6),   // Ускорили bottleneck на 20%!
            testingTime: TimeSpan.FromMinutes(10),
            packagingTime: TimeSpan.FromMinutes(6)
        );
    }
}