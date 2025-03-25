using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using modulum.Application.Requests.Identity;
using System.Security.Claims;

namespace Modulum.Client.Pages.Authentication
{
    public partial class Login
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private String textoBotao = "Entrar";
        private bool success, errors;
        private bool loading;
        private string email = string.Empty;
        private string password = string.Empty;
        private string[] errorList = [];

        private TokenRequest _tokenModel = new();
        
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
            success = errors = false;
            errorList = [];

            if (!Validated)
            {
                errors = true;
                loading = false;
                textoBotao = "Entrar";
                return;
            }

            var result = await _authenticationManager.Login(_tokenModel);

            if (!result.Succeeded)
            {
                errors = true;
                errorList = result.Messages.ToArray();
            }
            loading = false;
            textoBotao = "Entrar";
        }
    }
}
