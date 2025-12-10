using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Interfaces.IService
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(Register dto);
        Task<Result<AuthResponse>> LoginAsync(Login dto);
        Task<Result> AssignRoleAsync(string userId, string role);
        Task<Result> SignOut(string userId);
    }
}
