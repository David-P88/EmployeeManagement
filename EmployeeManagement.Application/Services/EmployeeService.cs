using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<PagedResult<EmployeeResponse>> GetPagedAsync(
    EmployeeQueryParameters parameters)
        {
            if (parameters.PageNumber < 1)
                parameters.PageNumber = 1;

            if (parameters.PageSize < 1)
                parameters.PageSize = 10;

            if (parameters.PageSize > 100)
                parameters.PageSize = 100;

            var result = await _employeeRepository.GetPagedAsync(parameters);

            return new PagedResult<EmployeeResponse>
            {
                Items = result.Items.Select(e => new EmployeeResponse
                {
                    Id = e.Id,
                    EmployeeCode = e.EmployeeCode,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Salary = e.Salary,
                    DateOfJoining = e.DateOfJoining,
                    IsActive = e.IsActive,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department?.Name ?? string.Empty
                }),

                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            return MapToResponse(employee);
        }

        public async Task<List<EmployeeResponse>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            return employees
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request)
        {
            if (await _employeeRepository
                .ExistsByEmployeeCodeAsync(request.EmployeeCode))
            {
                throw new InvalidOperationException(
                    "Employee code already exists.");
            }

            if (await _employeeRepository
                .ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            var employee = new Employee
            {
                EmployeeCode = request.EmployeeCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Salary = request.Salary,
                DateOfJoining = request.DateOfJoining,
                DepartmentId = request.DepartmentId,
                IsActive = true
            };

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            var createdEmployee =
                await _employeeRepository.GetByIdAsync(employee.Id);

            return MapToResponse(createdEmployee!);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            if (await _employeeRepository
                .ExistsByEmailAsync(request.Email, id))
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Salary = request.Salary;
            employee.DepartmentId = request.DepartmentId;
            employee.IsActive = request.IsActive;

            _employeeRepository.Update(employee);

            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            _employeeRepository.Delete(employee);

            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        private static EmployeeResponse MapToResponse(
            Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Salary = employee.Salary,
                DateOfJoining = employee.DateOfJoining,
                IsActive = employee.IsActive,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty
            };
        }
    }
}
