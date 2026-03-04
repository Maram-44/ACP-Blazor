using ACP;
using ACP.Models.Customers;
using ACP.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<JwtHandler>();

builder.Services.AddHttpClient<AnimalOprationsServices>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<AnimalServices>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<MedicalCenterReservationService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<MedicalCenterService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<AccountService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<BasketService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<ProductService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<SubscriptionService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<NotificationClientService>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddHttpClient<CustomerProfile>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["url"]);
});

builder.Services.AddAuthorizationCore();

//builder.Services.AddAuthorizationCore(options =>
//{
//    // ÓíÇÓÉ ÊãäÚ ÇáÏÎæá ÅáÇ áãä Ãßãá ãáÝå ÇáÔÎÕí
//    options.AddPolicy("CompletedProfileOnly", policy =>
//        policy.RequireClaim("IsProfileCompleted", "true"));
//});

// ÅÚÏÇÏ ÇÊÕÇá SignalR
builder.Services.AddScoped(sp =>
{
    var navManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navManager.ToAbsoluteUri("/notificationHub"))
        .WithAutomaticReconnect()
        .Build();
});


builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
