using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using modulum.Application.Requests.Identity;
using modulum.Client.Infrastructure.FormValidators;
using modulum.Shared.Wrapper;
using MudBlazor;
using System.Security.Claims;

namespace Modulum.Client.Pages.Authentication
{
    public partial class Login
    {
        // Novo login MudBlazor
        private MudForm _form;
        [Parameter]
        public bool Required { get; set; }

        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private bool _showPassword;
        private InputType _passwordInputType = InputType.Password;
        private string _passwordIcon = Icons.Material.Filled.VisibilityOff;

        private void PasswordIconClick()
        {
            if(_showPassword)
            {
                _showPassword = false;
                _passwordIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInputType = InputType.Password;
            }
            else
            {
                _showPassword = true;
                _passwordIcon = Icons.Material.Filled.Visibility;
                _passwordInputType = InputType.Text;
            }
        }


        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _loginFormValidator = new();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();

        private bool loading;        

        private void AddApiErrors(IResult response)
        {
            _loginFormValidator.ClearAllErrors();
            _loginFormValidator.DisplayAllErrors(response.Fields);
        }

        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            //if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))) // Verificar a nescessidade desse if
            //{
            //    _navigationManager.NavigateTo("/");
            //}
        }

        public async Task DoLoginAsync()
        {
            loading = true;
            if (!Validated)
            {
                loading = false;
                return;
            }

            var result = await _authenticationManager.Login(_tokenModel);
            AddApiErrors(result);
            if (result.Succeeded)
            {
                //_navigationManager.NavigateTo(""); // Redirecionar para pagina principal do sistema
            }
            loading = false;
        }
    }
}
