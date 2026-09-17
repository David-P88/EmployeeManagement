namespace EmployeeManagement.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(
    int userId,
    string username,
    string email,
    string role);
    }
}
