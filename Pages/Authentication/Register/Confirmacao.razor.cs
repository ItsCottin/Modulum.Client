using Blazored.FluentValidation;
using modulum.Application.Requests.Account;
using modulum.Application.Requests.Identity;
using modulum.Client.Infrastructure.FormValidators;
using modulum.Shared.Constants.Storage;
using modulum.Shared.Wrapper;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

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
            Code0 = twoFactorCode.Substring(0, 1);
            Code1 = twoFactorCode.Substring(1, 1);
            Code2 = twoFactorCode.Substring(2, 1);
            Code3 = twoFactorCode.Substring(3, 1);
            Code4 = twoFactorCode.Substring(4, 1);
            Code5 = twoFactorCode.Substring(5, 1);
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
            var codigoCompleto = $"{Code0}{Code1}{Code2}{Code3}{Code4}{Code5}";
            _twoFactorModel.Code = codigoCompleto;
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
                Breakpoint.Xs => "pt-3 pb-3",
                Breakpoint.Sm => "pa-10",
                Breakpoint.Md => "pa-15",
                _ => "pa-20"
            };

        private string Code0 { get; set; } = string.Empty;
        private string Code1 { get; set; } = string.Empty;
        private string Code2 { get; set; } = string.Empty;
        private string Code3 { get; set; } = string.Empty;
        private string Code4 { get; set; } = string.Empty;
        private string Code5 { get; set; } = string.Empty;

        private async Task HandleInput()
        {
            _twoFactorModel.Code = $"{Code0}{Code1}{Code2}{Code3}{Code4}{Code5}";
        }

        private void ValidarCodigo()
        {
            //var codigoCompleto = string.Join("", codes);
            //Console.WriteLine($"Código digitado: {codigoCompleto}");

            // Envie para API ou execute sua lógica aqui
        }
    }
}
