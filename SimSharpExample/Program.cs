using SimSharp;
using SimSharpExample.SimulationProcess;
using SimSharpExample.Metrics;
using SimSharpExample.Scenarios;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  СИМУЛЯЦИЯ ПРОИЗВОДСТВЕННОЙ ЛИНИИ - АНАЛИЗ УЗКИХ МЕСТ       ║");
Console.WriteLine("║  Демонстрация парадокса локальной оптимизации                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

// Количество деталей для каждого сценария
int itemCount = 20;

// Создаем анализатор для сравнения
var comparisonAnalyzer = new ComparisonAnalyzer();

// Запускаем все сценарии
var scenarios = new List<ScenarioBase>
{
    new BaselineScenario(),
    new ImprovedStationScenario(),
    new BottleneckFixedScenario(),
    new StochasticScenario()
};

foreach (var scenario in scenarios)
{
    // Запускаем сценарий
    var sim = new Simulation();
    var metrics = new MetricsCollector();
    var line = scenario.CreateProductionLine(sim, metrics);
    var exporter = new CsvExporter($"Results/{scenario.Name}");

    Console.WriteLine($"\n{'═', 70}");
    Console.WriteLine($"  Сценарий: {scenario.Name}");
    Console.WriteLine($"  {scenario.Description}");
    Console.WriteLine($"{'═', 70}");

    // Генерируем детали (стохастически для Stochastic, иначе обычно)
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
    
    Console.WriteLine($"\nВремя симуляции: {totalTime}");
    metrics.PrintStationStats(line, totalTime);
    metrics.PrintSummary(totalTime);
    
    exporter.ExportAll(line, metrics, totalTime);
    
    // Собираем данные для сравнения
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

// Выводим сравнительный анализ
comparisonAnalyzer.PrintComparison();
comparisonAnalyzer.ExportComparisonCsv();

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  АНАЛИЗ ЗАВЕРШЕН!                                            ║");
Console.WriteLine("║  Проверьте папки Results/* для CSV файлов                    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");