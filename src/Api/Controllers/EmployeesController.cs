using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.Application;
using SupplyChain.Domain;

namespace SupplyChain.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repo;
    private readonly IAuthService _auth;
    public EmployeesController(IEmployeeRepository repo, IAuthService auth)
    {
        _repo = repo;
        _auth = auth;
    }

    private Role CurrentRole
    {
        get
        {
            var r = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            return Enum.TryParse<Role>(r, out var role) ? role : Role.Employee;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var list = await _repo.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            list = list.Where(e =>
                (!string.IsNullOrWhiteSpace(e.FirstName) && e.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(e.LastName) && e.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(e.Email) && e.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(e.DocNumber) && e.DocNumber.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        var total = list.Count();
        var items = list.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new {
                e.Id,
                e.FirstName,
                e.LastName,
                e.Email,
                e.DocNumber,
                e.Role,
                e.Phones
            });

        return Ok(new { totalCount = total, page, pageSize, items });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest req)
    {
        if (req.Phones == null || req.Phones.Count < 2) return BadRequest("Must have at least 2 phones");
        if (req.BirthDate > DateTime.UtcNow.AddYears(-18)) return BadRequest("Underage not allowed");
        if (req.Role > CurrentRole) return Forbid("You cannot create employee with higher role");
        if (string.IsNullOrWhiteSpace(req.Password)) return BadRequest("Password is required");

        var exists = await _repo.GetByDocAsync(req.DocNumber);
        if (exists != null) return BadRequest("Doc already exists");

        var e = new Employee
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            DocNumber = req.DocNumber,
            BirthDate = req.BirthDate,
            Role = req.Role,
            Phones = req.Phones,
            ManagerId = req.ManagerId,
            PasswordHash = _auth.HashPassword(req.Password)
        };

        await _repo.AddAsync(e);
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, e);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return NotFound();
        return Ok(e);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateEmployeeRequest req)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        if (req.Phones == null || req.Phones.Count < 2) return BadRequest("Must have at least 2 phones");
        if (req.BirthDate > DateTime.UtcNow.AddYears(-18)) return BadRequest("Underage not allowed");
        if (req.Role > CurrentRole) return Forbid("You cannot set a role higher than yours.");

        existing.FirstName = req.FirstName;
        existing.LastName = req.LastName;
        existing.Email = req.Email;
        existing.Phones = req.Phones;
        existing.ManagerId = req.ManagerId;
        existing.Role = req.Role;
        if (!string.IsNullOrWhiteSpace(req.Password))
        {
            existing.PasswordHash = _auth.HashPassword(req.Password);
        }

        await _repo.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
