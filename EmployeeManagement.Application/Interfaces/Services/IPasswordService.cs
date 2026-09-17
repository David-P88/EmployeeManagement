using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces.Services
{
    public interface IPasswordService
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(
            User user,
            string password,
            string passwordHash);
    }
}
