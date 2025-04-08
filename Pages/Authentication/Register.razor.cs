using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Identity;
using modulum.Shared.Wrapper;
using modulum.Client.Infrastructure.FormValidators;
using MudBlazor;

namespace Modulum.Client.Pages.Authentication
{
    public partial class Register
    {
        [Parameter]
        public bool Required { get; set; }
        [Parameter]
        public string Class { get; set; }

        private bool _showPassword;
        private InputType _passwordInputType = InputType.Password;
        private string _passwordIcon = Icons.Material.Filled.VisibilityOff;

        private void PasswordIconClick()
        {
            if (_showPassword)
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
        private FormValidator _registerFormValidator = new ();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private RegisterRequest _registerUserModel = new();

        private bool loading;
        private bool success;
        private string urlAtivaEmail = string.Empty;

        private void AddApiErrors(IResult response)
        {
            _registerFormValidator.ClearAllErrors();
            _registerFormValidator.DisplayAllErrors(response.Fields);
        }

        public async Task DoRegisterAsync()
        {
            loading = true;
            success = false;
            if (!Validated)
            {
                loading = false;
                return;
            }
            var response = await _userManager.RegisterUserAsync(_registerUserModel);
            AddApiErrors(response);
            if (response.Succeeded)
            {
                if (response.Fields.TryGetValue("AtivacaoEmail", out var ativacaoEmail))
                {
                    urlAtivaEmail = ativacaoEmail?.ToString();
                }
                success = true;
                _registerUserModel = new RegisterRequest();
            }
            else
            {
                loading = false;
            }
        }
    }
}
