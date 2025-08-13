using OscarGKTest;
using OscarGKTest.Repositories;

Console.WriteLine("Starting Oscar GK Test");

var repository = new Repository();

var speaker = new Speaker();
var registerResult = speaker.Register(repository, "first-name", "last-name", "email", 0, false, "url", "browser", "csv-certifications", "s-emp", 0, "csv-sess");

if (registerResult.Success)
{
    Console.WriteLine($"Speaker registered successfully, Speaker ID: {registerResult.SpeakerId}");
}
else
{
    Console.WriteLine($"Error while registering speaker: {registerResult.Error}");
}

