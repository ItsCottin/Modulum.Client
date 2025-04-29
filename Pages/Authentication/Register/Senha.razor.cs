using Blazored.FluentValidation;
using modulum.Application.Requests.Identity;
using modulum.Client.Infrastructure.FormValidators;
using MudBlazor;
using modulum.Shared.Wrapper;
using nodulum.Application.Requests.Identity;
using modulum.Shared.Constants.Storage;

namespace Modulum.Client.Pages.Authentication.Register
{
    public partial class Senha
    {
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
        private FormValidator _registerFormValidator = new();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private FinishRegisterRequest _finishRegisterModel = new();
        private bool loading;

        private void AddApiErrors(IResult response)
        {
            _registerFormValidator.ClearAllErrors();
            _registerFormValidator.DisplayAllErrors(response.Fields);
        }

        public async Task DoFinalizaRegistroAsync()
        {
            _loadingService.Show();
            loading = true;
            if (!Validated)
            {
                loading = false;
                _loadingService.Hide();
                return;
            }
            var response = await _userManager.FimRegisterUserAsync(_finishRegisterModel);
            if (!response.Succeeded)
            {
                AddApiErrors(response);
                loading = false;
                _loadingService.Hide();
                return;
            }
            else
            {
                loading = false;
                _loadingService.Hide();
                _navigationManager.NavigateTo("/register/concluido");
            }
        }


        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
            string emailLocalSessao = await _userManager.GetItemLocalStorage(StorageConstants.Local.EmailCadastro);
            if (string.IsNullOrEmpty(emailLocalSessao))
            {
                // Tratar erro
            }
            _finishRegisterModel.Email = emailLocalSessao;
            _loadingService.Hide();
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
    }
}
