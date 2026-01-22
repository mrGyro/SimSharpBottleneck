using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public class BaselineScenario : ScenarioBase
{
    public BaselineScenario() 
        : base("Baseline", "Baseline configuration - all stations with standard processing time")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        return new ProductionLine(sim, metrics);
    }
}