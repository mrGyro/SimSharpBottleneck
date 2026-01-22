using System.Text;

namespace SimSharpExample.Metrics;

public class ComparisonAnalyzer
{
    public class ScenarioResult
    {
        public string Name { get; set; } = "";
        public TimeSpan TotalTime { get; set; }
        public int CompletedItems { get; set; }
        public double Throughput { get; set; }
        public TimeSpan AvgLeadTime { get; set; }
        public Dictionary<string, double> StationUtilization { get; set; } = new();
    }

    private List<ScenarioResult> _results = new();

    public void AddResult(string name, TimeSpan totalTime, MetricsCollector metrics, 
        Dictionary<string, double> utilization)
    {
        _results.Add(new ScenarioResult
        {
            Name = name,
            TotalTime = totalTime,
            CompletedItems = metrics.CompletedItems.Count,
            Throughput = metrics.GetThroughput(totalTime),
            AvgLeadTime = metrics.GetAverageLeadTime(),
            StationUtilization = utilization
        });
    }

    public void PrintComparison()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         COMPARATIVE ANALYSIS OF ALL SCENARIOS                ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

        // Comparison table
        Console.WriteLine("┌────────────────────┬──────────────┬─────────────┬──────────────┬──────────────┐");
        Console.WriteLine("│ Scenario           │ Time (min)   │ Throughput  │ Avg LeadTime │ Assembly Util│");
        Console.WriteLine("├────────────────────┼──────────────┼─────────────┼──────────────┼──────────────┤");

        var baseline = _results.FirstOrDefault(r => r.Name == "Baseline");

        foreach (var result in _results)
        {
            var assemblyUtil = result.StationUtilization.GetValueOrDefault("Assembly", 0);
            
            // Calculate changes relative to baseline
            string throughputChange = "";
            string leadTimeChange = "";
            
            if (baseline != null && result.Name != "Baseline")
            {
                var tpChange = ((result.Throughput - baseline.Throughput) / baseline.Throughput) * 100;
                var ltChange = ((result.AvgLeadTime - baseline.AvgLeadTime).TotalMinutes / baseline.AvgLeadTime.TotalMinutes) * 100;
                
                throughputChange = tpChange >= 0 ? $" (+{tpChange:F1}%)" : $" ({tpChange:F1}%)";
                leadTimeChange = ltChange <= 0 ? $" ({ltChange:F1}%)" : $" (+{ltChange:F1}%)";
            }

            Console.WriteLine($"│ {result.Name,-18} │ {result.TotalTime.TotalMinutes,11:F1} │ {result.Throughput,6:F2}{throughputChange,-6} │ {result.AvgLeadTime.TotalMinutes,7:F1}{leadTimeChange,-6} │ {assemblyUtil,11:F1}% │");
        }

        Console.WriteLine("└────────────────────┴──────────────┴─────────────┴──────────────┴──────────────┘");

        PrintInsights();
    }

    private void PrintInsights()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                      KEY INSIGHTS                            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

        var baseline = _results.FirstOrDefault(r => r.Name == "Baseline");
        var improved = _results.FirstOrDefault(r => r.Name == "ImprovedCutting");
        var bottleneck = _results.FirstOrDefault(r => r.Name == "BottleneckFixed");

        if (baseline != null && improved != null)
        {
            var improvement = ((improved.Throughput - baseline.Throughput) / baseline.Throughput) * 100;
            Console.WriteLine($"❌ PARADOX: Improving non-bottleneck (Cutting):");
            Console.WriteLine($"   → Throughput changed only by {improvement:F2}%");
            Console.WriteLine($"   → Practically NO effect!");
        }

        if (baseline != null && bottleneck != null)
        {
            var improvement = ((bottleneck.Throughput - baseline.Throughput) / baseline.Throughput) * 100;
            var leadTimeImprovement = ((baseline.AvgLeadTime - bottleneck.AvgLeadTime).TotalMinutes / baseline.AvgLeadTime.TotalMinutes) * 100;
            Console.WriteLine($"\n✅ CORRECT: Improving bottleneck (Assembly):");
            Console.WriteLine($"   → Throughput increased by {improvement:F2}%");
            Console.WriteLine($"   → Lead Time decreased by {leadTimeImprovement:F2}%");
            Console.WriteLine($"   → SIGNIFICANT system improvement!");
        }

        Console.WriteLine("\n💡 CONCLUSION: Local optimization ≠ Global optimization");
        Console.WriteLine("   Invest in bottlenecks, not in fast stations!");
    }

    public void ExportComparisonCsv(string outputPath = "Results/comparison.csv")
    {
        var sb = new StringBuilder();
        sb.AppendLine("Scenario,TotalTimeMinutes,Throughput,AvgLeadTimeMinutes,AssemblyUtilization");

        foreach (var result in _results)
        {
            var assemblyUtil = result.StationUtilization.GetValueOrDefault("Assembly", 0);
            sb.AppendLine($"{result.Name},{result.TotalTime.TotalMinutes:F2},{result.Throughput:F2},{result.AvgLeadTime.TotalMinutes:F2},{assemblyUtil:F2}");
        }

        File.WriteAllText(outputPath, sb.ToString());
        Console.WriteLine($"\n✓ Comparison table exported: {outputPath}");
    }
}