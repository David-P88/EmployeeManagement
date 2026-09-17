namespace EmployeeManagement.Application.DTOs.Employees
{
    public class UpdateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }

        public bool IsActive { get; set; }
    }
}
