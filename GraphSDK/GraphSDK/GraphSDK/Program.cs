using Microsoft.Extensions.Configuration;
using System.Globalization;

var appConfig = LoadAppSettings();

if (appConfig == null)
{
    Console.WriteLine("Missing or invalid appsettings.json...exiting");
    return;
}

var appId = appConfig["appId"];
var scopesString = appConfig["scopes"];
var scopes = scopesString.Split(';');

// Initialize Graph client
GraphSDK.GraphHelper.Initialize(appId, scopes, (code, cancellation) => {
    Console.WriteLine(code.Message);
    return Task.FromResult(0);
});

var accessToken = GraphSDK.GraphHelper.GetAccessTokenAsync(scopes).Result;

Console.WriteLine($"Access token: {accessToken}\n");


// Get signed in user
var user = GraphSDK.GraphHelper.GetMeAsync().Result;
Console.WriteLine($"Welcome {user.DisplayName}!\n");

// Check for timezone and date/time formats in mailbox settings
// Use defaults if absent
var userTimeZone = !string.IsNullOrEmpty(user.MailboxSettings?.TimeZone) ?
    user.MailboxSettings?.TimeZone : TimeZoneInfo.Local.StandardName;
var userDateFormat = !string.IsNullOrEmpty(user.MailboxSettings?.DateFormat) ?
    user.MailboxSettings?.DateFormat : CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
var userTimeFormat = !string.IsNullOrEmpty(user.MailboxSettings?.TimeFormat) ?
    user.MailboxSettings?.TimeFormat : CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;


int choice = -1;

while (choice != 0)
{
    Console.WriteLine("Please choose one of the following options:");
    Console.WriteLine("0. Exit");
    Console.WriteLine("1. Display access token");
    Console.WriteLine("2. View this week's calendar");
    Console.WriteLine("3. Add an event");

    try
    {
        choice = int.Parse(Console.ReadLine());
    }
    catch (System.FormatException)
    {
        // Set to invalid value
        choice = -1;
    }

    switch (choice)
    {
        case 0:
            // Exit the program
            Console.WriteLine("Goodbye...");
            break;
        case 1:
            // Display access token
            break;
        case 2:
            ListCalendarEvents(
                userTimeZone,
                $"{userDateFormat} {userTimeFormat}"
            );
            break;
        case 3:
            // Create a new event
            break;
        default:
            Console.WriteLine("Invalid choice! Please try again.");
            break;
    }
}

static IConfigurationRoot LoadAppSettings()
{
    var appConfig = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

    // Check for required settings
    if (string.IsNullOrEmpty(appConfig["appId"]) ||
        string.IsNullOrEmpty(appConfig["scopes"]))
    {
        return null;
    }

    return appConfig;
}

static string FormatDateTimeTimeZone(
    Microsoft.Graph.DateTimeTimeZone value,
    string dateTimeFormat)
{
    // Parse the date/time string from Graph into a DateTime
    var dateTime = DateTime.Parse(value.DateTime);

    return dateTime.ToString(dateTimeFormat);
}

static void ListCalendarEvents(string userTimeZone, string dateTimeFormat)
{
    var events = GraphSDK.GraphHelper
        .GetCurrentWeekCalendarViewAsync(DateTime.Today, userTimeZone)
        .Result;

    Console.WriteLine("Events:");

    foreach (var calendarEvent in events)
    {
        Console.WriteLine($"Subject: {calendarEvent.Subject}");
        Console.WriteLine($"  Organizer: {calendarEvent.Organizer.EmailAddress.Name}");
        Console.WriteLine($"  Start: {FormatDateTimeTimeZone(calendarEvent.Start, dateTimeFormat)}");
        Console.WriteLine($"  End: {FormatDateTimeTimeZone(calendarEvent.End, dateTimeFormat)}");
    }
}