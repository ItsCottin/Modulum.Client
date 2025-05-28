using MudBlazor;

namespace Modulum.Client.Pages.Authentication.Recuperacao
{
    public partial class Concluido
    {
        private bool loading;
        private Breakpoint _breakpoint = Breakpoint.Xs;
        private string GetResponsivePadding() =>
            _breakpoint switch
            {
                Breakpoint.Xs => "pt-3 pb-3",
                Breakpoint.Sm => "pa-12",
                Breakpoint.Md => "pa-15",
                _ => "pa-18"
            };

        protected override async Task OnInitializedAsync()
        {
            _loadingService.Show();
            _breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
            // Implementar logica para limpar do LocalStorage os itens "StorageConstants.Local.EmailCadastro" e "StorageConstants.Local.CodeTwoFactor"
            _loadingService.Hide();
        }
    }
}
