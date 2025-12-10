using AutoMapper;
using EcommerceWebAPI.Interfaces.IManager;
using EcommerceWebAPI.Interfaces.IServices;
using EcommerceWebAPI.Models.Constants;
using EcommerceWebAPI.Models.DTOs;
using ECommerceWebAPI.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
namespace EcommerceWebAPI.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        public AuthManager(ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<Result> AssignRoleAsync(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return new Result { Success = false, Message = ErrorConstants.UserNotFound };
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (roleExist == false)
                    await _roleManager.CreateAsync(new IdentityRole(role));
                var result = await _userManager.AddToRoleAsync(user, role);
                return new Result { Success = true, Message = "Role Assigned" };
            }
            catch (Exception)
            {
                return new Result { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result<AuthResponse>> LoginAsync(Login dto)
        {
            try
            {
                var userFound = await _userManager.FindByEmailAsync(dto.Email);
                if (userFound == null) return new Result<AuthResponse> { Success = false, Message = ErrorConstants.UserNotFound };
                var result = await _signInManager.PasswordSignInAsync(userFound, dto.Password, false, false);
                if (!result.Succeeded)
                {
                    
                    return new Result<AuthResponse> { Success = false, Message = ErrorConstants.InvalidCredentials };
                }
                var roles = await _userManager.GetRolesAsync(userFound);
                var role=roles.FirstOrDefault()??"Customer";
                var token = _tokenService.GenerateToken(userFound.UserName, role);
                var response = new AuthResponse
                {
                    Token = token,
                    Email = userFound.Email,
                    Role = role,
                };
                return new Result<AuthResponse> { Success = true, Message = "Login Successfull", Data = response };

            }
            catch (Exception)
            {
                return new Result<AuthResponse> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result> RegisterAsync(Register dto)
        {
            
            try
            {
                var userExist = await _userManager.FindByEmailAsync(dto.Email);
                if (userExist != null) return new Result { Success = false, Message = "User Already Exist" };
                var newUser = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    UserName = dto.FirstName + "_" + dto.LastName,
                };
                var result = await _userManager.CreateAsync(newUser, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new Result { Success = false, Message = $"{ErrorConstants.RegistrationFailed}:{errors}" };
                }
                return new Result { Success = true, Message = "Registration Successfull" };
            }
            catch (Exception)
            {
                return new Result { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result> SignOut(string userId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userId);
                if (user == null) return new Result { Success = false, Message = ErrorConstants.UserNotFound };
                await _signInManager.SignOutAsync();
                return new Result { Success = true, Message = "Signed out successfully" };
            }
            catch (Exception)
            {
                return new Result { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }
    }
}
