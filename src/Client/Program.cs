using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Syncfusion.Blazor;
using WASM.Client;
using WASM.Client.Data;
using WASM.Client.Shared;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTc0NzY0QDMxMzkyZTM0MmUzMFE3VCt2cjlReDNnTHM1Q3hQZ2lMOGxCRkpSMmpQaCtIQXdBY2hZaFhpdkU9");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

// Set the default culture of the application
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

// Get the modified culture from culture switcher
var host = builder.Build();
var jsInterop = host.Services.GetRequiredService<IJSRuntime>();

var result = await jsInterop.InvokeAsync<string>("cultureInfo.get");
if (result != null)
{
    // Set the culture from culture switcher
    var culture = new CultureInfo(result);
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}

builder.Services.AddSingleton<PowerPointService>();
builder.Services.AddSingleton<WordService>();
builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<ExcelService>();

builder.Services.AddHttpClient("WebUIAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebUIAPI"));

builder.Services.AddApiAuthorization();
//builder.Services.AddOptions();

await builder.Build().RunAsync();
