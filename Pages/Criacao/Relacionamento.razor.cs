using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Relacionamento
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        [Parameter] public int TableId { get; set; }
    }
}
