using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using modulum.Application.Requests.Identity;
using modulum.Application.Validators.Requests.Identity;
using modulum.Shared.Wrapper;
using modulum.Client.Infrastructure.FormValidators;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Modulum.Client.Pages.Authentication
{
    public partial class Register
    {

        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _registerFormValidator = new ();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private RegisterRequest _registerUserModel = new();

        private string textoBotao = "Cadastrar";
        private bool success;
        private string email = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;
        private string urlAtivaEmail = string.Empty;

        private bool loading;


        private void AddApiErrors(IResult response)
        {
            _registerFormValidator.ClearAllErrors();
            _registerFormValidator.DisplayAllErrors(response.Fields);
        }

        public async Task DoRegisterAsync()
        {
            loading = true;
            textoBotao = "  Cadastrando...";
            success = false;
            if (!Validated)
            {
                loading = false;
                textoBotao = "Cadastrar";
                return;
            }
            var response = await _userManager.RegisterUserAsync(_registerUserModel);
            AddApiErrors(response);
            if (response.Succeeded)
            {
                success = true;
                if (response.Fields.TryGetValue("AtivacaoEmail", out var ativacaoEmail))
                {
                    urlAtivaEmail = ativacaoEmail?.ToString();
                }
                _registerUserModel = new RegisterRequest();
            }
            else
            {
                loading = false;
                textoBotao = "Cadastrar";
            }
        }
    }
}
