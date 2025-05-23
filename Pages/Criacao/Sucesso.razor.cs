﻿using MudBlazor;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Sucesso
    {
        private bool loading;
        private Breakpoint _breakpoint = Breakpoint.Xs;
        private string GetResponsivePadding() =>
            _breakpoint switch
            {
                Breakpoint.Xs => "pt-3 pb-3",
                Breakpoint.Sm => "pa-10",
                Breakpoint.Md => "pa-15",
                _ => "pa-20"
            };

        protected override async Task OnInitializedAsync()
        {
            _breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
            // Implementar logica para limpar do LocalStorage os itens "StorageConstants.Local.EmailCadastro" e "StorageConstants.Local.CodeTwoFactor"
        }
    }
}
