using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Radzen;
using student_management_fe.Authentication;
using student_management_fe.Services;

namespace student_management_fe;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var js = builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
        var apiBaseUrl = await js.InvokeAsync<string>("eval", "window.AppConfig.API_BASE_URL");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<StudentServices>();
        builder.Services.AddScoped<StudentStatusService>();
        builder.Services.AddScoped<StudyProgramService>();
        builder.Services.AddScoped<FacultyService>();
        builder.Services.AddScoped<ConfigurationsService>();
        builder.Services.AddScoped<CountryPhoneCodeService>();
        builder.Services.AddScoped<CourseService>();
        builder.Services.AddScoped<Radzen.DialogService>();
        builder.Services.AddScoped<MudBlazor.DialogService> ();
        builder.Services.AddMudServices();
        builder.Services.AddRadzenComponents();

        await builder.Build().RunAsync();
    }
}
