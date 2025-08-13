namespace OscarGKTest.Models;

public class WebBrowser {
    public BrowserName Name { get; }
    public int MajorVersion { get; }

    public enum BrowserName
    {
        InternetExplorer
    }
}
