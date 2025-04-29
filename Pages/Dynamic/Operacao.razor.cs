using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using modulum.Shared;
using modulum.Shared.Enum;
using MudBlazor;
using System.Globalization;
using static System.Net.WebRequestMethods;

namespace Modulum.Client.Pages.Dynamic
{
    public partial class Operacao
    {
        [Parameter] public int TableId { get; set; }
        [Parameter] public string Operation { get; set; }
        [Parameter] public int? RecordId { get; set; }
        private bool _loading { get; set; } = false;
        private bool _loadingDados { get; set; } = false;
        public bool IsView { get; set; } = false;

        private DynamicTableRequest _registro = new();
        private List<DynamicFieldRequest> _campos = new();

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _loadingDados = true;
            var response = await _dynamicManager.GetAllRegistroTabela(TableId);
            if (response.Succeeded)
            {
                _registro = response.Data;
            }
            else
            {
                return; // Tratar erro aqui
            }
            if (Operation == "insert")
            {
                var response2 = await _dynamicManager.GetNewObject(TableId);
                if (response2.Succeeded)
                {
                    _registro = response2.Data;
                }
                else
                {
                    return; // Tratar erro aqui
                }
                var _campos2 = response2.Data?.Resultados?.FirstOrDefault()?.Valores ?? new List<DynamicFieldRequest>();
                foreach (var campo in _campos2)
                {
                    if (campo.Tipo == TypeColumnEnum.BIT)
                    {
                        campo.Valor = "0";
                    }
                    _campos.Add(campo);
                }
                //_campos = response2.Data?.Resultados?.FirstOrDefault()?.Valores;
            }
            else if (Operation == "update" && RecordId.HasValue)
            {
                var registroExistente = response.Data.Resultados.FirstOrDefault(r => r.Id == RecordId);
                var _campos2 = registroExistente?.Valores ?? new List<DynamicFieldRequest>();
                foreach (var campo in _campos2)
                {
                    if (campo.Tipo == TypeColumnEnum.DATE)
                    {
                        campo.Valor = AjustarFormatoDataParaInput(campo.Valor);
                    }
                    _campos.Add(campo);
                }
                IsView = false;
            }
            else if (Operation == "select" && RecordId.HasValue)
            {
                var registroExistente = response.Data.Resultados.FirstOrDefault(r => r.Id == RecordId);
                _campos = registroExistente?.Valores ?? new List<DynamicFieldRequest>();
                IsView = true;
            }
            _loadingDados = false;
            _loadingService.Hide();
        }

        private string AjustarFormatoDataParaInput(string data)
        {
            try
            {
                // A data está no formato "MM/dd/yyyy HH:mm:ss"
                var date = DateTime.ParseExact(data, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                data = date.ToString("yyyy-MM-ddTHH:mm"); // Formato aceito por input[datetime-local]
                return data;
            }
            catch (FormatException)
            {
                return data;
            }
        }

        private async Task SalvarRegistro()
        {
            _loading = true;
            _loadingService.Show();
            var payload = new DynamicTableRequest()
            {
                NomeTabela = _registro.NomeTabela,
                NomeTela = _registro.NomeTela,
                CampoPK = _registro.CampoPK,
                JsonObject = _registro.JsonObject,
                TelaObject = _registro.TelaObject,
                Id = TableId,
                Resultados = new List<DynamicDadoRequest>
                {
                    new DynamicDadoRequest ()
                    {
                        Id = RecordId ?? 0,
                        Valores = _campos
                    }
                }
            };
            var response = await _dynamicManager.OperacaoRegistro(payload, Operation);
            _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? Severity.Success : Severity.Error);
            
            // Redirecionar de volta para grid depois de salvar
            _loadingService.Hide(); 
            _loading = false;
            if (response.Succeeded)
            {
                _navigationManager.NavigateTo("/dynamic/" + TableId);
            }
        }
    }
}
