using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;
//using System.Security.Cryptography;
using EmployeeManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;



namespace EmployeeManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtTokenService jwtTokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration,
            IRefreshTokenService refreshTokenService)

        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var refreshTokenExpiresInDays =
    _configuration.GetValue<int>("Jwt:RefreshTokenExpiresInDays");

            var user = await _userRepository
                .GetByUsernameAsync(request.Username);

            if (user == null || !user.IsActivate)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            var isPasswordValid = _passwordService.VerifyPassword(
                user,
                request.Password,
                user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            // Generate JWT access token
            var tokenResult = _jwtTokenService.GenerateToken(
    user.Id,
    user.Username,
    user.Email,
    user.Role);

            // Generate secure refresh token
            var refreshTokenValue = _refreshTokenService.GenerateToken();

            var refreshTokenHash =
                _refreshTokenService.HashToken(refreshTokenValue);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiresInDays),
                CreatedAt = DateTime.UtcNow
            };

            // Save refresh token
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new LoginResponse
            {
                Token = tokenResult.Token,
                RefreshToken = refreshTokenValue,
                ExpiresAt = tokenResult.ExpiresAt,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(
     RefreshTokenRequest request)
        {
            var refreshTokenExpiresInDays =
                _configuration.GetValue<int>("Jwt:RefreshTokenExpiresInDays");

            var refreshTokenHash =
    _refreshTokenService.HashToken(request.RefreshToken);

            var refreshToken = await _refreshTokenRepository
                .GetByTokenAsync(refreshTokenHash);

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid refresh token.");
            }

            if (refreshToken.IsRevoked)
            {
                throw new UnauthorizedAccessException(
                    "Refresh token has been revoked.");
            }

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException(
                    "Refresh token has expired.");
            }

            var user = refreshToken.User;

            if (!user.IsActivate)
            {
                throw new UnauthorizedAccessException(
                    "User account is inactive.");
            }

            // Generate new JWT access token
            var tokenResult = _jwtTokenService.GenerateToken(
                user.Id,
                user.Username,
                user.Email,
                user.Role);

            // Generate new refresh token
            var newRefreshTokenValue =
    _refreshTokenService.GenerateToken();

            var newRefreshTokenHash =
                _refreshTokenService.HashToken(newRefreshTokenValue);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiresInDays),
                CreatedAt = DateTime.UtcNow
            };

            // Revoke old refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(refreshToken);

            // Store new refresh token
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            // Save both changes together
            await _refreshTokenRepository.SaveChangesAsync();

            return new LoginResponse
            {
                Token = tokenResult.Token,
                RefreshToken = newRefreshTokenValue,
                ExpiresAt = tokenResult.ExpiresAt,
                Username = user.Username,
                Role = user.Role
            };
        }
        public async Task LogoutAsync(string refreshToken)
        {
            var refreshTokenHash =
    _refreshTokenService.HashToken(refreshToken);

            var token = await _refreshTokenRepository
                .GetByTokenAsync(refreshTokenHash);

            if (token == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid refresh token.");
            }

            if (token.IsRevoked)
            {
                return;
            }

            token.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(token);
            await _refreshTokenRepository.SaveChangesAsync();
        }
    }
}
