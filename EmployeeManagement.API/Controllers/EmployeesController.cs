using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IValidator<CreateEmployeeRequest> _createValidator;

        public EmployeesController(IEmployeeService employeeService, IValidator<CreateEmployeeRequest> createValidator)
        {
            _employeeService = employeeService;
            _createValidator = createValidator;
        }
        //public EmployeesController(IEmployeeService employeeService)
        //{
        //    _employeeService = employeeService;
        //}

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<EmployeeResponse>>> GetPaged(
    [FromQuery] EmployeeQueryParameters parameters)
        {
            var result = await _employeeService.GetPagedAsync(parameters);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeResponse>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();

            return Ok(employees);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
                return NotFound(new
                {
                    message = $"Employee with ID {id} not found."
                });

            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> Create(CreateEmployeeRequest request)
        {
            var validationResult =
        await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var employee =
                await _employeeService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateEmployeeRequest request)
        {
            var updated =
        await _employeeService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = $"Employee with ID {id} not found."
                });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _employeeService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new
                {
                    message = $"Employee with ID {id} not found."
                });

            return NoContent();
        }
    }
}
