using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FintechApi.DTOs
{
    public class UpdateUserRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<AssetResponse> Assets { get; set; } = new();
    }
} 