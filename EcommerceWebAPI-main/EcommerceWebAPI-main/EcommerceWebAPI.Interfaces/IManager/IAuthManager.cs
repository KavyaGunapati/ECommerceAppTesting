using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Interfaces.IManager
{
    public interface IAuthManager
    {
        Task<Result> RegisterAsync(Register dto);
        Task<Result<AuthResponse>> LoginAsync(Login dto);
        Task<Result> AssignRoleAsync(string userId, string role);
        Task<Result> SignOut(string userId);
    }

}
