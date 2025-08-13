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
    public WebBrowser? Browser { get; set; } // Concession - assume another part of the program converts string browserName to WebBrowser - to re-evaluate
    public List<string> Certifications { get; set; } = []; // Concession - assume another part of the program converts csv string to List<string> - to re-evaluate
    public string? EmployerName { get; set; }
    public int Fee { get; set; }
    public List<Session> Sessions { get; set; } = []; // Concession - assume another part of the program converts csv string to List<Session> - to re-evaluate
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
            _             => 50,
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

        // TODO: re-write the rest of the method

        int? speakerId = null;
        try
        {
            speakerId = repository.SaveSpeaker(speaker);
        }
        catch (Exception)
        {
            //in case the db call fails 
        }

		//if we got this far, the speaker is registered.
		return new RegisterSpeakerResponse((int)speakerId);
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

        List<string> employerNames = [ "Pluralsight", "Microsoft", "Google" ];

        var speakerMeetsStandards = request.Experience > 10 || request.HasBlog || request.Certifications.Count > 3 || employerNames.Contains(request.EmployerName);

        if (!speakerMeetsStandards)
        {
            List<string> domainBlockList = [ "aol.com", "prodigy.com", "compuserve.com" ];

            var emailDomain = request.Email.Split('@').Last();

            var isEmailDomainBlocked = domainBlockList.Contains(emailDomain);
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

		//List<string> newTech = [ "Node.js", "Docker" ];
		List<string> oldTech = [ "Cobol", "Punch Cards", "Commodore", "VBScript" ];

        foreach (var session in request.Sessions)
        {
            //foreach (var tech in newTech)
            //{
            //    if (session.Title.Contains(tech))
            //    {
            //        session.Approved = true;
            //        break;
            //    }
            //}

            session.Approved = !oldTech.Any(tech => session.Title?.Contains(tech) == true || session.Description?.Contains(tech) == true);
        }

        if (!request.Sessions.Any(x => x.Approved))
        {
            return RegisterError.NoSessionsApproved;
        }

        return null;
    }
}
