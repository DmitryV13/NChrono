namespace FeaneMVC.Models;

public class Notification
{
    public string TimeString { get; set; }
    public string AppName { get; set; }
    public string Summary { get; set; }
    public string Prefix { get; set; }
    public string Body { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj is Notification other)
        {
            return TimeString == other.TimeString &&
                   AppName == other.AppName &&
                   Summary == other.Summary &&
                   Prefix == other.Prefix &&
                   Body == other.Body;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TimeString, AppName, Summary, Prefix, Body);
    }
}