using System.Text.Json.Serialization;

namespace FeaneMVC.Models.weather
{
    public class WindData
    {
        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }
}
