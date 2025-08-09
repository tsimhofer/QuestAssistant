using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace QuestAssistant.Services;

public class GoogleCalendarService
{
    private static string[] Scopes = { CalendarService.Scope.CalendarEvents };
    private const string ApplicationName = "QuestAssistant"; // Projektnamen hier verwenden
    private const string CredentialsPath = "credentials.json";
    private const string TokenPath = "token.json"; // Pfad, wo der Token gespeichert wird

    private UserCredential _credential;
    private CalendarService _service;

    public async Task InitializeAsync()
    {
        using (var stream = new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read))
        {
            // Der FileDataStore speichert den Token im angegebenen Verzeichnis
            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user", // Dies ist der User, für den der Token gespeichert wird
                CancellationToken.None,
                new FileDataStore(TokenPath, true)); // Speichert den Token persistent
            Console.WriteLine("Credential saved to: " + TokenPath);
        }

        _service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = _credential,
            ApplicationName = ApplicationName,
        });
    }

    public async Task CreateCalendarEvent(TaskItem task)
    {
        if (_service == null)
        {
            Console.WriteLine("Google Calendar Service nicht initialisiert.");
            return;
        }

        var newEvent = new Event()
        {
            Summary = $"Haus-Quest: {task.Name}", // Angepasst an den Projektnamen
            Description = $"Zugewiesen an: {task.AssignedUser}\nDauer: {task.Duration.TotalMinutes} Minuten\nPunkte: {task.Points}\nBeschreibung: {task.Description}",
            Start = new EventDateTime()
            {
                DateTime = task.EarliestStartTime,
                TimeZone = "Europe/Berlin", // Wichtig: Passende Zeitzone einstellen!
            },
            End = new EventDateTime()
            {
                DateTime = task.LatestEndTime,
                TimeZone = "Europe/Berlin",
            },
        };

        // Hier kannst du die CalendarId deines spezifischen Kalenders angeben
        // "primary" ist der Standardkalender des Benutzers
        string calendarId = "primary"; // Oder eine spezifische Kalender-ID

        try
        {
            var request = _service.Events.Insert(newEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();
            Console.WriteLine($"Ereignis erstellt: {createdEvent.HtmlLink}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Erstellen des Kalendereintrags: {ex.Message}");
        }
    }
}
