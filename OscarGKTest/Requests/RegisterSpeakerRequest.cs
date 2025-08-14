using OscarGKTest.Models;
using OscarGKTest.Models.Enums;
using OscarGKTest.Repositories;

namespace OscarGKTest.Requests;

public class RegisterSpeakerRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public int Experience { get; set; }
    public bool HasBlog { get; set; }
    public string? BlogUrl { get; set; }
    public WebBrowser? Browser { get; set; } // Concession - assume another part of the program converts string browserName to WebBrowser
    public List<string> Certifications { get; set; } = []; // Concession - assume another part of the program converts csv string to List<string>
    public string? EmployerName { get; set; }
    public int Fee { get; set; }
    public List<Session> Sessions { get; set; } = []; // Concession - assume another part of the program converts csv string to List<Session>
}

public class RegisterSpeakerResponse
{
    public bool Success { get; }
    public int? SpeakerId { get; }
    public RegisterError? Error { get; }

    public RegisterSpeakerResponse(RegisterError error)
    {
        Success = false;
        Error = error;
    }

    public RegisterSpeakerResponse(int speakerId)
    {
        Success = true;
        SpeakerId = speakerId;
    }
}

// Concession - I have made this static for simplcity - I would potentially use MediatiR and make this a request handler
public static class RegisterSpeakerRequestHandler
{
    public static RegisterSpeakerResponse RegisterSpeaker(RegisterSpeakerRequest request, IRepository repository)
    {
        var validationError = ValidateRequest(request);

        if (validationError != null)
        {
            return new RegisterSpeakerResponse(validationError.Value);
        }

        int registrationFee = request.Experience switch
        {
            <= 1          => 500,
            >= 2 and <= 3 => 250,
            >= 4 and <= 5 => 100,
            >= 6 and <= 9 => 50,
            _             => 0,
        };

        var speaker = new Speaker
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Exp = request.Experience,
            HasBlog = request.HasBlog,
            BlogURL = request.BlogUrl,
            Browser = request.Browser,
            Certifications = request.Certifications,
            Employer = request.EmployerName,
            RegistrationFee = registrationFee,
            Sessions = request.Sessions
        };

        try
        {
            var speakerId = repository.SaveSpeaker(speaker);
            return new RegisterSpeakerResponse(speakerId);
        }
        catch (Exception ex)
        {
            // Make sure we log the expection using the project's logger, for example:
            // _logger.LogError(ex, "Error while saving speaker");

            return new RegisterSpeakerResponse(RegisterError.DatabaseFailure);
        }
    }

    private static RegisterError? ValidateRequest(RegisterSpeakerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return RegisterError.FirstNameRequired;
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return RegisterError.LastNameRequired;
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return RegisterError.EmailRequired;
        }

        List<string> validEmployerNames = ["Pluralsight", "Microsoft", "Google"];

        var employerNameInValidList = validEmployerNames.Any(x => string.Equals(x, request.EmployerName, StringComparison.OrdinalIgnoreCase));

        var speakerMeetsStandards = request.Experience > 10 || request.HasBlog || request.Certifications.Count > 3 || employerNameInValidList;

        if (!speakerMeetsStandards)
        {
            List<string> domainBlockList = ["aol.com", "prodigy.com", "compuserve.com"];

            var emailDomain = request.Email.Split('@').Last();

            var isEmailDomainBlocked = domainBlockList.Any(x => string.Equals(x, emailDomain, StringComparison.OrdinalIgnoreCase));
            var isBrowserBlocked = request.Browser?.Name == WebBrowser.BrowserName.InternetExplorer && request.Browser?.MajorVersion < 9;

            if (isEmailDomainBlocked || isBrowserBlocked)
            {
                return RegisterError.SpeakerDoesNotMeetStandards;
            }
        }

        if (request.Sessions.Count == 0)
        {
            return RegisterError.NoSessionsProvided;
        }

        // Removed the newTech code that was commented out
        // Made the assumption that the newTech check was an old business requirement
        // Perhaps we used to only allow talks about new tech, but now, we allow any talks as long as it's not about the below old techs
        foreach (var session in request.Sessions)
        {
            var sessionContainsOldTech = StringContainsOldTech(session.Title ?? "") || StringContainsOldTech(session.Description ?? "");

            session.Approved = !sessionContainsOldTech;
        }

        // Assumed that you only need one session to be approved in order to register
        if (!request.Sessions.Any(x => x.Approved))
        {
            return RegisterError.NoSessionsApproved;
        }

        return null;
    }

    private static bool StringContainsOldTech(string input)
    {
        List<string> oldTech = ["Cobol", "Punch Cards", "Commodore", "VBScript"];

        return oldTech.Any(tech => input.Contains(tech, StringComparison.OrdinalIgnoreCase));
    }
}
