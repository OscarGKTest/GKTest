namespace OscarGKTest.Models;

public class WebBrowser {
    public BrowserName Name { get; set; }
    public int MajorVersion { get; set; }

    public enum BrowserName
    {
        InternetExplorer,
        Chrome
    }
}
