using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;
using SimSharpExample.Scenarios;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  PRODUCTION LINE SIMULATION - BOTTLENECK ANALYSIS            ║");
Console.WriteLine("║  Demonstrating local optimization paradox                    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

// Number of items for each scenario
int itemCount = 20;

// Create analyzer for comparison
var comparisonAnalyzer = new ComparisonAnalyzer();

// Run all scenarios
var scenarios = new List<ScenarioBase>
{
    new BaselineScenario(),
    new ImprovedStationScenario(),
    new BottleneckFixedScenario(),
    new StochasticScenario()
};

foreach (var scenario in scenarios)
{
    // Run scenario
    var sim = new Simulation();
    var metrics = new MetricsCollector();
    var line = scenario.CreateProductionLine(sim, metrics);
    var exporter = new CsvExporter($"Results/{scenario.Name}");

    Console.WriteLine($"\n{'═', 70}");
    Console.WriteLine($"  Scenario: {scenario.Name}");
    Console.WriteLine($"  {scenario.Description}");
    Console.WriteLine($"{'═', 70}");

    // Generate items (stochastically for Stochastic, otherwise normally)
    if (scenario is StochasticScenario)
    {
        sim.Process(line.GenerateItems(sim, itemCount, useStochasticArrival: true, meanInterarrivalMinutes: 5.0));
    }
    else
    {
        sim.Process(line.GenerateItems(sim, itemCount));
    }
    
    sim.Run();
    
    var totalTime = sim.Now - new DateTime(1970, 1, 1);
    
    Console.WriteLine($"\nSimulation time: {totalTime}");
    metrics.PrintStationStats(line, totalTime);
    metrics.PrintSummary(totalTime);
    
    exporter.ExportAll(line, metrics, totalTime);
    
    // Collect data for comparison
    var utilization = new Dictionary<string, double>
    {
        { "Cutting", line.CuttingStation.GetUtilization(totalTime) },
        { "Assembly", line.AssemblyStation.GetUtilization(totalTime) },
        { "Testing", line.TestingStation.GetUtilization(totalTime) },
        { "Packaging", line.PackagingStation.GetUtilization(totalTime) }
    };
    
    comparisonAnalyzer.AddResult(scenario.Name, totalTime, metrics, utilization);
    
    Console.WriteLine();
}

// Print comparative analysis
comparisonAnalyzer.PrintComparison();
comparisonAnalyzer.ExportComparisonCsv();

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ANALYSIS COMPLETE!                                          ║");
Console.WriteLine("║  Check Results/* folders for CSV files                       ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");