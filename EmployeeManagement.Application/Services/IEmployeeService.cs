using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.Application.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeResponse?> GetByIdAsync(int id);

        Task<List<EmployeeResponse>> GetAllAsync();

        Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request);


        Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request);

        Task<bool> DeleteAsync(int id);

        Task<PagedResult<EmployeeResponse>> GetPagedAsync(
    EmployeeQueryParameters parameters);
    }
}
