using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Commons;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("users")]
[Authorize(Roles = $"{Roles.Admin}, {Roles.Member}")]
public sealed class UsersController(ApplicationDbContext context,
    UserContext userContext,
    LinkService linkService,
    ApplicationDbContext dbContext)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        
        UserDto? user = await context.Users
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null || user.Id != userId)
        {
            return NotFound();
        }

        return Ok(user);
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser([FromHeader(Name = "Accept")] string? accept)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        
        UserDto? user = await context.Users
            .Where(u => u.Id == userId)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }
        
        if (accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            user.Links = [];
        }

        return Ok(user);
    }

    [HttpPut("me/profile")]
    public async Task<ActionResult> UpdateProfile(UpdateUserProfileDto dto)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        User? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return NotFound();
        }

        user.Name = dto.Name;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForUser()
    {
        List<LinkDto> links = 
        [
            linkService.Create(nameof(GetCurrentUser), "self", HttpMethods.Get),
            linkService.Create(nameof(UpdateProfile), "update-profile", HttpMethods.Put)
        ];

        return links;
    }
}
