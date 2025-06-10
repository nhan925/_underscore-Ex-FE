using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Radzen;
using student_management_fe.Authentication;
using student_management_fe.Services;
using student_management_fe.Resources;
using Microsoft.AspNetCore.Mvc.Razor;
using student_management_fe.Extensions;
using student_management_fe.Models;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

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

        builder.Services.AddLocalization();
        builder.Services.AddMvcCore()
                        .AddDataAnnotations()
                        .AddDataAnnotationsLocalization();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IStudentServices, StudentServices>();
        builder.Services.AddScoped<IStudentStatusService, StudentStatusService>();
        builder.Services.AddScoped<IStudyProgramService, StudyProgramService>();
        builder.Services.AddScoped<IFacultyService, FacultyService>();
        builder.Services.AddScoped<IConfigurationsService, ConfigurationsService>();
        builder.Services.AddScoped<ICountryPhoneCodeService, CountryPhoneCodeService>();
        builder.Services.AddScoped<ICourseClassService, CourseClassService>();
        builder.Services.AddScoped<IYearAndSemesterService, YearAndSemesterService>();
        builder.Services.AddScoped<ICourseService, CourseService>();
        builder.Services.AddScoped<ILecturerService, LecturerService>();
        builder.Services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();
        builder.Services.AddScoped<IDataService, DataService>();
        
        builder.Services.AddScoped<Radzen.DialogService>();
        builder.Services.AddScoped<MudBlazor.DialogService>();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMudServices();
        builder.Services.AddRadzenComponents();

        var host = builder.Build();
        await host.SetDefaultCulture();

        await host.RunAsync();
    }
}
