using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Application;
using SupplyChain.Domain;

namespace SupplyChain.Infrastructure;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _ctx;
    public EmployeeRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Employee e)
    {
        _ctx.Employees.Add(e);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var e = await _ctx.Employees.FindAsync(id);
        if (e != null)
        {
            _ctx.Employees.Remove(e);
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync() => await _ctx.Employees.ToListAsync();

    public async Task<Employee?> GetByDocAsync(string doc) => await _ctx.Employees.FirstOrDefaultAsync(x => x.DocNumber == doc);

    public async Task<Employee?> GetByIdAsync(Guid id) => await _ctx.Employees.FindAsync(id);

    public async Task UpdateAsync(Employee e)
    {
        _ctx.Employees.Update(e);
        await _ctx.SaveChangesAsync();
    }
}