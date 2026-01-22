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

        // Create 4 stations with different processing times
        CuttingStation = new Station(sim, "Cutting", capacity: 1, 
        TimeSpan.FromMinutes(8));
        AssemblyStation = new Station(sim, "Assembly", capacity: 1, 
        TimeSpan.FromMinutes(12));
        TestingStation = new Station(sim, "Testing", capacity: 1, 
        TimeSpan.FromMinutes(10));
        PackagingStation = new Station(sim, "Packaging", capacity: 1, 
        TimeSpan.FromMinutes(6));
    }

     // Process: item goes through all 4 stations
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
        
        // Item completed!
        item.CompletionTime = sim.Now;

        // Register in metrics
        _metrics.RecordCompletedItem(item);
    }

    // Items generator
    public IEnumerable<Event> GenerateItems(Simulation sim, int count, bool useStochasticArrival = false, double meanInterarrivalMinutes = 5.0)
    {
        for (int i = 1; i <= count; i++)
        {
            var item = new Item(id: i, creationTime: sim.Now);
            sim.Process(ProcessItem(sim, item));
            
            // Pause between items (deterministic or stochastic)
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
