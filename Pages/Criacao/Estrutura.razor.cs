using modulum.Application.Requests.Dynamic;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Estrutura
    {
        private bool _loading = false;
        private bool _loadingDados;
        private List<MenuRequest> _telas = new List<MenuRequest>();

        protected override async Task OnInitializedAsync()
        {
            await CarregarDados();
        }

        public async Task CarregarDados()
        {
            _loadingService.Show();
            _loadingDados = true;
            var response = await _dynamicManager.GetMenu();
            if (response.Succeeded)
            {
                _telas = response.Data;
            }
            _loadingDados = false;
            _loadingService.Hide();
        }

        private async Task DeleteMapTableAsync(int idRegistro)
        {
            var parameters = new DialogParameters<DialogDeleteComponent>
            {
                { x => x.ContentText, "Deseja realmente excluir esta Tela? Este processo não pode ser desfeito." },
                { x => x.ButtonText, "Deletar" },
                { x => x.Color, Color.Error },
                { x => x.IdTable, idRegistro }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large };

            var dialog = await _dialogService.ShowAsync<DialogDeleteComponent>("Delete", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                _loadingService.Show();
                _menuService.OnMenuChanged += ReloadMenus;
                var response = await _dynamicManager.GetMenu();
                if (response.Succeeded)
                {
                    _telas = response.Data;
                }

                // Correção para o cenario onde o usuario deletou a tela em que estava aberto
                if (_navigationManager.Uri.Contains($"/dynamic/{idRegistro}"))
                {
                    _navigationManager.NavigateTo("/");
                }
                _loadingService.Hide();
            }
        }

        private async void ReloadMenus()
        {
            _loadingService.Show();
            _telas.Clear();
            var response = await _dynamicManager.GetMenu();
            if (response.Succeeded)
            {
                _telas = response.Data;
            }
            _loading = true;
            _loadingService.Hide();
            StateHasChanged();
        }
    }
}
