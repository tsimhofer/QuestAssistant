
// TaskItem.cs
public class TaskItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Frequency { get; set; } // z.B. "Daily", "Weekly", "Monthly"
    public TimeSpan Duration { get; set; }
    public int Points { get; set; }
    public DateTime EarliestStartTime { get; set; }
    public DateTime LatestEndTime { get; set; }
    public string Status { get; set; } // z.B. "Pending", "Completed"
    public string AssignedUser { get; set; }
}