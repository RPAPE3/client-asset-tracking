using System.Text.Json.Serialization;

namespace FintechApi.Models
{
    public class StockAsset : Asset
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }
        [JsonPropertyName("shares")]
        public int Shares { get; set; }
    }
} 