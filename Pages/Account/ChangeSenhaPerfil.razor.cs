using Blazored.FluentValidation;
using modulum.Domain.Entities.Account;

namespace Modulum.Client.Pages.Account
{
    public partial class ChangeSenhaPerfil
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private bool _loading = false;
        private bool _loadingDados = false;
        
        public string senhaAtual { get; set; } = string.Empty;

        public void SalvarSenha()
        {
            _loading = !_loading;
        }
    }
}
