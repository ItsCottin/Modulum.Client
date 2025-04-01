using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using modulum.Application.Requests.Identity;
using modulum.Client.Infrastructure.FormValidators;
using modulum.Shared.Wrapper;
using System.Security.Claims;

namespace Modulum.Client.Pages.Authentication
{
    public partial class Login
    {
        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _loginFormValidator = new();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();

        private String textoBotao = "Entrar";
        private bool success;
        private bool loading;
        private string email = string.Empty;
        private string password = string.Empty;

        

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
            textoBotao = "  Entrando...";
            success = false;
            if (!Validated)
            {
                loading = false;
                textoBotao = "Entrar";
                return;
            }

            var result = await _authenticationManager.Login(_tokenModel);
            AddApiErrors(result);
            //if (!result.Succeeded)
            //{

            //}
            loading = false;
            textoBotao = "Entrar";
        }
    }
}
