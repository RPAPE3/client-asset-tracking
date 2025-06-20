using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FintechApi.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; } = new List<Asset>();
        [JsonIgnore]
        public string PasswordHash { get; set; }
    }
} 