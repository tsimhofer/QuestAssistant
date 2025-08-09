namespace QuestAssistant.Models;

// HallOfFameEntry.cs
public class HallOfFameEntry
{
    public string UserName { get; set; }
    public int Points { get; set; }
    public DateTime DateAchieved { get; set; }
    public string Period { get; set; } // "Weekly", "Monthly", "Yearly"
}
