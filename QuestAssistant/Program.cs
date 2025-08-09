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
        await calendarService.InitializeAsync(); // F�hrt den OAuth-Flow aus

        var taskManager = new TaskManager(calendarService);

        // Beispiel: User hinzuf�gen (initial oder �ber eine Admin-Oberfl�che)
        taskManager.AddUser("Alice");
        taskManager.AddUser("Bob");

        // Beispiel: Aufgaben zuweisen und Kalendereintr�ge erstellen
        taskManager.AssignTasks();

        // Simuliere, dass Alice eine Aufgabe erledigt
        // Hier m�sstest du eine M�glichkeit schaffen, diese Interaktion aus Home Assistant auszul�sen
        taskManager.CompleteTask("Wohnzimmer saugen", "Alice");

        // Ruhmeshalle anzeigen
        var weeklyHallOfFame = taskManager.GetWeeklyHallOfFame();
        Console.WriteLine("\n--- W�chentliche Ruhmeshalle ---");
        foreach (var entry in weeklyHallOfFame)
        {
            Console.WriteLine($"{entry.UserName}: {entry.Points} Punkte ({entry.Period})");
        }


        Console.WriteLine("\nQuestAssistant l�uft. Dr�cke eine beliebige Taste zum Beenden."); // Name angepasst
        Console.ReadKey();
    }
}
