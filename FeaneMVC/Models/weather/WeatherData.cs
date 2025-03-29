using System.Text.Json.Serialization;

namespace FeaneMVC.Models.weather
{
    public class WeatherData
    {
        [JsonPropertyName("coord")]
        public Coordinates Coord { get; set; }

        [JsonPropertyName("weather")]
        public WeatherInfo[] Weather { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("main")]
        public MainData Main { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("wind")]
        public WindData Wind { get; set; }

        [JsonPropertyName("clouds")]
        public CloudsData Clouds { get; set; }

        [JsonPropertyName("dt")]
        public long Dt { get; set; } // Время в формате Unix

        [JsonPropertyName("sys")]
        public SysData Sys { get; set; }

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cod")]
        public int Cod { get; set; }
    }
}
