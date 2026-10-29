using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;
using Blazored.SessionStorage;
using GestionOrdinateurs.Client.Utils;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddBlazoredSessionStorage();


builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());


builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/api/");
});


builder.Services.AddScoped<JwtHandler>();


builder.Services.AddHttpClient("GestionApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/api/");
})
.AddHttpMessageHandler<JwtHandler>();

await builder.Build().RunAsync();

