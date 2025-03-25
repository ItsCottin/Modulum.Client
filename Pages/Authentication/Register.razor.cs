using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Identity;

namespace Modulum.Client.Pages.Authentication
{
    public partial class Register
    {
        //private FluentValidationValidator _fluentValidationValidator;
        //private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private RegisterRequest _registerUserModel = new();

        private string textoBotao = "Cadastrar";
        private bool success, errors;
        private string email = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;
        private string[] errorList = [];
        private string urlAtivaEmail = string.Empty;

        private bool loading;

        public async Task DoRegisterAsync()
        {
            loading = true;
            textoBotao = "  Cadastrando...";
            success = errors = false;
            errorList = [];
            if (false)
            {
                errors = true;
                loading = false;
                textoBotao = "Cadastrar";
                return;
            }
            var response = await _userManager.RegisterUserAsync(_registerUserModel);
            if (response.Succeeded)
            {
                success = true;
                errorList = response.Messages.ToArray();
                _registerUserModel = new RegisterRequest();
            }
            else
            {
                errors = true;
                errorList = response.Messages.ToArray();
                _registerUserModel = new RegisterRequest();
            }
        }
    }
}
