using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public class BaselineScenario : ScenarioBase
{
    public BaselineScenario() 
        : base("Baseline", "Базовая конфигурация - все станции со стандартным временем обработки")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        return new ProductionLine(sim, metrics);
    }
}