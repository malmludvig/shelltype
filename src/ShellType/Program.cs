using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShellType;
using ShellType.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<BrowserStorage>();
builder.Services.AddSingleton<SettingsService>();
builder.Services.AddSingleton<HistoryService>();

await builder.Build().RunAsync();
