using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using WASM.Client;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDIxNDQxQDMxMzkyZTMxMmUzMEZuWDBPTlBSWFBIK3hWZlZNWXlJRE9RZWZRLytHRkltK0cwd29ObGZNTFk9");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("WebUIAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebUIAPI"));

builder.Services.AddApiAuthorization();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddOptions();

await builder.Build().RunAsync();
