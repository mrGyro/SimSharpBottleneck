using System.Text;
using SimSharpExample.SimulationProcess;

namespace SimSharpExample.Metrics;

public class CsvExporter
{
    private readonly string _outputDirectory;

    public CsvExporter(string outputDirectory = "Results")
    {
        _outputDirectory = outputDirectory;
        
        // Create folder if it doesn't exist
        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }
    }

    // Export station utilization
    public void ExportStationUtilization(ProductionLine line, TimeSpan totalTime)
    {
        var filePath = Path.Combine(_outputDirectory, "station_utilization.csv");
        var sb = new StringBuilder();
        
        sb.AppendLine("Station,Utilization,ProcessedItems,AvgQueueLength,MaxQueueLength");
        
        AppendStationRow(sb, line.CuttingStation, totalTime);
        AppendStationRow(sb, line.AssemblyStation, totalTime);
        AppendStationRow(sb, line.TestingStation, totalTime);
        AppendStationRow(sb, line.PackagingStation, totalTime);
        
        File.WriteAllText(filePath, sb.ToString());
        Console.WriteLine($"\n✓ Exported: {filePath}");
    }

    private void AppendStationRow(StringBuilder sb, Station station, TimeSpan totalTime)
    {
        sb.AppendLine($"{station.Name},{station.GetUtilization(totalTime):F2},{station.ProcessedItems},{station.GetAverageQueueLength():F2},{station.MaxQueueLength}");
    }

    // Export Lead Time
    public void ExportLeadTimeDistribution(MetricsCollector metrics)
    {
        var filePath = Path.Combine(_outputDirectory, "lead_time_distribution.csv");
        var sb = new StringBuilder();
        
        sb.AppendLine("ItemId,LeadTimeMinutes");
        
        foreach (var item in metrics.CompletedItems)
        {
            var leadTime = item.GetLeadTime();
            if (leadTime.HasValue)
            {
                sb.AppendLine($"{item.Id},{leadTime.Value.TotalMinutes:F2}");
            }
        }
        
        File.WriteAllText(filePath, sb.ToString());
        Console.WriteLine($"✓ Exported: {filePath}");
    }

    // Export overall statistics
    public void ExportSummaryStats(MetricsCollector metrics, TimeSpan totalTime)
    {
        var filePath = Path.Combine(_outputDirectory, "summary_stats.csv");
        var sb = new StringBuilder();
        
        sb.AppendLine("Metric,Value");
        sb.AppendLine($"TotalSimulationTime,{totalTime.TotalMinutes:F2}");
        sb.AppendLine($"CompletedItems,{metrics.CompletedItems.Count}");
        sb.AppendLine($"Throughput,{metrics.GetThroughput(totalTime):F2}");
        sb.AppendLine($"AverageLeadTime,{metrics.GetAverageLeadTime().TotalMinutes:F2}");
        
        File.WriteAllText(filePath, sb.ToString());
        Console.WriteLine($"✓ Exported: {filePath}");
    }

    // Export all data
    public void ExportAll(ProductionLine line, MetricsCollector metrics, TimeSpan totalTime)
    {
        Console.WriteLine("\n=== Exporting data to CSV ===");
        ExportStationUtilization(line, totalTime);
        ExportLeadTimeDistribution(metrics);
        ExportSummaryStats(metrics, totalTime);
        Console.WriteLine($"\nAll files saved in folder: {Path.GetFullPath(_outputDirectory)}");
    }
}