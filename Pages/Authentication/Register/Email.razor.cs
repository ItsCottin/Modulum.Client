using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Identity;
using modulum.Shared.Wrapper;
using modulum.Client.Infrastructure.FormValidators;
using MudBlazor;
using static MudBlazor.Colors;

namespace Modulum.Client.Pages.Authentication.Register
{
    public partial class Email
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
        private PreRegisterRequest _registerUserModel = new();

        private bool loading;

        private void AddApiErrors(IResult response)
        {
            _registerFormValidator.ClearAllErrors();
            _registerFormValidator.DisplayAllErrors(response.Fields);
        }

        public async Task DoRegisterAsync()
        {
            _loadingService.Show();
            loading = true;
            if (!Validated)
            {
                loading = false;
                _loadingService.Hide();
                return;
            }
            var response = await _userManager.PreRegisterUserAsync(_registerUserModel);
            AddApiErrors(response);
            if (response.Succeeded)
            {
                loading = false;
                _loadingService.Hide();
                _navigationManager.NavigateTo("/register/confirmacao");
            }
            else
            {
                loading = false;
                _loadingService.Hide();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync(); 
            _loadingService.Hide();
        }

        private Breakpoint _breakpoint = Breakpoint.Xs;
        private string GetResponsivePadding() =>
            _breakpoint switch
            {
                Breakpoint.Xs => "pt-3 pb-3",
                Breakpoint.Sm => "pa-12",
                Breakpoint.Md => "pa-15",
                _ => "pa-18"
            };
    }
}
