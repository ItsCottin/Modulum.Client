using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using MudBlazor;

namespace Modulum.Client.Pages.Dynamic
{
    public partial class Dynamic
    {
        [Parameter] public int TableId { get; set; }
        public DynamicTableRequest _dados { get; set; } = new();
        private bool _loading = false;
        private bool _loadingDados;

        protected override async Task OnInitializedAsync()
        {
            await CarregarDados();
        }

        protected override async Task OnParametersSetAsync()
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
            _loadingService.Show();
            _loadingDados = true;
            var response = await _dynamicManager.GetAllRegistroTabela(TableId);
            if (response.Succeeded)
            {
                _dados = response.Data;
            }
            _loadingDados = false;
            _loadingService.Hide();
        }

        private void AdicionarRegistro()
        {
            _navigationManager.NavigateTo($"/dynamic/{TableId}/insert");
        }

        private void VisualizarRegistro(int idRegistro)
        {
            _navigationManager.NavigateTo($"/dynamic/{TableId}/select/{idRegistro}");
        }

        private void EditarRegistro(int idRegistro)
        {
            _navigationManager.NavigateTo($"/dynamic/{TableId}/update/{idRegistro}");
        }


        private Task DeleteUserAsync(int idRegistro)
        {
            var parameters = new DialogParameters<DialogComponent>
            {
                { x => x.ContentText, "Deseja realmente excluir este registro? Este processo não pode ser desfeito." },
                { x => x.ButtonText, "Deletar" },
                { x => x.Color, Color.Error }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            return _dialogService.ShowAsync<DialogComponent>("Delete", parameters, options);
        }

        private async Task ExcluirRegistro(int idRegistro)
        {
            
            //bool confirm = await DialogService.ShowMessageBox(
            //    "Confirmação",
            //    "Tem certeza que deseja excluir este registro?",
            //    yesText: "Sim", noText: "Não"
            //);
            //
            //if (confirm)
            //{
            //    var registro = _dados.Resultados.FirstOrDefault(r => r.Id == idRegistro);
            //    if (registro != null)
            //    {
            //        var request = new DynamicTableRequest
            //        {
            //            NomeTabela = _dados.NomeTabela,
            //            NomeTela = _dados.NomeTela,
            //            CampoPK = _dados.CampoPK,
            //            JsonObject = _dados.JsonObject,
            //            TelaObject = _dados.TelaObject,
            //            Id = _dados.Id,
            //            Resultados = new List<DynamicDadoRequest> { registro }
            //        };
            //
            //        var response = await _dynamicManager.ExcluirRegistro(request);
            //        var response = await _dynamicManager.ExcluirRegistro(request);
            //
            //        if (response.Succeeded)
            //        {
            //            _dados.Resultados.Remove(registro);
            //            StateHasChanged();
            //            Snackbar.Add("Registro excluído com sucesso!", Severity.Success);
            //        }
            //        else
            //        {
            //            Snackbar.Add("Erro ao excluir registro.", Severity.Error);
            //        }
            //    }
            //}
        }
    }
}
