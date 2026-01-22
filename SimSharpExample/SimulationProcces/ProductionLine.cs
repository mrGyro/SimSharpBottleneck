using SimSharp;
using SimSharpExample.Metrics;
using SimSharpExample.Utils;

namespace SimSharpExample.SimulationProcess;

public class ProductionLine
{
    public Station CuttingStation { get; protected set; }
    public Station AssemblyStation { get; protected set; }
    public Station TestingStation { get; protected set; }
    public Station PackagingStation { get; protected set; }

    private MetricsCollector _metrics;

    public ProductionLine(Simulation sim, MetricsCollector metrics)
    {
        _metrics = metrics;

        // Создаем 4 станции с разным временем обработки
        CuttingStation = new Station(sim, "Cutting", capacity: 1, 
        TimeSpan.FromMinutes(8));
        AssemblyStation = new Station(sim, "Assembly", capacity: 1, 
        TimeSpan.FromMinutes(12));
        TestingStation = new Station(sim, "Testing", capacity: 1, 
        TimeSpan.FromMinutes(10));
        PackagingStation = new Station(sim, "Packaging", capacity: 1, 
        TimeSpan.FromMinutes(6));
    }

    // Процесс: деталь проходит все 4 станции
    public IEnumerable<Event> ProcessItem(Simulation sim, Item item)
    {
        // 1. Cutting
        yield return sim.Process(CuttingStation.Process(sim, item));
        
        // 2. Assembly
        yield return sim.Process(AssemblyStation.Process(sim, item));
        
        // 3. Testing
        yield return sim.Process(TestingStation.Process(sim, item));
        
        // 4. Packaging
        yield return sim.Process(PackagingStation.Process(sim, item));
        
        // Деталь завершена!
        item.CompletionTime = sim.Now;

        // Регистрируем в метриках
        _metrics.RecordCompletedItem(item);
    }

    // Генератор деталей
    public IEnumerable<Event> GenerateItems(Simulation sim, int count, bool useStochasticArrival = false, double meanInterarrivalMinutes = 5.0)
    {
        for (int i = 1; i <= count; i++)
        {
            var item = new Item(id: i, creationTime: sim.Now);
            sim.Process(ProcessItem(sim, item));
            
            // Пауза между деталями (детерминированная или стохастическая)
            TimeSpan interarrivalTime;
            if (useStochasticArrival)
            {
                interarrivalTime = RandomDistributions.Exponential(meanInterarrivalMinutes);
            }
            else
            {
                interarrivalTime = TimeSpan.FromMinutes(meanInterarrivalMinutes);
            }
            
            yield return sim.Timeout(interarrivalTime);
        }
    }
}
