using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintechApi.Data;
using FintechApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace FintechApi.Controllers
{
    [Route("api/users/{userId}/assets")]
    public class AssetsController : BaseController
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/{userId}/assets - Only allow access to own assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetsForUser(int userId)
        {
            var authResult = EnsureUserAuthorization(userId);
            if (authResult != null)
            {
                return authResult;
            }

            var user = await _context.Users.Include(u => u.Assets).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound($"User with id {userId} not found.");
            }
            return Ok(user.Assets);
        }

        // GET: api/users/{userId}/assets/{assetId} - Only allow access to own assets
        [HttpGet("{assetId}")]
        public async Task<ActionResult<Asset>> GetAssetForUser(int userId, int assetId)
        {
            var authResult = EnsureUserAuthorization(userId);
            if (authResult != null)
            {
                return authResult;
            }

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.UserId == userId);
            if (asset == null)
            {
                return NotFound($"Asset with id {assetId} not found for user {userId}.");
            }
            return Ok(asset);
        }

        // POST: api/users/{userId}/assets - Only allow creating assets for own account
        [HttpPost]
        public async Task<ActionResult<Asset>> CreateAssetForUser(int userId, [FromBody] JsonElement assetJson)
        {
            var authResult = EnsureUserAuthorization(userId);
            if (authResult != null)
            {
                return authResult;
            }

            var asset = DeserializeAsset(assetJson);
            asset.UserId = userId;
            
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetAssetForUser), new { userId = userId, assetId = asset.Id }, asset);
        }

        // PUT: api/users/{userId}/assets/{assetId} - Only allow updating own assets
        [HttpPut("{assetId}")]
        public async Task<IActionResult> UpdateAssetForUser(int userId, int assetId, [FromBody] JsonElement assetJson)
        {
            var authResult = EnsureUserAuthorization(userId);
            if (authResult != null)
            {
                return authResult;
            }

            var asset = DeserializeAsset(assetJson);
            if (assetId != asset.Id)
            {
                return BadRequest("Asset ID mismatch");
            }

            asset.UserId = userId;

            var existingAsset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.UserId == userId);
            if (existingAsset == null)
            {
                return NotFound($"Asset with id {assetId} not found for user {userId}.");
            }

            _context.Entry(existingAsset).CurrentValues.SetValues(asset);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Assets.AnyAsync(a => a.Id == assetId && a.UserId == userId))
                {
                    return NotFound($"Asset with id {assetId} not found for user {userId}.");
                }
                throw;
            }
            return NoContent();
        }

        // DELETE: api/users/{userId}/assets/{assetId} - Only allow deleting own assets
        [HttpDelete("{assetId}")]
        public async Task<IActionResult> DeleteAssetForUser(int userId, int assetId)
        {
            var authResult = EnsureUserAuthorization(userId);
            if (authResult != null)
            {
                return authResult;
            }

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.UserId == userId);
            if (asset == null)
            {
                return NotFound($"Asset with id {assetId} not found for user {userId}.");
            }
            
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Helper method to deserialize asset based on type
        private static Asset DeserializeAsset(JsonElement assetJson)
        {
            var assetType = assetJson.GetProperty("assetType").GetString();
            return assetType switch
            {
                "Stock" => assetJson.Deserialize<StockAsset>(),
                "Crypto" => assetJson.Deserialize<CryptoAsset>(),
                "Cash" => assetJson.Deserialize<CashAsset>(),
                _ => throw new ArgumentException($"Invalid asset type: {assetType}")
            };
        }
    }
}
