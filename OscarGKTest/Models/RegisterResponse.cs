using OscarGKTest.Models.Enums;

namespace OscarGKTest.Models;

public class RegisterResponse
{
    public bool Success { get; }
    public int? SpeakerId { get; }
    public RegisterError? Error { get; }

    public RegisterResponse(RegisterError error)
    {
        Success = false;
        Error = error;
    }

    public RegisterResponse(int speakerId)
    {
        Success = true;
        SpeakerId = speakerId;
    }
}
