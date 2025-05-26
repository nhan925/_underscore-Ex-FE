using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Radzen;
using ServiceStack;
using student_management_fe.Authentication;
using student_management_fe.Services;
using Microsoft.AspNetCore.Mvc.Localization;
using student_management_fe.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using student_management_fe.Extensions;

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

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

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
        builder.Services.AddScoped<CourseClassService>();
        builder.Services.AddScoped<YearAndSemesterService>();
        builder.Services.AddScoped<CourseService>();
        builder.Services.AddScoped<LecturerService>();
        builder.Services.AddScoped<CourseEnrollmentService>();
        builder.Services.AddScoped<Radzen.DialogService>();
        builder.Services.AddScoped<MudBlazor.DialogService> ();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<DataService>();
        builder.Services.AddMudServices();
        builder.Services.AddRadzenComponents();

        var host = builder.Build();
        await host.SetDefaultCulture();

        await host.RunAsync();
    }
}
