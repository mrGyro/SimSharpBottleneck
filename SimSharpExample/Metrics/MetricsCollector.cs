using SimSharpExample.SimulationProcess;

namespace SimSharpExample.Metrics;

public class MetricsCollector
{
    public List<Item> CompletedItems { get; private set; }

    public MetricsCollector()
    {
        CompletedItems = new List<Item>();
    }

    // Record completed item
    public void RecordCompletedItem(Item item)
    {
        if (item.CompletionTime.HasValue)
        {
            CompletedItems.Add(item);
        }
    }

    // Average Lead Time
    public TimeSpan GetAverageLeadTime()
    {
        if (CompletedItems.Count == 0)
            return TimeSpan.Zero;
            
        var totalTicks = CompletedItems
            .Where(i => i.GetLeadTime().HasValue)
            .Sum(i => i.GetLeadTime().Value.Ticks);
            
        return TimeSpan.FromTicks(totalTicks / CompletedItems.Count);
    }

    // Station statistics (detailed)
    public void PrintStationStats(ProductionLine line, TimeSpan totalTime)
    {
        Console.WriteLine("\n=== Detailed Station Statistics ===");
        Console.WriteLine($"{"Station",-12} | {"Processed",-11} | {"Utilization",-12} | {"Avg.Queue",-11} | {"Max.Queue",-12}");
        Console.WriteLine(new string('-', 70));
        
        PrintStationRow(line.CuttingStation, totalTime);
        PrintStationRow(line.AssemblyStation, totalTime);
        PrintStationRow(line.TestingStation, totalTime);
        PrintStationRow(line.PackagingStation, totalTime);
    }
    
    private void PrintStationRow(Station station, TimeSpan totalTime)
    {
        Console.WriteLine($"{station.Name,-12} | {station.ProcessedItems,-11} | {station.GetUtilization(totalTime),10:F2}% | {station.GetAverageQueueLength(),11:F2} | {station.MaxQueueLength,-12}");
    }
    
    // Calculate Throughput (items per hour)
    public double GetThroughput(TimeSpan totalTime)
    {
        if (totalTime.TotalHours == 0)
            return 0;
        return CompletedItems.Count / totalTime.TotalHours;
    }
    
    // Overall statistics
    public void PrintSummary(TimeSpan totalTime)
    {
        Console.WriteLine("\n=== Overall Statistics ===");
        Console.WriteLine($"Simulation time: {totalTime}");
        Console.WriteLine($"Total completed items: {CompletedItems.Count}");
        Console.WriteLine($"Throughput: {GetThroughput(totalTime):F2} items/hour");
        Console.WriteLine($"Average Lead Time: {GetAverageLeadTime()}");
        
        if (CompletedItems.Count > 0)
        {
            Console.WriteLine("\nLead Time by items:");
            foreach (var item in CompletedItems)
            {
                Console.WriteLine($"  Item #{item.Id}: {item.GetLeadTime()}");
            }
        }
    }
}