using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modulum.Client.Pages.Account
{
    public partial class AddEditEnderecoPerfil
    {
        [Parameter] public string Operation { get; set; } = string.Empty;
        [Parameter] public int IdEndereco { get; set; }
        private bool _loading = false;
        private bool _loadingDados = false;
        private bool _loadingCEP = false;

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        public PatternMask maskCep = new PatternMask("XX-XX-XX-XX-XX-XX")
        {
            MaskChars = new[] { new MaskChar('X', @"[0-9a-fA-F]") },
            Placeholder = '_',
            CleanDelimiters = true,
            Transformation = AllUpperCase
        };

        // transform lower-case chars into upper-case chars
        private static char AllUpperCase(char c) => c.ToString().ToUpperInvariant()[0];
        private async Task PesquisarCEP()
        {
            _loadingCEP = !_loadingCEP;
            await Task.CompletedTask;
        }

        private async Task SalvarEndereco()
        {
            _loading = !_loading;
            await Task.CompletedTask;
        }


        public string GetDisplayText(Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type type = value.GetType();
            if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
            {
                var sb = new System.Text.StringBuilder();

                foreach (Enum field in Enum.GetValues(type))
                {
                    if (Convert.ToInt64(field) == 0 && Convert.ToInt32(value) > 0)
                        continue;

                    if (value.HasFlag(field))
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");

                        var f = type.GetField(field.ToString());
                        var da = (DisplayAttribute)Attribute.GetCustomAttribute(f, typeof(DisplayAttribute));
                        sb.Append(da?.ShortName ?? da?.Name ?? field.ToString());
                    }
                }

                return sb.ToString();
            }
            else
            {
                var f = type.GetField(value.ToString());
                if (f != null)
                {
                    var da = (DisplayAttribute)Attribute.GetCustomAttribute(f, typeof(DisplayAttribute));
                    if (da != null)
                        return da.ShortName ?? da.Name;
                }
            }
            return value.ToString();
        }
    }
}
