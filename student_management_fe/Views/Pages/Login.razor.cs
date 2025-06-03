using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using student_management_fe.Models;
using student_management_fe.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Views.Pages;

public partial class Login
{
    private LoginModel loginModel = new();
    private string errorMessage = "";
    private readonly AuthService _authService;
    private readonly IStringLocalizer<Content> _localizer;

    public Login(AuthService authService, IStringLocalizer<Content> localizer)
    {
        _authService = authService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var isAuthenticated = authState.User.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
        {
            Navigation.NavigateTo("/", true);
        }
    }

    private async Task HandleLogin()
    {
        if (string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
        {
            errorMessage = _localizer["login_username_password_required_error"];
            return;
        }

        try
        {
            await _authService.Login(loginModel);
            Navigation.NavigateTo("/");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }
}
