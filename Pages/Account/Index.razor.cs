using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using modulum.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Modulum.Client.Pages.Account
{
    public partial class Index
    {
        private User user = default!;
        private string? username;
        private string? phoneNumber;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        //protected override async Task OnInitializedAsync()
        //{
        //    user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        //    username = await UserManager.GetUserNameAsync(user);
        //    phoneNumber = await UserManager.GetPhoneNumberAsync(user);

        //    Input.PhoneNumber ??= phoneNumber;
        //}

        private async Task OnValidSubmitAsync()
        {
            if (Input.PhoneNumber != phoneNumber)
            {
                //var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                //if (!setPhoneResult.Succeeded)
                //{
                //    RedirectManager.RedirectToCurrentPageWithStatus("Erro: Falha ao definir o número de telefone.", HttpContext);
                //}
            }

            //await SignInManager.RefreshSignInAsync(user);
            //RedirectManager.RedirectToCurrentPageWithStatus("Seu perfil foi atualizado", HttpContext);
        }

        private sealed class InputModel
        {
            [Phone]
            [Display(Name = "Número de telefone")]
            public string? PhoneNumber { get; set; }
        }
    }
}
