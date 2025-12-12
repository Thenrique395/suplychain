using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Domain;

namespace SupplyChain.Application;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByDocAsync(string doc);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task AddAsync(Employee e);
    Task UpdateAsync(Employee e);
    Task DeleteAsync(Guid id);
}