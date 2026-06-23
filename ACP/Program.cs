using ACP;
using ACP.Models.Customers;
using ACP.Services;
//  „ «·«” €‰«¡ ⁄‰ «·‹ LocalStorage ··√„«‰
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(options =>
{
    // ”Ì«”…  ›—÷ √‰ ÌﬂÊ‰ «·Õ”«» ﬂ«„·« ··œŒÊ·
    options.AddPolicy("CompletedProfileOnly", policy =>
        policy.RequireClaim("IsProfileCompleted", "true"));
});

// 1.  ”ÃÌ· «·‹ «·‹ Handler «·„—ﬂ“Ì
builder.Services.AddScoped<CookieAndTokenHandler>();

// 2.  ›⁄Ì· ‰Ÿ«„ «·’·«ÕÌ«  «·√”«”Ì ›Ì »·«“Ê—
builder.Services.AddAuthorizationCore();


// 2.  ›⁄Ì· ‰Ÿ«„ «·’·«ÕÌ«  «·√”«”Ì
builder.Services.AddAuthorizationCore();

// 3.  ÊÕÌœ ﬁ—«¡… «·—«»ÿ „‰ «·≈⁄œ«œ«  »Ê—  7073
var apiUrl = builder.Configuration["url"];

// 4.  ”ÃÌ· «·⁄„Ì· «·„Œ’’ ··‹ Auth ·ﬂÌ ·« Ì„— »«·‹ Handler ‰›”Â (Õ„«Ì… „‰ «·ﬂ—«‘)
builder.Services.AddHttpClient("AuthClient", client => { client.BaseAddress = new Uri(apiUrl); });

builder.Services.AddScoped<CustomAuthStateProvider>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var authHttpClient = clientFactory.CreateClient("AuthClient");
    return new CustomAuthStateProvider(authHttpClient);
});

// —»ÿ «·„“Êœ „⁄ «·‰Ÿ«„ «·«› —«÷Ì ·»·«“Ê—
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());



builder.Services.AddHttpClient<AnimalOprationsServices>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<AnimalServices>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
//builder.Services.AddHttpClient<MedicalCenterReservationService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<MedicalCenterService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<AccountService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<NotificationClientService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<CustomerService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<AnimalTransactionClientService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddHttpClient<BlogHttpService>((sp, client) => { client.BaseAddress = new Uri(apiUrl); }).AddHttpMessageHandler<CookieAndTokenHandler>();
builder.Services.AddScoped<UserSession>();


// 5. ≈⁄œ«œ « ’«· SignalR (»ﬁÌ ﬂ„« ÂÊ)
builder.Services.AddScoped(sp =>
{
    var navManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navManager.ToAbsoluteUri("/notificationHub"))
        .WithAutomaticReconnect()
        .Build();
});

await builder.Build().RunAsync();