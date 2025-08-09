using QuestAssistant.Models;
using QuestAssistant.Services;
using System.Text.Json;

namespace QuestAssistant.Logic;

public class TaskManager
{
    private List<TaskItem> _tasks;
    private List<User> _users;
    private GoogleCalendarService _calendarService;
    private const string UserDataFilePath = "users.json";
    private const string TaskDataFilePath = "tasks_current_status.json"; // Für den Status

    public TaskManager(GoogleCalendarService calendarService)
    {
        _calendarService = calendarService;
        LoadUserData();
        LoadTaskStatusData();
    }

    private void LoadUserData()
    {
        if (File.Exists(UserDataFilePath))
        {
            var json = File.ReadAllText(UserDataFilePath);
            _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        else
        {
            _users = new List<User>(); // Annahme: Nutzer werden manuell hinzugefügt oder aus Excel gelesen
            Console.WriteLine("Keine User-Daten gefunden, initialisiere neue Liste.");
        }
    }

    private void SaveUserData()
    {
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(UserDataFilePath, json);
        Console.WriteLine("User-Daten gespeichert.");
    }

    private void LoadTaskStatusData()
    {
        if (File.Exists(TaskDataFilePath))
        {
            var json = File.ReadAllText(TaskDataFilePath);
            _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
        }
        else
        {
            // Beim ersten Start Aufgaben aus Excel laden
            _tasks = ExcelDataReader.ReadTasks("tasks.xlsx");
            Console.WriteLine("Keine Aufgabenstatus-Daten gefunden, lade aus Excel.");
        }
    }

    private void SaveTaskStatusData()
    {
        var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(TaskDataFilePath, json);
        Console.WriteLine("Aufgabenstatus-Daten gespeichert.");
    }

    public void AssignTasks()
    {
        Console.WriteLine("Prüfe und weise Aufgaben zu...");
        foreach (var task in _tasks.Where(t => t.Status == "Pending" && t.EarliestStartTime <= DateTime.Now))
        {
            if (string.IsNullOrEmpty(task.AssignedUser))
            {
                // Einfache Zuweisungslogik: Zuweisen zu einem beliebigen User
                // Hier könnte komplexere Logik stehen (z.B. Rotation, Least-Assigned-User)
                var userToAssign = _users.FirstOrDefault();
                if (userToAssign != null)
                {
                    task.AssignedUser = userToAssign.Name;
                    Console.WriteLine($"Quest '{task.Name}' zugewiesen an '{task.AssignedUser}'."); // Angepasst
                }
            }
            // Kalendereintrag erstellen, wenn noch nicht geschehen
            _calendarService.CreateCalendarEvent(task).Wait(); // Blockierend für Einfachheit, besser async
        }
        SaveTaskStatusData();
    }

    public void CompleteTask(string taskName, string userName)
    {
        var task = _tasks.FirstOrDefault(t => t.Name == taskName && t.AssignedUser == userName && t.Status == "Pending");
        if (task != null)
        {
            task.Status = "Completed";
            var user = _users.FirstOrDefault(u => u.Name == userName);
            if (user != null)
            {
                user.CurrentPoints += task.Points;
                Console.WriteLine($"Quest '{task.Name}' von '{userName}' erledigt! {task.Points} Punkte gutgeschrieben. Total: {user.CurrentPoints}"); // Angepasst
            }
            SaveUserData();
            SaveTaskStatusData();
        }
        else
        {
            Console.WriteLine($"Quest '{taskName}' für '{userName}' nicht gefunden oder bereits erledigt."); // Angepasst
        }
    }
    // ... Logik für Ruhmeshalle (Weekly, Monthly, Yearly)
    public List<HallOfFameEntry> GetWeeklyHallOfFame()
    {
        // Annahme: Hier werden Aufgabenlog-Einträge verwendet, nicht nur aktuelle Punkte
        // Für diese Anleitung nehmen wir an, die Punkte werden weekly/monthly zurückgesetzt
        // In einer echten Anwendung: Logge abgeschlossene Aufgaben mit Datum und User
        return _users.OrderByDescending(u => u.CurrentPoints).Take(3)
                     .Select(u => new HallOfFameEntry { UserName = u.Name, Points = u.CurrentPoints, DateAchieved = DateTime.Now, Period = "Weekly" }).ToList();
    }

    public void AddUser(string userName)
    {
        if (!_users.Any(u => u.Name == userName))
        {
            _users.Add(new User { Name = userName, CurrentPoints = 0 });
            SaveUserData();
            Console.WriteLine($"Abenteurer '{userName}' hinzugefügt."); // Angepasst
        }
        else
        {
            Console.WriteLine($"Abenteurer '{userName}' existiert bereits."); // Angepasst
        }
    }

    public List<User> GetUsers() => _users;
}

