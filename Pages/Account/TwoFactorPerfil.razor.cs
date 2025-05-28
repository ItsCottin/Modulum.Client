using Blazored.FluentValidation;

namespace Modulum.Client.Pages.Account
{
    public partial class TwoFactorPerfil
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private bool _loading = false;
        private bool _loadingDados = false;
        private bool _twoFactor;
        private string _label = string.Empty;
        private string _twoFactorCode = string.Empty;
        private bool _onChangeTwoFactor = false;

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _loadingDados = true;

            _twoFactor = false; // Implementar aqui a lógica para verificar se o usuário tem autenticação de dois fatores habilitada.
            _loadingDados = false;
            _loadingService.Hide();

            await Task.CompletedTask;
        }

        public async Task SalvarSeguranca()
        {
            _loadingService.Show();
            _loadingDados = true;
            if (_onChangeTwoFactor)
            {
                
            }
            else
            {

            }
            _loadingDados = false;
            _loadingService.Hide();
            await Task.CompletedTask;
        }

        private void ChangeTwoFactor(bool value)
        {
            _twoFactor = value;
            _onChangeTwoFactor = true;
        }
    }
}
