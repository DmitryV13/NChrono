using WebApplication1.Models;

namespace FeaneMVC.Models;

public class UserFilterNotificationList
{
    public Guid Id { get; set; }
    public List<string> Filter { get; set; }
    public Dictionary<string, List<string>> maDictionary = new Dictionary<string, List<string>>();
}