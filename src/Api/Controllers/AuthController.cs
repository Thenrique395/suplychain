using Microsoft.AspNetCore.Mvc;
using SupplyChain.Application;
using SupplyChain.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace SupplyChain.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IEmployeeRepository _repo;
    private readonly IAuthService _auth;
    private readonly IConfiguration _config;

    public AuthController(IEmployeeRepository repo, IAuthService auth, IConfiguration config)
    {
        _repo = repo;
        _auth = auth;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateEmployeeRequest req)
    {
        var exists = await _repo.GetByDocAsync(req.DocNumber);
        if (exists != null) return BadRequest("Doc already exists");

        var user = new Employee
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            DocNumber = req.DocNumber,
            BirthDate = req.BirthDate,
            Role = req.Role,
            Phones = req.Phones,
            PasswordHash = _auth.HashPassword(req.Password)
        };

        await _repo.AddAsync(user);
        return Ok(new { user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _repo.GetByDocAsync(req.DocNumber);
        if (user == null) return Unauthorized();
        if (!_auth.Verify(req.Password, user.PasswordHash)) return Unauthorized();

        var token = _auth.GenerateToken(user, _config);
        return Ok(new { token });
    }
}

public class LoginRequest
{
    public string DocNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
}