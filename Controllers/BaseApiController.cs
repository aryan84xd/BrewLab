using BrewLab.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected readonly AppDbContext _db;

    protected BaseApiController(AppDbContext db)
    {
        _db = db;
    }

    protected async Task<User?> GetCurrentUserAsync()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                     ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId is null || !Guid.TryParse(userId, out var id))
            return null;

        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}