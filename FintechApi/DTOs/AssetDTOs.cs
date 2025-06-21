using System.ComponentModel.DataAnnotations;

namespace FintechApi.DTOs
{
    public class CreateAssetRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [Required]
        public string AssetType { get; set; } = string.Empty;

        public string? Currency { get; set; }
        public string? CoinType { get; set; }
        public string? WalletAddress { get; set; }
        public string? Ticker { get; set; }
        public int? Shares { get; set; }
    }

    public class UpdateAssetRequest : CreateAssetRequest
    {
        [Required]
        public int Id { get; set; }
    }

    public class AssetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string AssetType { get; set; } = string.Empty;
        public string? Currency { get; set; }
        public string? CoinType { get; set; }
        public string? WalletAddress { get; set; }
        public string? Ticker { get; set; }
        public int? Shares { get; set; }
    }
} 