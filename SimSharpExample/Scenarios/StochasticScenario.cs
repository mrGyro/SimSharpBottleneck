using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;
using SimSharpExample.Utils;

namespace SimSharpExample.Scenarios;

public class StochasticScenario : ScenarioBase
{
    public StochasticScenario() 
        : base("Stochastic", "Realistic simulation with random processing and arrival times")
    {
    }

    public override ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics)
    {
        // Set seed for reproducibility
        RandomDistributions.SetSeed(42);
        
        return new StochasticProductionLine(sim, metrics);
    }
    
    public override void Run(int itemCount = 10)
    {
        var sim = new Simulation();
        var metrics = new MetricsCollector();
        var line = CreateProductionLine(sim, metrics);
        var exporter = new CsvExporter($"Results/{Name}");

        Console.WriteLine($"\n{'═', 70}");
        Console.WriteLine($"  Scenario: {Name}");
        Console.WriteLine($"  {Description}");
        Console.WriteLine($"{'=', 70}");

        // Generate items with stochastic arrival
        sim.Process(line.GenerateItems(sim, itemCount, useStochasticArrival: true, meanInterarrivalMinutes: 5.0));
        
        // Run
        sim.Run();
        
        // Results
        var totalTime = sim.Now - new DateTime(1970, 1, 1);
        
        Console.WriteLine($"\nSimulation time: {totalTime}");
        metrics.PrintStationStats(line, totalTime);
        metrics.PrintSummary(totalTime);
        
        // Export
        exporter.ExportAll(line, metrics, totalTime);
    }
}

// Stochastic production line
public class StochasticProductionLine : ProductionLine
{
    public StochasticProductionLine(Simulation sim, MetricsCollector metrics)
        : base(sim, metrics)
    {
        // Recreate stations with stochastic behavior
        CuttingStation = new Station(sim, "Cutting", 1, TimeSpan.FromMinutes(8), useStochastic: true);
        AssemblyStation = new Station(sim, "Assembly", 1, TimeSpan.FromMinutes(12), useStochastic: true);
        TestingStation = new Station(sim, "Testing", 1, TimeSpan.FromMinutes(10), useStochastic: true);
        PackagingStation = new Station(sim, "Packaging", 1, TimeSpan.FromMinutes(6), useStochastic: true);
    }
}