using OscarGKTest.Models;
using OscarGKTest.Repositories;
using OscarGKTest.Requests;

Console.WriteLine("Starting Oscar GK Test");

// Request object would be built up by the user, perhaps via a web form for example
// Here, we hard-code a request as an example
var request = new RegisterSpeakerRequest
{
    FirstName = "first-name",
    LastName = "last-name",
    Email = "email",
    Experience = 0,
    HasBlog = true,
    BlogUrl = "blog-url",
    Browser = new WebBrowser(),
    Certifications = ["certification"],
    EmployerName = "employer-name",
    Sessions = [ new() { Title = "session-title" } ]
};

// TODO: Tidy this up. Should we add MediatR to do this properly?
// Also, repository would be passed in via dependancy injection.
var registerResult = RegisterSpeakerRequestHandler.RegisterSpeaker(request, new Repository());

if (registerResult.Success)
{
    Console.WriteLine($"Speaker registered successfully, Speaker ID: {registerResult.SpeakerId}");
}
else
{
    Console.WriteLine($"Error while registering speaker: {registerResult.Error}");
}

