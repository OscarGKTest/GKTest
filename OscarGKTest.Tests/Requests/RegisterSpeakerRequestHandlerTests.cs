using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
        Repository = Substitute.For<IRepository>();
    }

    private static RegisterSpeakerRequest CreateValidRequest()
    {
        return new RegisterSpeakerRequest
        {
            FirstName = "first-name",
            LastName = "last-name",
            Email = "email",
            Experience = 0,
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
    public void RegisterSpeakerRequestHandler_ValidRequest_SavesCorrectInformation()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        // TODO: Assert too large, not ideal. Better to check these things individually?
        Repository.Received(1).SaveSpeaker(Arg.Is<Speaker>(s =>
            s.FirstName == request.FirstName &&
            s.LastName == request.LastName &&
            s.Email == request.Email &&
            s.Exp == request.Experience &&
            s.HasBlog == request.HasBlog &&
            s.BlogURL == request.BlogUrl &&
            s.Browser == request.Browser &&
            s.Certifications.SequenceEqual(request.Certifications) &&
            s.Employer == request.EmployerName &&
            s.RegistrationFee == 500 &&
            s.Sessions.Count == request.Sessions.Count &&
            s.Sessions.All(session => session.Approved)
        ));
    }

    // The below four tests that check the returned fee are somewhat flawed
    // They cover each possible outcome of the switch statement, but don't cover the full ranges of the each condition
    // Which we would need to do for full code coverage.
    [Fact]
    public void RegisterSpeakerRequestHandler_TwoYearsExperience_Returns250Fee()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Experience = 2;
        var expectedFee = 250;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Repository.Received(1).SaveSpeaker(Arg.Is<Speaker>(s => s.RegistrationFee == expectedFee));
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_FourYearsExperience_Returns100Fee()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Experience = 4;
        var expectedFee = 100;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Repository.Received(1).SaveSpeaker(Arg.Is<Speaker>(s => s.RegistrationFee == expectedFee));
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_SixYearsExperience_Returns50Fee()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Experience = 6;
        var expectedFee = 50;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Repository.Received(1).SaveSpeaker(Arg.Is<Speaker>(s => s.RegistrationFee == expectedFee));
    }

    [Fact]
    public void RegisterSpeakerRequestHandler_TenYearsExperience_ReturnsZeroFee()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Experience = 10;
        var expectedFee = 0;

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Repository.Received(1).SaveSpeaker(Arg.Is<Speaker>(s => s.RegistrationFee == expectedFee));
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

    [Fact]
    public void RegisterSpeakerRequestHandler_SaveSpeakerThrows_ReturnsDatabaseFailureError()
    {
        // Arrange
        var request = CreateValidRequest();
        Repository
            .SaveSpeaker(Arg.Any<Speaker>())
            .Throws(new Exception());

        // Act
        var result = RegisterSpeakerRequestHandler.RegisterSpeaker(request, Repository);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RegisterError.DatabaseFailure, result.Error);
    }
}
