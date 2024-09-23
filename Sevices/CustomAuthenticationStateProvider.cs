using modulum_client.Model;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
namespace modulum_client.Sevices
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
    {
        private bool _authenticated = false;
        private string? _menssagem;

        private readonly ClaimsPrincipal Unauthenticated =
           new(new ClaimsIdentity());

        private readonly HttpClient _httpClient;


        private readonly JsonSerializerOptions jsonSerializerOptions =
          new()
          {
              PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          };
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthenticationStateProvider(IHttpClientFactory httpClientFactory, 
            ILocalStorageService localStorageService)
        {
            _httpClient = httpClientFactory.CreateClient("Auth");
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _authenticated = false;

            // default to not authenticated
            var user = Unauthenticated;

            try
            {
                var userResponse = await _httpClient.GetAsync("manage/info");

                userResponse.EnsureSuccessStatusCode();

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, jsonSerializerOptions);

                if (userInfo != null)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email)
                    };

                    claims.AddRange(
                      userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                          .Select(c => new Claim(c.Key, c.Value)));

                    var rolesResponse = await _httpClient.GetAsync($"Role/GetuserRole?userEmail={userInfo.Email}");

                    rolesResponse.EnsureSuccessStatusCode();

                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();

                    var roles = JsonSerializer.Deserialize<string[]>(rolesJson, jsonSerializerOptions);
                    if (roles != null && roles?.Length > 0)
                    {
                        foreach (var role in roles)
                        {
                            claims.Add(new(ClaimTypes.Role, role));
                        }
                    }

                    var id = new ClaimsIdentity(claims,nameof(CustomAuthenticationStateProvider));

                    user = new ClaimsPrincipal(id);

                    _authenticated = true;

                }
            }
            catch (Exception ex)
            {

               
            }
            return new AuthenticationState(user);

        }

        public async Task<BaseResponse> RegisterAsync(string userName, string email, string password)
        {
            string[] defaultDetail = ["Um erro desconhecido impediu o sucesso do registro."];
            try
            {
                var result = await _httpClient.PostAsJsonAsync("Account/create", new { userName, email, password }); // TODO - Alterar nessa linha para realizar chamada ao endpoint implementado - Implementado
                if (result.IsSuccessStatusCode)
                {
                    return new BaseResponse { Status = true, Mensagens = ["Conta criada com sucesso." ] };
                }

                var details = await result.Content.ReadAsStringAsync();
                var problemDetails = JsonDocument.Parse(details);
                var mensagens = new List<string>();
                var errorList = problemDetails.RootElement.GetProperty("errors");

                foreach (var errorEntry in errorList.EnumerateObject())
                {
                    if (errorEntry.Value.ValueKind == JsonValueKind.String)
                    {
                        mensagens.Add(errorEntry.Value.GetString()!);
                    }
                    else if (errorEntry.Value.ValueKind == JsonValueKind.Array)
                    {
                        mensagens.AddRange(
                            errorEntry.Value.EnumerateArray().Select(e => e.GetString() ?? string.Empty)
                        );
                    }
                }
                return new BaseResponse { Status = false, Mensagens = mensagens.Any() ? mensagens.ToArray() : defaultDetail };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Status = false, Mensagens = [ ex.Message ] };
            }
        }

        public async Task<BaseResponse> LoginAsync(string email, string password)
        {
            try
            {
                var emailIsConfirmedResponse = await _httpClient.GetAsync($"Account/isConfirmedEmail?email={email}");
                if (emailIsConfirmedResponse.IsSuccessStatusCode)
                {
                    var emailIsConfirmed = await emailIsConfirmedResponse.Content.ReadAsStringAsync();
                    var baseResponse = JsonSerializer.Deserialize<BaseResponse>(emailIsConfirmed, jsonSerializerOptions);
                    if (!baseResponse.Status) 
                    {
                        return baseResponse;
                    }
                }

                var result = await _httpClient.PostAsJsonAsync("login", new { email, password } );

                if(result.IsSuccessStatusCode)
                {
                    var tokenResponse = await result.Content.ReadAsStringAsync();
                    var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(tokenResponse, jsonSerializerOptions);
                    await _localStorageService.SetItemAsync("accessToken", tokenInfo.AccessToken);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return new BaseResponse { Status = true };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return new BaseResponse { Status = false, Mensagens = [ "E-mail e/ou senha inválidos." ] };
        }

        public async Task LogoutAsync()
        {
            const string Empty = "{}";
            var emptyContent = new StringContent(Empty, Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync("Account/Logout", emptyContent);
            if(result.IsSuccessStatusCode)
            {
                await  _localStorageService.RemoveItemAsync("accessToken");
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
            

        }

        public async Task<bool> CheckAuthenticatedAsync()
        {
            await GetAuthenticationStateAsync();
            return _authenticated;
        }

        public async Task<List<Role>> GetRoles()
        {
            try
            {
                var result = await _httpClient.GetAsync("Role/GetRoles");
                var resposne = await result.Content.ReadAsStringAsync();
                var rolelist = JsonSerializer.Deserialize<List<Role>>(resposne, jsonSerializerOptions);
                if (result.IsSuccessStatusCode)
                {
                    return rolelist;
                }
            }
            catch (Exception ex)
            {

            }
            return new List<Role>();

        }

        public async Task<BaseResponse> AddRole(string[] roles)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(roles), Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync("Role/addRoles", content);

                if (result.IsSuccessStatusCode)
                {
                    return new BaseResponse { Status = true };
                }
            }
            catch (Exception ex)
            {


            }
            return new BaseResponse { Status = false, Mensagens = ["API tem algum problema, mais não é sua culpa"] };
        }

        public async Task<UserViewModel[]> GetUsers()
        {
            try
            {
                var result = await _httpClient.GetAsync("User");
                var resposne = await result.Content.ReadAsStringAsync();
                var userlist = JsonSerializer.Deserialize<UserViewModel[]>(resposne, jsonSerializerOptions);
                if (result.IsSuccessStatusCode)
                {
                    return userlist;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<UserViewModel> GetUserByEmail(string userEmailId)
        {
            try
            {
                var result = await _httpClient.GetAsync($"User/{userEmailId}");
                var resposne = await result.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserViewModel>(resposne, jsonSerializerOptions);
                if (result.IsSuccessStatusCode)
                {
                    return user;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<bool> UserUpdate(string userEmailId, UserViewModel user)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"User/{userEmailId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<bool> Delete(string userEmailId)
        {
            try
            {
                var result = await _httpClient.DeleteAsync($"User/{userEmailId}");
                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
