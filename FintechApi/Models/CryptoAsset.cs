using System.Text.Json.Serialization;

namespace FintechApi.Models
{
    public class CryptoAsset : Asset
    {
        [JsonPropertyName("coinType")]
        public string CoinType { get; set; }
        [JsonPropertyName("walletAddress")]
        public string WalletAddress { get; set; }
    }
} 