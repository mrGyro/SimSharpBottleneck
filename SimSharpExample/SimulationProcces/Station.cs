using SimSharp;
using SimSharpExample.Utils;

namespace SimSharpExample.SimulationProcess;

public class Station
{
    public string Name { get; private set; }
    public Resource MachineResource { get; private set; }
    public TimeSpan MeanProcessingTime { get; private set; }
    public bool UseStochastic { get; set; } = false;
    public double StdDevPercentage { get; set; } = 0.2; // 20% стандартное отклонение

    // Для статистики
    public int ProcessedItems { get; private set; } = 0;
    public TimeSpan TotalBusyTime { get; private set; } = TimeSpan.Zero;
    public int MaxQueueLength { get; private set; } = 0;
    
    // Для отслеживания очередей
    private List<int> _queueLengthSamples = new List<int>();

    public Station(Simulation sim, string name, int capacity, TimeSpan meanProcessingTime, bool useStochastic = false)
    {
        Name = name;
        MachineResource = new Resource(sim, capacity);
        MeanProcessingTime = meanProcessingTime;
        ProcessedItems = 0;
        UseStochastic = useStochastic;
    }

    // Процесс обработки детали на станке
    public IEnumerable<Event> Process(SimSharp.Simulation sim, Item item)
    {
        // Записываем размер очереди (InUse = занятые ресурсы, Waiting в очереди)
        int queueLength = MachineResource.InUse;
        _queueLengthSamples.Add(queueLength);
        if (queueLength > MaxQueueLength)
            MaxQueueLength = queueLength;

        // Запрос на использование станка
        var request = MachineResource.Request();
        yield return request; // Ожидание освобождения станка

        // Засекаем время начала обработки
        var startTime = sim.Now;

        // Вычисляем время обработки (детерминированное или стохастическое)
        TimeSpan processingTime;
        if (UseStochastic)
        {
            // Используем нормальное распределение со стандартным отклонением
            double stdDev = MeanProcessingTime.TotalMinutes * StdDevPercentage;
            processingTime = RandomDistributions.Normal(MeanProcessingTime.TotalMinutes, stdDev);
        }
        else
        {
            processingTime = MeanProcessingTime;
        }

        // Обработка детали
        yield return sim.Timeout(processingTime);

        // Рассчитываем время занятости
        TotalBusyTime += (sim.Now - startTime);

        // Освобождение станка
        MachineResource.Release(request);

        // Обновление статистики
        ProcessedItems++;
    }

    // Получить среднюю длину очереди
    public double GetAverageQueueLength()
    {
        if (_queueLengthSamples.Count == 0)
            return 0;
        return _queueLengthSamples.Average();
    }

    // Рассчитать утилизацию (в процентах)
    public double GetUtilization(TimeSpan totalTime)
    {
        if (totalTime == TimeSpan.Zero)
            return 0;
        return (TotalBusyTime.TotalMinutes / totalTime.TotalMinutes) * 100;
    }
}