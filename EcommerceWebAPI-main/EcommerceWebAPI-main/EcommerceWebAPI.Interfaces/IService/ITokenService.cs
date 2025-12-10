namespace EcommerceWebAPI.Interfaces.IServices
{
    public interface ITokenService
    {
        string GenerateToken(string userName, string role);
    }
}
