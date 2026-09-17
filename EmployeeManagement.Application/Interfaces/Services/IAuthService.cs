using EmployeeManagement.Application.DTOs;



namespace EmployeeManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(
    RefreshTokenRequest request);

        Task LogoutAsync(string refreshToken);

    }
}
