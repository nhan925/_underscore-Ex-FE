using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using student_management_fe.Models;
namespace student_management_fe.Pages
{
    public partial class Login
    {
        private LoginModel loginModel = new();
        private string errorMessage = "";

        private void HandleLogin()
        {
            if (string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
            {
                errorMessage = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }
            //if (loginModel.Username == "admin" && loginModel.Password == "123456")
            //{
            //    Navigation.NavigateTo("/");
            //}
            if(CheckLogin())
            {
                Navigation.NavigateTo("/");
            }
            else
            {
                errorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            }
        }

        private bool CheckLogin()
        {
            //Call api to check login
            return true;
        }

    }


}
