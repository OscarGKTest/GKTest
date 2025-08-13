using OscarGKTest.Models;

namespace OscarGKTest.Repositories;

public class Repository : IRepository
{
    public int? SaveSpeaker(Speaker speaker)
    {
        // Repository layer - acting as a bridge between our business logic and database interactions.
        // For example, in here, let's now save the speaker to the DB, something like:
        // context.Speakers.Add(speaker);
        // context.SaveChangesAsync(cancelationToken);

        return 1;
    }
}
