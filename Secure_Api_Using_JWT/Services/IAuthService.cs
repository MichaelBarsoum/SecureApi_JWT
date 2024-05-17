using Secure_Api_Using_JWT.Models;

namespace Secure_Api_Using_JWT.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(RoleModel role);
    }
}
