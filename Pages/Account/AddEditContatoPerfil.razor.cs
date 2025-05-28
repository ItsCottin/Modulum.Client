using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using modulum.Shared.Enum;

namespace Modulum.Client.Pages.Account
{
    public partial class AddEditContatoPerfil
    {
        [Parameter] public string Operation { get; set; } = string.Empty;
        [Parameter] public int IdContato { get; set; }
        private bool _loading = false;
        private bool _loadingDados = false;
        private TypeContatoEnum? _tipoContatoSelecionado = TypeContatoEnum.Celular;

        private PatternMask _contatoMask = new PatternMask("00 0 0000-0000");
        private string numeroContato;

        private async Task SalvarContato()
        {
            _loading = !_loading;
            await Task.CompletedTask;
        }

        private void ChangeMask()
        {
            if (_tipoContatoSelecionado == TypeContatoEnum.Celular)
            {
                _contatoMask = new PatternMask("00 0 0000-0000");
            }
            else
            {
                _contatoMask = new PatternMask("00 0000-0000");
            }
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
