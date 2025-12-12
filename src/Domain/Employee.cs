using System;
using System.Collections.Generic;

namespace SupplyChain.Domain;

public enum Role
{
    Employee = 1,
    Leader = 2,
    Director = 3
}

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DocNumber { get; set; } = null!;
    public DateTimeOffset BirthDate { get; set; }
    public Role Role { get; set; } = Role.Employee;
    public List<string> Phones { get; set; } = new();
    public Guid? ManagerId { get; set; }
    public string PasswordHash { get; set; } = null!;
}