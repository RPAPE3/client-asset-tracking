using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintechApi.Data;
using FintechApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;

namespace FintechApi.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/{userId}/assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetsForUser(int userId)
        {
            var user = await _context.Users.Include(u => u.Assets).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound($"User with id {userId} not found.");
            }
            return Ok(user.Assets);
        }

        // GET: api/users/{userId}/assets/{assetId}
        [HttpGet("{assetId}")]
        public async Task<ActionResult<Asset>> GetAssetForUser(int userId, int assetId)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.UserId == userId);
            if (asset == null)
            {
                return NotFound();
            }
            return asset;
        }



        // POST: api/users/{userId}/assets
        [HttpPost]
        public async Task<ActionResult<Asset>> CreateAssetForUser(int userId, [FromBody] JsonElement assetJson)
        {
            var assetType = assetJson.GetProperty("assetType").GetString();
            Asset asset = assetType switch
            {
                "Stock" => assetJson.Deserialize<StockAsset>(),
                "Crypto" => assetJson.Deserialize<CryptoAsset>(),
                "Cash" => assetJson.Deserialize<CashAsset>(),
                _ => throw new ArgumentException("Invalid assetType")
            };

            // Debug output
            Console.WriteLine($"DEBUG: assetType={assetType}, Name={assetJson.GetProperty("name").GetString()}");

            asset.UserId = userId;
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAssetForUser), new { userId = userId, assetId = asset.Id }, asset);
        }

        // PUT: api/users/{userId}/assets/{assetId}
        [HttpPut("{assetId}")]
        public async Task<IActionResult> UpdateAssetForUser(int userId, int assetId, [FromBody] JsonElement assetJson)
        {
            var assetType = assetJson.GetProperty("assetType").GetString();
            Asset asset = assetType switch
            {
                "Stock" => assetJson.Deserialize<StockAsset>(),
                "Crypto" => assetJson.Deserialize<CryptoAsset>(),
                "Cash" => assetJson.Deserialize<CashAsset>(),
                _ => throw new ArgumentException("Invalid assetType")
            };

            if (assetId != asset.Id)
            {
                return BadRequest();
            }

            // Always set the userId from the route
            asset.UserId = userId;

            var exists = await _context.Assets.AnyAsync(a => a.Id == assetId && a.UserId == userId);
            if (!exists)
            {
                return NotFound();
            }

            _context.Entry(asset).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Assets.AnyAsync(a => a.Id == assetId && a.UserId == userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/users/{userId}/assets/{assetId}
        [HttpDelete("{assetId}")]
        public async Task<IActionResult> DeleteAssetForUser(int userId, int assetId)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.UserId == userId);
            if (asset == null)
            {
                return NotFound();
            }
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
