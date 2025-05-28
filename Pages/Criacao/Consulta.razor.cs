using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using modulum.Application.Requests.Dynamic.Create;
using System.Text.Json;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Consulta
    {
        [Parameter] public int TableId { get; set; }
        private bool _loadingDados { get; set; } = false;

        private string? _json;

        private CreateDynamicTableRequest _mapTable = new();

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _loadingDados = true;

            var response = await _dynamicManager.GetMapTable(TableId);

            if (response.Succeeded)
            {
                _mapTable = response.Data;
                _json = JsonSerializer.Serialize(_mapTable, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            else
            {
                _snackBar.Add("Failed to load data.", MudBlazor.Severity.Error);
            }
            _loadingDados = false;
            _loadingService.Hide();
        }
    }
}
