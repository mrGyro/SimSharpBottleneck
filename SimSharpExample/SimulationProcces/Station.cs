using SimSharp;
using SimSharpExample.Utils;

namespace SimSharpExample.SimulationProcess;

public class Station
{
    public string Name { get; private set; }
    public Resource MachineResource { get; private set; }
    public TimeSpan MeanProcessingTime { get; private set; }
    public bool UseStochastic { get; set; } = false;
    public double StdDevPercentage { get; set; } = 0.2; // 20% standard deviation

    // For statistics
    public int ProcessedItems { get; private set; } = 0;
    public TimeSpan TotalBusyTime { get; private set; } = TimeSpan.Zero;
    public int MaxQueueLength { get; private set; } = 0;
    
    // For tracking queues
    private List<int> _queueLengthSamples = new List<int>();

    public Station(Simulation sim, string name, int capacity, TimeSpan meanProcessingTime, bool useStochastic = false)
    {
        Name = name;
        MachineResource = new Resource(sim, capacity);
        MeanProcessingTime = meanProcessingTime;
        ProcessedItems = 0;
        UseStochastic = useStochastic;
    }

    // Item processing on the machine
    public IEnumerable<Event> Process(SimSharp.Simulation sim, Item item)
    {
        // Record queue size (InUse = busy resources)
        int queueLength = MachineResource.InUse;
        _queueLengthSamples.Add(queueLength);
        if (queueLength > MaxQueueLength)
            MaxQueueLength = queueLength;

        // Request machine usage
        var request = MachineResource.Request();
        yield return request; // Wait for machine to become available

        // Record start time of processing
        var startTime = sim.Now;

        // Calculate processing time (deterministic or stochastic)
        TimeSpan processingTime;
        if (UseStochastic)
        {
            // Use normal distribution with standard deviation
            double stdDev = MeanProcessingTime.TotalMinutes * StdDevPercentage;
            processingTime = RandomDistributions.Normal(MeanProcessingTime.TotalMinutes, stdDev);
        }
        else
        {
            processingTime = MeanProcessingTime;
        }

        // Process item
        yield return sim.Timeout(processingTime);

        // Calculate busy time
        TotalBusyTime += (sim.Now - startTime);

        // Release machine
        MachineResource.Release(request);

        // Update statistics
        ProcessedItems++;
    }

    // Get average queue length
    public double GetAverageQueueLength()
    {
        if (_queueLengthSamples.Count == 0)
            return 0;
        return _queueLengthSamples.Average();
    }

    // Calculate utilization (in percentage)
    public double GetUtilization(TimeSpan totalTime)
    {
        if (totalTime == TimeSpan.Zero)
            return 0;
        return (TotalBusyTime.TotalMinutes / totalTime.TotalMinutes) * 100;
    }
}