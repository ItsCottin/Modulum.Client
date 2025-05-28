using Blazored.FluentValidation;
using MudBlazor;

namespace Modulum.Client.Pages.Account
{
    public partial class EnderecoPerfil
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private void Editar()
        {
            _navigationManager.NavigateTo("/account/manage/endereco/edit/1");
        }
        private bool _loading = false;
        private bool _loadingDados = false;
    }
}
