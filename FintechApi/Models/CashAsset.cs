using System.Text.Json.Serialization;

namespace FintechApi.Models
{
    public class CashAsset : Asset
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
} 