using SupplyChain.Domain;

namespace SupplyChain.Application;

public class CreateEmployeeRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string DocNumber { get; init; } = null!;
    public DateTime BirthDate { get; init; }
    public Role Role { get; init; } = Role.Employee;
    public List<string> Phones { get; init; } = [];
    public Guid? ManagerId { get; set; }
    public string Password { get; init; } = null!;
}

public class EmployeeResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Role Role { get; set; }
}