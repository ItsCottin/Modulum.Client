using Blazored.FluentValidation;
using modulum.Application.Requests.Account;
using modulum.Application.Requests.Identity;
using modulum.Client.Infrastructure.FormValidators;
using modulum.Shared.Constants.Storage;
using modulum.Shared.Wrapper;
using MudBlazor;

namespace Modulum.Client.Pages.Authentication.Register
{
    public partial class Confirmacao
    {
        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _registerFormValidator = new();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TwoFactorRequest _twoFactorModel = new();
        private bool loading;

        protected override async Task OnInitializedAsync()
        {
            _breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
            string emailLocalSessao = await _userManager.GetItemLocalStorage(StorageConstants.Local.EmailCadastro);
            if (string.IsNullOrEmpty(emailLocalSessao)) 
            {
                // Tratar erro
            }
            _twoFactorModel.Email = emailLocalSessao;
        }

        private async Task SetTwoFactorCode() 
        {
            string twoFactorCode = await _userManager.GetItemLocalStorage(StorageConstants.Local.CodeTwoFactor);
            if (string.IsNullOrEmpty(twoFactorCode))
            {
                // Tratar erro
            }
            _twoFactorModel.Code = twoFactorCode;
        }

        private void AddApiErrors(IResult response)
        {
            _registerFormValidator.ClearAllErrors();
            _registerFormValidator.DisplayAllErrors(response.Fields);
        }

        public async Task DoConfirmaEmail()
        {
            loading = true;
            if (!Validated)
            {
                loading = false;
                return;
            }
            var response = await _userManager.ConfirmEmail(_twoFactorModel);
            if (!response.Succeeded)
            {
                AddApiErrors(response);
                loading = false;
                return;
            }
            else
            {
                loading = false;
                _navigationManager.NavigateTo("/register/senha");
            }
        }

        private Breakpoint _breakpoint = Breakpoint.Xs;
        private string GetResponsivePadding() =>
            _breakpoint switch
            {
                Breakpoint.Xs => "pa-6",
                Breakpoint.Sm => "pa-10",
                Breakpoint.Md => "pa-15",
                _ => "pa-20"
            };
    }
}
