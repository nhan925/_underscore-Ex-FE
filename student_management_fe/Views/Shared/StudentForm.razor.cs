using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using student_management_fe.Models;


namespace student_management_fe.Views.Shared;

public partial class StudentForm
{
    [Parameter] public StudentModel Student { get; set; } = new();
    //[Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public bool IsUpdateMode { get; set; } = false;

    [Parameter]
    public List<Faculty> Faculties { get; set; } = new();

    [Parameter]
    public List<StudentStatus> StudentStatuses { get; set; } = new();

    [Inject] private NavigationManager Navigation { get; set; }
    private string FullNameError = "";
    private string EmailError = "";
    private string PhoneError = "";
    private string BirthDateError = "";
    private string IntakeYearError = "";
    private bool IsInvalid = false;
   

    private Dictionary<string, string> ValidationErrors = new();

    private void ValidateField(string fieldName, object? value)
    {
        string errorMessage = "";

        switch (fieldName)
        {
            case nameof(Student.FullName):
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                    errorMessage = "Họ và tên không được để trống.";
                break;

            case nameof(Student.DateOfBirth):
                if (value == null)
                    errorMessage = "Ngày sinh không được để trống.";
                else if ((DateTime)value > DateTime.Now)
                    errorMessage = "Ngày sinh không thể lớn hơn ngày hiện tại.";
                break;

            case nameof(Student.IntakeYear):
                if (value == null)
                    errorMessage = "Khóa học không được để trống.";
                else if ((int)value > DateTime.Now.Year)
                    errorMessage = "Khóa học không thể lớn hơn năm hiện tại.";
                break;

            case nameof(Student.Address):
                if (value == null)
                    errorMessage = "Địa chỉ không được để trống.";
                break;

            case nameof(Student.Email):
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                    errorMessage = "Email không được để trống.";
                else if (!System.Text.RegularExpressions.Regex.IsMatch(value.ToString(),
                    @"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})$"))
                    errorMessage = "Email không hợp lệ.";
                break;

            case nameof(Student.PhoneNumber):
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                    errorMessage = "Số điện thoại không được để trống.";
                else if (!System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), @"^\d{10}$"))
                    errorMessage = "Số điện thoại phải có 10 chữ số.";
                break;

            case nameof(Student.Gender):
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                    errorMessage = "Giới tính không được để trống.";
                break;

            
            case nameof(Student.FacultyId):
                if (value == null)
                    errorMessage = "Khoa không được để trống.";
                break;

            
            case nameof(Student.Program):
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                    errorMessage = "Chương trình học không được để trống.";
                break;

            
            case nameof(Student.StatusId):
                if (value == null)
                    errorMessage = "Trạng thái sinh viên không được để trống.";
                break;
        }

        if (!string.IsNullOrEmpty(errorMessage))
            ValidationErrors[fieldName] = errorMessage;
        else
            ValidationErrors.Remove(fieldName);

        StateHasChanged();
    }


    private void ValidateAndSave()
    {
        ValidationErrors.Clear();

        ValidateField(nameof(Student.FullName), Student.FullName);
        ValidateField(nameof(Student.DateOfBirth), Student.DateOfBirth);
        ValidateField(nameof(Student.Address), Student.Address);
        ValidateField(nameof(Student.IntakeYear), Student.IntakeYear);
        ValidateField(nameof(Student.Email), Student.Email);
        ValidateField(nameof(Student.PhoneNumber), Student.PhoneNumber);
        ValidateField(nameof(Student.Gender), Student.Gender);
        ValidateField(nameof(Student.FacultyId), Student.FacultyId);
        ValidateField(nameof(Student.Program), Student.Program);
        ValidateField(nameof(Student.StatusId), Student.StatusId);

        if (ValidationErrors.Count > 0)
        {
            StateHasChanged();
            return;
        }

        //await OnSave.InvokeAsync();
    }

}
