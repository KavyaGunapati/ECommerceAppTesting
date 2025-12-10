using EcommerceWebAPI.Interfaces.IManager;
using EcommerceWebAPI.Interfaces.IService;
using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthManager _authManager;
        public AuthService(IAuthManager authManager)
        {
            _authManager = authManager;
        }
        public async Task<Result> AssignRoleAsync(string userId, string role)
        {
          var result= await _authManager.AssignRoleAsync(userId, role);
            return result;
        }

        public async Task<Result<AuthResponse>> LoginAsync(Login dto)
        {
            return await _authManager.LoginAsync(dto);
        }

        public async Task<Result> RegisterAsync(Register dto)
        {
            return await _authManager.RegisterAsync(dto);
        }

        public async Task<Result> SignOut(string userId)
        {
           return await _authManager.SignOut(userId);
        }
    }
}
