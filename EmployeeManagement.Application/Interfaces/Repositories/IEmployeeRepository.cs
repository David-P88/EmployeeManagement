using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);

        Task<List<Employee>> GetAllAsync();

        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsByEmployeeCodeAsync(
            string employeeCode,
            int? excludeId = null);

        Task<bool> ExistsByEmailAsync(
            string email,
            int? excludeId = null);

        Task<PagedResult<Employee>> GetPagedAsync(
    EmployeeQueryParameters parameters);
        Task AddAsync(Employee employee);

        void Update(Employee employee);

        void Delete(Employee employee);

        Task SaveChangesAsync();
    }
}
