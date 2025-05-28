using Blazored.FluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using modulum.Application.Validators.Requests.Dynamic;
using modulum.Client.Infrastructure.FormValidators;
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
        private bool _submitFoiChamado = false;

        private DynamicTableRequest _registro = new();
        private List<DynamicFieldRequest> _campos = new();

        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _dynamicFormValidator = new();
        //private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _loadingDados = true;
            // Refazer toda a estrutura de chamadas pois agora a API tem metodos especificos para trazer um unico registro ou trazer o formulario limpo
            // Porem como esta funcionando não vou mexer agora
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
                _registro.Resultados = new List<DynamicDadoRequest> { registroExistente };
                _registro.Resultados.First().Valores = _campos;
                IsView = false;
            }
            else if (Operation == "select" && RecordId.HasValue)
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
                _registro.Resultados = new List<DynamicDadoRequest> { registroExistente };
                _registro.Resultados.First().Valores = _campos;
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
                var date = DateTime.ParseExact(data, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                data = date.ToString("yyyy-MM-ddTHH:mm"); // Formato aceito por input[datetime-local]
                return data;
            }
            catch (FormatException ex)
            {
                // Chapeu pois quando a API esta hospedada no IIS Azure, retorna 'MM/dd/yyyy HH:mm:ss', ja localmente retorna 'dd/MM/yyyy HH:mm:ss'
                // Avaliar correção posteriormente na API setando o campo 'valor' como 'object'
                try
                {
                    // A data está no formato "MM/dd/yyyy HH:mm:ss"
                    var date = DateTime.ParseExact(data, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    data = date.ToString("yyyy-MM-ddTHH:mm"); // Formato aceito por input[datetime-local]
                    return data;
                }
                catch (FormatException ex2)
                {
                    return data;
                }
            }
        }

        private bool HasError(DynamicFieldRequest campo)
        {
            if (!_submitFoiChamado) return false;
            var context = new ValidationContext<DynamicTableRequest>(_registro);
            var result = new DynamicTableRequestValidator().Validate(context);
            var propertyName = $"Resultados[0].Valores[{_campos.IndexOf(campo)}].Valor";

            return result.Errors.Any(e => e.PropertyName == propertyName);
        }

        private string GetError(DynamicFieldRequest campo)
        {
            if (!_submitFoiChamado) return string.Empty;
            var context = new ValidationContext<DynamicTableRequest>(_registro);
            var result = new DynamicTableRequestValidator().Validate(context);
            var propertyName = $"Resultados[0].Valores[{_campos.IndexOf(campo)}].Valor";

            return result.Errors.FirstOrDefault(e => e.PropertyName == propertyName)?.ErrorMessage ?? "";
        }

        private async Task SalvarRegistro()
        {
            _submitFoiChamado = true;
            _loading = true;
            _loadingService.Show();
            if (!_fluentValidationValidator.Validate(options => options.IncludeAllRuleSets()))
            {
                _loadingService.Hide();
                _loading = false;
                return;
            }
            var payload = new DynamicTableRequest()
            {
                NomeTabela = _registro.NomeTabela,
                NomeTela = _registro.NomeTela,
                CampoPK = _registro.CampoPK,
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
            _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
            
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
