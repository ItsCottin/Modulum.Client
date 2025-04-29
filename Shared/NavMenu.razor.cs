using Microsoft.AspNetCore.Components.Routing;
using modulum.Application.Requests.Dynamic;
using modulum.Client.Infrastructure.Services;
using modulum.Shared;
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
    }
}
