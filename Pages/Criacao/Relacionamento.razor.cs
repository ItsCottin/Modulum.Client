using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using modulum.Application.Requests.Dynamic.Create;
using modulum.Application.Requests.Dynamic.Relationship;
using modulum.Shared;
using modulum.Shared.Enum;
using Modulum.Client.Pages.Dynamic;
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
        private List<MenuRequest> _todasTelas = new();
        private List<CreateDynamicRelationshipRequest> _relacionamentos = new();
        private CreateDynamicRelationshipRequest _novoRelacionamento = new();
        private CreateDynamicRelationshipRequest _backupRelacionamentoEdit = new();
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
            _novoRelacionamento = new() { TempId = Guid.NewGuid() };
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
                _todasTelas = response.Data;
                _telasDisponiveis = _todasTelas.Where(x => x.Id != TableId && !_relacionamentos.Any(r => r.TabelaOrigemId == x.Id || r.TabelaDestinoId == x.Id)).ToList();
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
            var index = _relacionamentos.FindIndex(r => r.Id == _novoRelacionamento.Id &&
                            (r.Id != 0 || r.TempId == _novoRelacionamento.TempId));
            if (index != -1)
            {
                _relacionamentos[index] = _novoRelacionamento;
            }
            else 
            {
                _novoRelacionamento.NomeTelaDestino = _mapTelaOrigem?.NomeTela;
                _relacionamentos.Add(_novoRelacionamento);
            }
            _campos = new();
            _novoRelacionamento = new() { TempId = Guid.NewGuid() };
            _backupRelacionamentoEdit = new();
            _telaSelecionada = null; 
            _campoSelecionado = null;
            _tipoRelacionamento = string.Empty;
            _telasDisponiveis = _todasTelas.Where(x => x.Id != TableId && !_relacionamentos.Any(r => r.TabelaOrigemId == x.Id || r.TabelaDestinoId == x.Id)).ToList();
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
                _novoRelacionamento.NomeTelaOrigem = _telaSelecionada.NomeTela;
                //_novoRelacionamento.CampoDestino = _mapTelaOrigem.CampoPK;
                _campos = response.Data.Campos.Where(x => !x.IsForeigeKey).ToList();
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
            _novoRelacionamento.Tipo = _tipoRelacionamento.Equals("tem_um") ? TypeRelationshipEnum.ManyToOne : TypeRelationshipEnum.OneToMany;
        }

        private void CancelarEdicao()
        {
            _campos = new();
            _novoRelacionamento = _backupRelacionamentoEdit;
            AdicionarRelacionamento();
            _novoRelacionamento = new() { TempId = Guid.NewGuid() };
            _telaSelecionada = null;
            _campoSelecionado = null;
            _tipoRelacionamento = string.Empty;
        }

        private async Task EditarRelacionamentoAsync(CreateDynamicRelationshipRequest item)
        {
            _telaSelecionada = null;
            _backupRelacionamentoEdit = new CreateDynamicRelationshipRequest 
            {
                Id = item.Id,
                CampoParaExibicaoRelacionamento = item.CampoParaExibicaoRelacionamento,
                CampoTelaParaExibicaoRelacionamento = item.CampoTelaParaExibicaoRelacionamento,
                NomeTelaOrigem = item.NomeTelaOrigem,
                NomeTelaDestino = item.NomeTelaDestino,
                TabelaDestinoId = item.TabelaDestinoId,
                TabelaOrigemId = item.TabelaOrigemId,
                Tipo = item.Tipo,
                CampoDestino = item.CampoDestino,
                CampoOrigem = item.CampoOrigem,
                TempId = item.TempId,
                IsObrigatorio = item.IsObrigatorio,
            };
            _novoRelacionamento = item;
            _telasDisponiveis = _todasTelas.Where(x => x.NomeTela.Equals(item.NomeTelaOrigem)).ToList();
            _telaSelecionada = _telasDisponiveis.FirstOrDefault(x => x.NomeTela.Equals(item.NomeTelaOrigem));
            _tipoRelacionamento = item.Tipo == TypeRelationshipEnum.ManyToOne ? "tem_um" : "tem_muitos";
            await SetTelaSelecionada();
            _campoSelecionado = _campos.FirstOrDefault(x => x.NomeCampoBase == item.CampoParaExibicaoRelacionamento);
        }

        private async Task RemoverRelacionamentoAsync(CreateDynamicRelationshipRequest item)
        {
            _loadingService.Show();
            if (item.Id > 0)
            {
                var parameters = new DialogParameters<DialogDeleteRelacionamentoComponent>
                {
                    { x => x.ContentText, "Deseja realmente excluir este registro? Este processo não pode ser desfeito." },
                    { x => x.ButtonText, "Deletar" },
                    { x => x.Color, Color.Error },
                    { x => x.Model, new DynamicForIdRequest { IdTable = TableId, IdRegistro = item.Id } }
                };

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

                var dialog = await _dialogService.ShowAsync<DialogDeleteRelacionamentoComponent>("Delete", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    _relacionamentos.Remove(item);
                }
            }
            else
            {
                _relacionamentos.Remove(item);
            }
            _telasDisponiveis = _todasTelas.Where(x => x.Id != TableId && !_relacionamentos.Any(r => r.TabelaOrigemId == x.Id || r.TabelaDestinoId == x.Id)).ToList();
            _loadingService.Hide();
        }

        private async Task SalvarRelacionamentos()
        {
            _loadingService.Show();
            _loading = true;
            var response = await _dynamicManager.AlterarRelacionamento(_relacionamentos);
            _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
            if(response.Succeeded)
            {
                _navigationManager.NavigateTo("/");
            }
            _loading = false;
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
