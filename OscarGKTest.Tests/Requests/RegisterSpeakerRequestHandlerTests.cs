using OscarGKTest.Models;
using OscarGKTest.Models.Enums;
using OscarGKTest.Repositories;
using OscarGKTest.Requests;

namespace OscarGKTest.Tests.Requests;

public class RegisterSpeakerRequestHandlerTests
{
    private readonly IRepository Repository;

    public RegisterSpeakerRequestHandlerTests()
    {
        Repository = new Repository();
    }

    private static RegisterSpeakerRequest CreateValidRequest()
    {
        return new RegisterSpeakerRequest
        {
            FirstName = "first-name",
            LastName = "last-name",
            Email = "email",
            Experience = 1,
            HasBlog = true,
            BlogUrl = "blog-url",
            Browser = new WebBrowser { Name = WebBrowser.BrowserName.Chrome, MajorVersion = 1 },
            Certifications = ["cert-1", "cert-2", "cert-3", "cert-4"],
            EmployerName = "employer-name",
            Sessions =
            [
                new Session { Title = "session-title", Description = "session-description" }
            ]
        };
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_ValidRequest_ReturnsSpeakerId()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.SpeakerId);
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_EmptyFirstName_ReturnsFirstNameRequiredError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.FirstName = string.Empty;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.FirstNameRequired, result.Error);
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_EmptyLastName_ReturnsLastNameRequiredError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.LastName = string.Empty;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.LastNameRequired, result.Error);
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_EmptyEmail_ReturnsEmailRequiredError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Email = string.Empty;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.EmailRequired, result.Error);
    }

    // TODO: I'm not 100% happy with this test.
    // Ideally, I'd like to check that moving each of these request paramaters, eg, HasBlog, to the valid state individually, results in a success
    [Fact]
    public void RegisterSpeakerRequestHandler_DoesNotMeetStandards_ReturnsSpeakerDoesNotMeetStandardsError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Experience = 0;
        request.HasBlog = false;
        request.Certifications.Clear();
        request.Email = "blocked@AoL.com";
        request.Browser = new WebBrowser { Name = WebBrowser.BrowserName.InternetExplorer, MajorVersion = 1 };

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.SpeakerDoesNotMeetStandards, result.Error);
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_HasNoSessions_ReturnsNoSessionsProvidedError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Sessions.Clear();

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.NoSessionsProvided, result.Error);
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_NoValidSessions_ReturnsNoSessionsApprovedError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Sessions = [new Session { Title = "invalid-title-example-cObOl" }];

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.NoSessionsApproved, result.Error);
    }
}
