using Microsoft.AspNetCore.Components;
using modulum.Application.Requests.Dynamic;
using MudBlazor;
using System.Threading.Tasks;

namespace Modulum.Client.Pages.Dynamic
{
    public partial class DialogComponent
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
        public DynamicForIdRequest Model { get; set; }

        private async Task Submit()
        {
            _loadingService.Show();
            if (Model == null)
            {
                _loadingService.Hide();
                MudDialog.Cancel();
                return;
            }
            var response = await _dynamicManager.DeletePorIdAsync(Model);
            if (response == null)
            {
                _loadingService.Hide();
                MudDialog.Cancel();
                return;
            }
            else
            {
                _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
            }
            _loadingService.Hide();
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
