namespace SimSharpExample.Utils;

public static class RandomDistributions
{
    private static Random _random = new Random();

    // Exponential distribution (for arrival intervals)
    public static TimeSpan Exponential(double meanMinutes)
    {
        double u = _random.NextDouble();
        double minutes = -meanMinutes * Math.Log(1 - u);
        return TimeSpan.FromMinutes(minutes);
    }

    // Normal distribution (for processing time)
    public static TimeSpan Normal(double meanMinutes, double stdDevMinutes)
    {
        // Box-Muller transform
        double u1 = 1.0 - _random.NextDouble();
        double u2 = 1.0 - _random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double minutes = meanMinutes + stdDevMinutes * randStdNormal;
        
        // Don't allow negative values
        minutes = Math.Max(0.1, minutes);
        
        return TimeSpan.FromMinutes(minutes);
    }

    // Triangular distribution (min, mode, max)
    public static TimeSpan Triangular(double minMinutes, double modeMinutes, double maxMinutes)
    {
        double u = _random.NextDouble();
        double f = (modeMinutes - minMinutes) / (maxMinutes - minMinutes);
        
        double minutes;
        if (u < f)
        {
            minutes = minMinutes + Math.Sqrt(u * (maxMinutes - minMinutes) * (modeMinutes - minMinutes));
        }
        else
        {
            minutes = maxMinutes - Math.Sqrt((1 - u) * (maxMinutes - minMinutes) * (maxMinutes - modeMinutes));
        }
        
        return TimeSpan.FromMinutes(minutes);
    }

    // Set seed for reproducible results
    public static void SetSeed(int seed)
    {
        _random = new Random(seed);
    }
}