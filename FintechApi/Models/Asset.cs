using System.Text.Json.Serialization;

namespace FintechApi.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "assetType")]
    [JsonDerivedType(typeof(StockAsset), "Stock")]
    [JsonDerivedType(typeof(CryptoAsset), "Crypto")]
    [JsonDerivedType(typeof(CashAsset), "Cash")]
    public abstract class Asset
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
} 