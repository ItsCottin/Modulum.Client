using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using MudBlazor;

namespace Modulum.Client.Pages.Criacao
{
    public partial class DialogDeleteComponent
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public string ContentText { get; set; }

        [Parameter]
        public string ButtonText { get; set; }

        [Parameter]
        public Color Color { get; set; }

        [Parameter]
        public int IdTable { get; set; }
        public bool _loading { get; set; } = false;

        private async Task Submit()
        {
            _loadingService.Show();
            _loading = true;
            if (IdTable == null)
            {
                _loadingService.Hide();
                _loading = false;
                MudDialog.Cancel();
                return;
            }
            var response = await _dynamicManager.DeleteMapTableAsync(IdTable);
            if (response == null)
            {
                _loadingService.Hide();
                _loading = false;
                MudDialog.Cancel();
                return;
            }
            else
            {
                _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
            }
            _loadingService.Hide();
            _loading = false;
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
