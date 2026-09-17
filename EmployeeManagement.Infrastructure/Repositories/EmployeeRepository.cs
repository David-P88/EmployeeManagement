using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(x => x.Department)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees
                .AnyAsync(x => x.Id == id);
        }
        public async Task<bool> ExistsByEmployeeCodeAsync(
        string employeeCode,
        int? excludeId = null)
        {
            return await _context.Employees
                .AnyAsync(x =>
                    x.EmployeeCode == employeeCode &&
                    (!excludeId.HasValue || x.Id != excludeId.Value));
        }
        public async Task<bool> ExistsByEmailAsync(
        string email,
        int? excludeId = null)
        {
            return await _context.Employees
                .AnyAsync(x =>
                    x.Email == email &&
                    (!excludeId.HasValue || x.Id != excludeId.Value));
        }
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
        }

        public void Delete(Employee employee)
        {
            _context.Employees.Remove(employee);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(
    EmployeeQueryParameters parameters)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();

                query = query.Where(e =>
                    e.EmployeeCode.Contains(search) ||
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search));
            }

            // Department filter
            if (parameters.DepartmentId.HasValue)
            {
                query = query.Where(e =>
                    e.DepartmentId == parameters.DepartmentId.Value);
            }

            // Total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "firstname" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.FirstName)
                    : query.OrderBy(e => e.FirstName),

                "lastname" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.LastName)
                    : query.OrderBy(e => e.LastName),

                "salary" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.Salary)
                    : query.OrderBy(e => e.Salary),

                "email" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.Email)
                    : query.OrderBy(e => e.Email),

                _ => query.OrderBy(e => e.Id)
            };

            // Pagination
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Employee>
            {
                Items = items,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount
            };
        }

    }
}
