namespace OscarGKTest.Models;

public class Speaker
{
	public int Id { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Email { get; set; }
	public int? Exp { get; set; }
	public bool HasBlog { get; set; }
	public string? BlogURL { get; set; }
	public WebBrowser? Browser { get; set; }
	public List<string> Certifications { get; set; } = [];
	public string? Employer { get; set; }
	public int RegistrationFee { get; set; }
	public List<Session> Sessions { get; set; } = [];
}
