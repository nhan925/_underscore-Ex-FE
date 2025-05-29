using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using student_management_fe.Models;
using student_management_fe.Services;
using System.Threading.Tasks;
namespace student_management_fe.Views.Pages;

public partial class Login
{
    private LoginModel loginModel = new();
    private string errorMessage = "";
    private readonly AuthService _authService;

    public Login(AuthService authService)
    {
        _authService = authService;
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
            errorMessage = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
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
