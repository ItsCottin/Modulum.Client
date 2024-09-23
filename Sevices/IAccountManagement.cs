using modulum_client.Model.Response;
using modulum_client.Model;
using System.Data;

namespace modulum_client.Sevices
{
    public interface IAccountManagement
    {
        public Task<CadastroUsuarioResponse> RegisterAsync(string nome, string email, string password);

        public Task<BaseResponse> LoginAsync(string email, string password);

        public Task<BaseResponse> ConfirmEmail(string userId, string token);

        public Task LogoutAsync();

        public Task<bool> CheckAuthenticatedAsync();

        public Task<List<Role>> GetRoles();

        public Task<BaseResponse> AddRole(string[] roles);

        public Task<UserViewModel[]> GetUsers();

        public Task<UserViewModel> GetUserByEmail(string userEmailId);

        public Task<bool> UserUpdate(string userEmailId, UserViewModel user);

        public Task<bool> Delete(string userEmailId);
    }
}
