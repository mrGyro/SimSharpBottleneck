namespace SimSharpExample.SimulationProcess;

public class Item
{
    public int Id { get; private set; }
    public DateTime CreationTime { get; private set; }
    public DateTime? CompletionTime { get; set; }

    public Item(int id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public TimeSpan? GetLeadTime(){
        if(CompletionTime.HasValue){
            return CompletionTime.Value - CreationTime;
        }
        return null;
    }
}