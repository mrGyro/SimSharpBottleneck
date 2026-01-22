using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;

namespace SimSharpExample.Scenarios;

public abstract class ScenarioBase
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }

    protected ScenarioBase(string name, string description)
    {
        Name = name;
        Description = description;
    }

    // Each scenario defines its own parameters
    public abstract ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics);

    // General launch logic (can be overridden)
    public virtual void Run(int itemCount = 10)
    {
        var sim = new Simulation();
        var metrics = new MetricsCollector();
        var line = CreateProductionLine(sim, metrics);
        var exporter = new CsvExporter($"Results/{Name}");

        Console.WriteLine($"\n{'=', 70}");
        Console.WriteLine($"  Scenario: {Name}");
        Console.WriteLine($"  {Description}");
        Console.WriteLine($"{'=', 70}");

        // Generate items
        sim.Process(line.GenerateItems(sim, itemCount));
        
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