namespace Modulum.Client.Pages.Account
{
    public partial class ContatoPerfil
    {
        private void EditarContato()
        {
            _navigationManager.NavigateTo("/account/manage/contato/edit/1");
        }

        private void RemoverContato()
        {
            
        }
        private bool _loading = false;
        private bool _loadingDados = false;
    }
}
