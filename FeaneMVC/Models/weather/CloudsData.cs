using System.Text.Json.Serialization;

namespace FeaneMVC.Models.weather
{
    public class CloudsData
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }
}
