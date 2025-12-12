using System;
using System.Collections.Generic;
using SupplyChain.Domain;

namespace SupplyChain.Application;

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DocNumber { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public Role Role { get; set; } = Role.Employee;
    public List<string> Phones { get; set; } = new();
    public Guid? ManagerId { get; set; }
    public string Password { get; set; } = null!;
}

public class EmployeeResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Role Role { get; set; }
}