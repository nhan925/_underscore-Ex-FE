using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;
using static student_management_fe.Views.Pages.Home;
using static student_management_fe.Views.Shared.StudentForm;
using Radzen;


namespace student_management_fe.Views.Pages;

public partial class Home
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private string? searchText;
    private List<StudentModel> students = new List<StudentModel>();

    private int _currentPage = 1;
    private int currentPage
    {
        get
        {
            if (totalPages == 0)
            {
                return 1;
            }

            return _currentPage;
        }

        set
        {
            if (value > 0 && value <= totalPages)
            {
                _currentPage = value;
            }
        }
    }
    private int pageSize = 10;
    private int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    private int totalCount { get; set; } = 100;

    private List<Faculty> faculties = new List<Faculty>();
    private List<StudentStatus> studentStatuses = new List<StudentStatus>();
    private List<ProgramModel> programs = new List<ProgramModel>();

    private readonly StudentServices _studentServices;
    private readonly FacultyService _facultyService;
    private readonly StudentStatusService _studentStatusService;
    private readonly ProgramService _programService;
    public Home(StudentServices studentServices, FacultyService facultyService, StudentStatusService studentStatusService, ProgramService programService)
    {
        _studentServices = studentServices;
        _facultyService = facultyService;
        _studentStatusService = studentStatusService;
        _programService = programService;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStudents();
        faculties = await _facultyService.GetFaculties();
        studentStatuses = await _studentStatusService.GetStudentStatuses();
        //programs = await _programService.GetPrograms(); 


    }

    private string ConvertIdToFacultyName(int? id)
    {
        var faculty = faculties.FirstOrDefault(x => x.Id == id);
        return faculty?.Name ?? "";
    }

    private string ConvertIdToStudentStatusName(int? id)
    {
        var studentStatus = studentStatuses.FirstOrDefault(x => x.Id == id);
        return studentStatus?.Name ?? "";
    }

    private async Task LoadStudents(string? search = null)
    {
        var result = await _studentServices.GetAllStudents(currentPage, pageSize, search);
        students = result.Items;
        totalCount = result.TotalCount;
    }

    private async Task SearchStudents()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            searchText = null;
        }

        currentPage = 1;
        await LoadStudents(searchText);
    }

    private async Task HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchStudents();
        }
    }

    private async Task AddStudent()
    {

        var options = new Radzen.DialogOptions(){
            Resizable = false,
            Draggable = false,
            Width = "90vw", 
            Style = "max-width: 90%;",
            ContentCssClass= "custom-dialog"
        };
        var newStudent = new StudentModel(); 
        var parameters = new Dictionary<string, object>
        {
            { "ButtonText", "Lưu" },
            { "Student", newStudent },
            { "Faculties", faculties },
            { "StudentStatuses", studentStatuses },
            //{ "Programs", programs   }
        };
        var result = await DialogService.OpenAsync<StudentForm>("Thêm sinh viên", parameters, options);
        if (result)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

            try
            {
                var studentId = await _studentServices.AddStudent(newStudent);
                currentPage = 1;
                await LoadStudents();
                Snackbar.Add($"Đã thêm sinh viên với MSSV {studentId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

        Console.WriteLine($"Dialog closed with result: {result}");
    }

    private async Task EditStudent(string mssv)
    {
        //var student = await _studentServices.GetStudentById(mssv);
        //var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        //var parameters = new DialogParameters<StudentForm>
        //{
        //    { x => x.ButtonText, "Thay đổi" },
        //    { x => x.Student, student},
        //    { x => x.Faculties, faculties},
        //    { x => x.StudentStatuses, studentStatuses}
        //};

        //var dialog = await DialogService.ShowAsync<StudentForm>("Thay đổi thông tin", parameters, options);
        //var result = await dialog.Result;

        //if (!result.Canceled)
        //{
        //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

        //    try
        //    {
        //        await _studentServices.UpdateStudent(student);
        //        await LoadStudents();
        //        Snackbar.Add($"Thay đổi thông tin thành công !", Severity.Success);
        //    }
        //    catch (Exception ex)
        //    {
        //        Snackbar.Add(ex.Message, Severity.Error);
        //    }
        //}
    }

    private async Task DeleteStudent(string id)
    {
        //var parameters = new DialogParameters<DeleteConfirmationDialog>
        //{
        //    { x => x.ContentText, "Bạn có chắc chắn muốn xóa không? Sau khi xóa không thể khôi phục!" },
        //    { x => x.ButtonText, "Xóa" },
        //    { x => x.Color, Color.Error }
        //};

        //var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        //var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Xác nhận xóa", parameters, options);
        //var result = await dialog.Result;

        //if (!result.Canceled)
        //{
        //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

        //    try
        //    {
        //        await _studentServices.DeleteStudent(id);
        //        currentPage = 1;
        //        await LoadStudents();
        //        Snackbar.Add("Xóa sinh viên thành công!", Severity.Success);
        //    }
        //    catch (Exception ex)
        //    {
        //        Snackbar.Add(ex.Message, Severity.Error);
        //    }
        //}
    }


    private async Task NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            await LoadStudents();
        }
    }

    private async Task PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            await LoadStudents();
        }
    }
}
