using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using modulum_client;
using modulum_client.Sevices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddTransient<CutomHttpHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress =
    new Uri(builder.Configuration["FrontendUrl"] ?? "https://ashy-plant-0122bb30f.5.azurestaticapps.net")
    //new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:5002")
});


builder.Services.AddHttpClient("Auth", opt => opt.BaseAddress =
new Uri(builder.Configuration["BackendUrl"] ?? "https://modulum-api.azurewebsites.net/")).AddHttpMessageHandler<CutomHttpHandler>();
//new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:7239")).AddHttpMessageHandler<CutomHttpHandler>();

await builder.Build().RunAsync();
