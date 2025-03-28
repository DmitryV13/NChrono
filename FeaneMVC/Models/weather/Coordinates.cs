using System.Text.Json.Serialization;

namespace FeaneMVC.Models.weather
{
    public class Coordinates
    {
        [JsonPropertyName("lon")]
        public float Lon { get; set; }

        [JsonPropertyName("lat")]
        public float Lat { get; set; }
    }
}
