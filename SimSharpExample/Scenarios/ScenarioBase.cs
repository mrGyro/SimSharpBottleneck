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

    // Каждый сценарий определяет свои параметры
    public abstract ProductionLine CreateProductionLine(Simulation sim, MetricsCollector metrics);

    // Общая логика запуска (может быть переопределена)
    public virtual void Run(int itemCount = 10)
    {
        var sim = new Simulation();
        var metrics = new MetricsCollector();
        var line = CreateProductionLine(sim, metrics);
        var exporter = new CsvExporter($"Results/{Name}");

        Console.WriteLine($"\n{'═', 70}");
        Console.WriteLine($"  Сценарий: {Name}");
        Console.WriteLine($"  {Description}");
        Console.WriteLine($"{'═', 70}");

        // Генерируем детали
        sim.Process(line.GenerateItems(sim, itemCount));
        
        // Запускаем
        sim.Run();
        
        // Результаты
        var totalTime = sim.Now - new DateTime(1970, 1, 1);
        
        Console.WriteLine($"\nВремя симуляции: {totalTime}");
        metrics.PrintStationStats(line, totalTime);
        metrics.PrintSummary(totalTime);
        
        // Экспорт
        exporter.ExportAll(line, metrics, totalTime);
    }
}