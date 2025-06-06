using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class LoginModel
{
    [Required(ErrorMessageResourceName = "login_model_username_required",
              ErrorMessageResourceType = typeof(Content))]
    public string Username { get; set; } = "";

    [Required(ErrorMessageResourceName = "login_model_password_required",
              ErrorMessageResourceType = typeof(Content))]
    public string Password { get; set; } = "";
}
