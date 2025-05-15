using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using modulum.Application.Requests.Dynamic.Create;
using modulum.Application.Requests.Dynamic.Relationship;
using modulum.Shared.Enum;
using MudBlazor;
using System.Threading.Tasks;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Relacionamento
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        [Parameter] public int TableId { get; set; }

        private bool _loadingDados = true;
        private MenuRequest _telaSelecionada;
        private CreateDynamicTableRequest _mapTelaOrigem = new();
        private string _tipoRelacionamento = string.Empty;
        private List<MenuRequest> _telasDisponiveis = new();
        private List<CreateDynamicRelationshipRequest> _relacionamentos = new();
        private CreateDynamicRelationshipRequest _novoRelacionamento = new();
        private bool _loading = false;
        private List<CreateDynamicFieldRequest> _campos = new();
        private CreateDynamicFieldRequest _campoSelecionado;
        private bool _loadingComboBoxCampo = false;


        protected override async Task OnParametersSetAsync()
        {
            await CarregarDados();
        }
        protected override async Task OnInitializedAsync()
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
            _relacionamentos = new();
            _novoRelacionamento = new();
            _telaSelecionada = null;
            _campoSelecionado = null;
            _tipoRelacionamento = string.Empty;
            _loadingService.Show();
            _loadingDados = true;
            var getMapTable = await _dynamicManager.GetMapTable(TableId);
            var relacionamentos = await _dynamicManager.ConsultarRelacionamento(TableId);
            if (relacionamentos.Succeeded)
            {
                if (relacionamentos.Data.Any())
                    _relacionamentos = relacionamentos.Data;
            }
            if (getMapTable != null && getMapTable.Succeeded)
            {
                _mapTelaOrigem = getMapTable.Data;
            }
            else
            {
                _snackBar.Add("Erro ao carregar dados", Severity.Error);
            }
            var response = await _dynamicManager.GetMenu();
            if (response != null && response.Succeeded)
            {
                _telasDisponiveis = response.Data.Where(x => x.Id != TableId && !_relacionamentos.Any(r => r.TabelaOrigemId == x.Id || r.TabelaDestinoId == x.Id)).ToList();
            }
            else
            {
                _snackBar.Add("Erro ao carregar dados", Severity.Error);
            }
            _loadingDados = false;
            _loadingService.Hide();
        }

        private void AdicionarRelacionamento()
        {
            _novoRelacionamento.NomeTelaOrigem = _mapTelaOrigem?.NomeTela;
            _relacionamentos.Add(_novoRelacionamento);

            _novoRelacionamento = new();

            _telasDisponiveis = _telasDisponiveis.Where(x => x.Id != TableId && !_relacionamentos.Any(r => r.TabelaOrigemId == x.Id || r.TabelaDestinoId == x.Id)).ToList();
        }

        public async Task SetTelaSelecionada()
        {
            _loadingComboBoxCampo = true;
            _campos = new();
            _campoSelecionado = null;
            var response = await _dynamicManager.GetMapTable(_telaSelecionada.Id);
            if (_telaSelecionada.Id != 0)
            {
                _novoRelacionamento.TabelaDestinoId = TableId;
                _novoRelacionamento.TabelaOrigemId = _telaSelecionada.Id; 
                _novoRelacionamento.NomeTelaDestino = _telaSelecionada.NomeTela;
                _novoRelacionamento.CampoDestino = _mapTelaOrigem.CampoPK;
                _campos = response.Data.Campos;
            }
            _loadingComboBoxCampo = false;
        }

        public void SetCampoSelecionada()
        {
            _novoRelacionamento.CampoParaExibicaoRelacionamento = _campoSelecionado.NomeCampoBase;
            _novoRelacionamento.CampoTelaParaExibicaoRelacionamento = _campoSelecionado.NomeCampoTela;
        }

        public void SetTipoRelacionamento()
        {
            _novoRelacionamento.Tipo = _tipoRelacionamento.Equals("tem_um") ? TypeRelationshipEnum.OneToOne : TypeRelationshipEnum.OneToMany;
        }

        private void RemoverRelacionamento(CreateDynamicRelationshipRequest item)
        {
            _relacionamentos.Remove(item);
        }

        private async Task SalvarRelacionamentos()
        {
            _loadingService.Show();
            var response = await _dynamicManager.AlterarRelacionamento(_relacionamentos);
            _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
            if(response.Succeeded)
            {
                _navigationManager.NavigateTo("/");
            }
            _loadingService.Hide();
        }

        private bool PodeAdicionar()
        {
            return _novoRelacionamento.TabelaDestinoId != 0
                && _novoRelacionamento.Tipo != null
                && _novoRelacionamento.CampoTelaParaExibicaoRelacionamento != null;
        }
    }
}
