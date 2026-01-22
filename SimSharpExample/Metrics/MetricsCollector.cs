using SimSharpExample.SimulationProcess;

namespace SimSharpExample.Metrics;

public class MetricsCollector
{
    public List<Item> CompletedItems { get; private set; }

    public MetricsCollector()
    {
        CompletedItems = new List<Item>();
    }

    // Регистрация завершенной детали
    public void RecordCompletedItem(Item item)
    {
        if (item.CompletionTime.HasValue)
        {
            CompletedItems.Add(item);
        }
    }

    // Средний Lead Time
    public TimeSpan GetAverageLeadTime()
    {
        if (CompletedItems.Count == 0)
            return TimeSpan.Zero;
            
        var totalTicks = CompletedItems
            .Where(i => i.GetLeadTime().HasValue)
            .Sum(i => i.GetLeadTime().Value.Ticks);
            
        return TimeSpan.FromTicks(totalTicks / CompletedItems.Count);
    }

    // Статистика по станциям (расширенная)
    public void PrintStationStats(ProductionLine line, TimeSpan totalTime)
    {
        Console.WriteLine("\n=== Детальная статистика станций ===");
        Console.WriteLine($"{"Станция",-12} | {"Обработано",-11} | {"Утилизация",-12} | {"Ср.Очередь",-11} | {"Макс.Очередь",-12}");
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
    
    // Расчет Throughput (деталей в час)
    public double GetThroughput(TimeSpan totalTime)
    {
        if (totalTime.TotalHours == 0)
            return 0;
        return CompletedItems.Count / totalTime.TotalHours;
    }
    
    // Общая статистика
    public void PrintSummary(TimeSpan totalTime)
    {
        Console.WriteLine("\n=== Общая статистика ===");
        Console.WriteLine($"Время симуляции: {totalTime}");
        Console.WriteLine($"Всего завершено деталей: {CompletedItems.Count}");
        Console.WriteLine($"Throughput: {GetThroughput(totalTime):F2} деталей/час");
        Console.WriteLine($"Средний Lead Time: {GetAverageLeadTime()}");
        
        if (CompletedItems.Count > 0)
        {
            Console.WriteLine("\nLead Time по деталям:");
            foreach (var item in CompletedItems)
            {
                Console.WriteLine($"  Item #{item.Id}: {item.GetLeadTime()}");
            }
        }
    }
}