using ChatApp.Client;
using ChatApp.Client.Models;
using ChatApp.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

Usuario usuario = new Usuario();

builder.Services.AddSingleton(sp => usuario);
builder.Services.AddSingleton(sp => new Conexao(usuario));

await builder.Build().RunAsync();
