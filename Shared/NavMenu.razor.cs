using Microsoft.AspNetCore.Components.Routing;
using modulum.Application.Requests.Dynamic;
using modulum.Application.Requests.Dynamic.Create;
using modulum.Client.Infrastructure.Services;
using modulum.Domain.Entities.DynamicEntity;
using modulum.Shared;
using Modulum.Client.Pages.Criacao;
using Modulum.Client.Pages.Dynamic;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace Modulum.Client.Shared
{
    public partial class NavMenu
    {
        private int margin = 10;
        private bool collapseNavMenu = true;
        private List<MenuRequest> _menus = new List<MenuRequest>();
        private bool _loading = false;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private string? currentUrl;

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _loading = false;
            currentUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
            _navigationManager.LocationChanged += OnLocationChanged;
            _menuService.OnMenuChanged += ReloadMenus;
            var response = await _dynamicManager.GetMenu();
            if ( response.Succeeded )
            {
                _menus = response.Data;
            }
            _loading = true;
            _loadingService.Hide();
        }

        private async void ReloadMenus()
        {
            _loadingService.Show();
            _menus.Clear();
            var response = await _dynamicManager.GetMenu();
            if (response.Succeeded)
            {
                _menus = response.Data;
            }
            _loading = true;
            _loadingService.Hide();
            StateHasChanged();
        }

        private async Task LoadMenus()
        {
            var response = await _dynamicManager.GetMenu();
            if (response.Succeeded)
            {
                _menus = response.Data;
            }
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = _navigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
            _menuService.OnMenuChanged -= ReloadMenus;
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
                    _menus = response.Data;
                }
                _loadingService.Hide();
            }
        }

        private async Task RenameTelaAsync(int idRegistro)
        {
            var menuSelecionado = _menus.FirstOrDefault(x => x.Id == idRegistro);
            var parameters2 = new DialogParameters<DialogRenameComponent>
            {
                { x => x.ContentText, "Deseja renomear sua tela ?" },
                { x => x.ButtonText, "Renomear" },
                { x => x.Color, Color.Info },
                { x => x._model, new RenameNomeTabelaTelaRequest { IdTabela = idRegistro, NovoNome = menuSelecionado.NomeTela } }
            };

            var options2 = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large };

            var dialog = await _dialogService.ShowAsync<DialogRenameComponent>("Renomear", parameters2, options2);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                _loadingService.Show();
                _menuService.OnMenuChanged += ReloadMenus;
                var response = await _dynamicManager.GetMenu();
                if (response.Succeeded)
                {
                    _menus = response.Data;
                }
                _loadingService.Hide();
            }
        }
    }
}
