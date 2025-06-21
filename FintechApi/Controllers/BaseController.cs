using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FintechApi.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current user ID from the JWT token
        /// </summary>
        /// <returns>User ID if found, null otherwise</returns>
        protected int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Checks if the current user is authorized to access the specified user's data
        /// </summary>
        /// <param name="requestedUserId">The user ID being requested</param>
        /// <returns>True if authorized, false otherwise</returns>
        protected bool IsAuthorizedForUser(int requestedUserId)
        {
            var currentUserId = GetCurrentUserId();
            return currentUserId.HasValue && currentUserId.Value == requestedUserId;
        }

        /// <summary>
        /// Ensures the current user is authorized to access the specified user's data
        /// </summary>
        /// <param name="requestedUserId">The user ID being requested</param>
        /// <returns>UnauthorizedResult if not authenticated, ForbidResult if not authorized, null if authorized</returns>
        protected ActionResult? EnsureUserAuthorization(int requestedUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (requestedUserId != currentUserId.Value)
            {
                return Forbid();
            }

            return null; // Authorized
        }
    }
} 