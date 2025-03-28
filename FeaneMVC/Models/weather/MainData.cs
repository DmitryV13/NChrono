using System.Text.Json.Serialization;

namespace FeaneMVC.Models.weather
{
    public class MainData
    {
        [JsonPropertyName("temp")]
        public float Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public float FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public float TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public float TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }
}
