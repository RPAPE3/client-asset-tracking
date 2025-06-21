using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintechApi.Data;
using FintechApi.Models;
using System.Threading.Tasks;

namespace FintechApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/me - Get current user's profile
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Assets)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/users/{id} - Only allow access to own profile
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var authResult = EnsureUserAuthorization(id);
            if (authResult != null)
            {
                return authResult;
            }

            var user = await _context.Users
                .Include(u => u.Assets)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/{id} - Only allow updating own profile
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            var authResult = EnsureUserAuthorization(id);
            if (authResult != null)
            {
                return authResult;
            }

            if (id != user.Id)
            {
                return BadRequest("User ID mismatch");
            }

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(u => u.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        // DELETE: api/users/{id} - Only allow deleting own profile
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var authResult = EnsureUserAuthorization(id);
            if (authResult != null)
            {
                return authResult;
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
