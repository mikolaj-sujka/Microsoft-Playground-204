using Microsoft.AspNetCore.Mvc;

namespace MicrosoftPlayground.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProfiles()
    {
        var profiles = new[]
        {
            new
            {
                Name = "John Doe",
                Email = "john.doe@example.com"
            },
            new
            {
                Name = "Jane Smith",
                Email = "jane.smith@example.com"
            }
        };
        return Ok(profiles);
    }

    [HttpGet("{id}")]
    public IActionResult GetProfileById(int id)
    {
        var profile = new
        {
            Id = id,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };
        return Ok(profile);
    }
}