using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public class ImprovedStationScenario : ScenarioBase
{
    public ImprovedStationScenario() 
        : base("ImprovedCutting", "Ускорили Cutting на 20% (8 мин → 6.4 мин), но это НЕ bottleneck!")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        var line = new ProductionLine(sim, metrics);
        
        // Пересоздаем станции с новыми параметрами
        return new CustomProductionLine(sim, metrics,
            cuttingTime: TimeSpan.FromMinutes(6.4),   // Ускорили на 20%!
            assemblyTime: TimeSpan.FromMinutes(12),    // Без изменений (bottleneck)
            testingTime: TimeSpan.FromMinutes(10),
            packagingTime: TimeSpan.FromMinutes(6)
        );
    }
}

// Кастомная производственная линия с настраиваемыми временами
public class CustomProductionLine : ProductionLine
{
    public CustomProductionLine(Simulation sim, MetricsCollector metrics,
        TimeSpan cuttingTime, TimeSpan assemblyTime, 
        TimeSpan testingTime, TimeSpan packagingTime)
        : base(sim, metrics)
    {
        // Пересоздаем станции с новыми временами
        CuttingStation = new Station(sim, "Cutting", 1, cuttingTime);
        AssemblyStation = new Station(sim, "Assembly", 1, assemblyTime);
        TestingStation = new Station(sim, "Testing", 1, testingTime);
        PackagingStation = new Station(sim, "Packaging", 1, packagingTime);
    }
}