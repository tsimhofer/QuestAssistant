namespace QuestAssistant;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("QuestAssistant Add-on startet..."); // Name angepasst

        // Sicherstellen, dass die credentials.json existiert
        if (!File.Exists("credentials.json"))
        {
            Console.WriteLine("Fehler: 'credentials.json' nicht gefunden. Bitte herunterladen und ins Stammverzeichnis kopieren.");
            Console.WriteLine("Das Add-on kann ohne Google Calendar Credentials nicht gestartet werden.");
            return;
        }

        var calendarService = new GoogleCalendarService();
        await calendarService.InitializeAsync(); // Führt den OAuth-Flow aus

        var taskManager = new TaskManager(calendarService);

        // Beispiel: User hinzufügen (initial oder über eine Admin-Oberfläche)
        taskManager.AddUser("Alice");
        taskManager.AddUser("Bob");

        // Beispiel: Aufgaben zuweisen und Kalendereinträge erstellen
        taskManager.AssignTasks();

        // Simuliere, dass Alice eine Aufgabe erledigt
        // Hier müsstest du eine Möglichkeit schaffen, diese Interaktion aus Home Assistant auszulösen
        taskManager.CompleteTask("Wohnzimmer saugen", "Alice");

        // Ruhmeshalle anzeigen
        var weeklyHallOfFame = taskManager.GetWeeklyHallOfFame();
        Console.WriteLine("\n--- Wöchentliche Ruhmeshalle ---");
        foreach (var entry in weeklyHallOfFame)
        {
            Console.WriteLine($"{entry.UserName}: {entry.Points} Punkte ({entry.Period})");
        }


        Console.WriteLine("\nQuestAssistant läuft. Drücke eine beliebige Taste zum Beenden."); // Name angepasst
        Console.ReadKey();
    }
}
