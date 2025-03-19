using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;
using static MudBlazor.CategoryTypes;
using static student_management_fe.Views.Pages.Home;
using static student_management_fe.Views.Shared.StudentForm;
using Radzen;
using Microsoft.AspNetCore.Components.Forms;


namespace student_management_fe.Views.Pages;

public partial class Home
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    [Inject]
    private MudBlazor.DialogService MudDialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private string? searchText;
    public bool? IsConfirmed { get; set; }

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

    private bool showFilter = false;
    private IEnumerable<String> selectedFaculties = new HashSet<String>();

    private List<Faculty> faculties = new List<Faculty>();
    private List<StudyProgram> studyPrograms = new List<StudyProgram>();
    private List<StudentStatus> studentStatuses = new List<StudentStatus>();
    private List<StudentHomePageModel> students = new List<StudentHomePageModel>();
    private StudentHomePageModel SelectedStudent { get; set; }


    private readonly FacultyService _facultyService;
    private readonly StudyProgramService _studyProgramService;
    private readonly StudentStatusService _studentStatusService;
    private readonly StudentServices _studentServices;
    public Home(StudentServices studentServices, FacultyService facultyService, StudentStatusService studentStatusService, StudyProgramService studyProgramService)
    {
        _studentServices = studentServices;
        _facultyService = facultyService;
        _studentStatusService = studentStatusService;
        _studyProgramService = studyProgramService;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStudents();
        faculties = await _facultyService.GetFaculties();
        studentStatuses = await _studentStatusService.GetStudentStatuses();
        studyPrograms = await _studyProgramService.GetPrograms();
    }

    private string ConvertIdToFacultyName(int? id)
    {
        var faculty = faculties.FirstOrDefault(x => x.Id == id);
        return faculty?.Name ?? "";
    }

    private string ConvertIdToStudyProgramName(int? id)
    {
        var program = studyPrograms.FirstOrDefault(x => x.Id == id);
        return program?.Name ?? "";

    }

    private string ConvertIdToStudentStatusName(int? id)
    {
        var studentStatus = studentStatuses.FirstOrDefault(x => x.Id == id);
        return studentStatus?.Name ?? "";
    }

    private async Task LoadStudents(string? search = null)
    {
        //Add API call to get students

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

        Console.WriteLine($"Search Text: {searchText}");
        Console.WriteLine($"Faculties Filter: {string.Join(",", selectedFaculties)}");

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
        var newStudent = new StudentModel
        {
            Addresses = new List<Address>(),
            IdentityInfo = new IdentityInfo()
        };
        var parameters = new Dictionary<string, object>
        {
            { "ButtonText", "Lưu" },
            { "Student", newStudent },
            { "Faculties", faculties },
            { "StudentStatuses", studentStatuses },
            { "StudyPrograms", studyPrograms   }
        };
        var result = await DialogService.OpenAsync<StudentForm>("Thêm sinh viên", parameters, options);
        if (result is bool isConfirmed && isConfirmed)
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
        var student = await _studentServices.GetStudentById(mssv);

        var options = new Radzen.DialogOptions()
        {
            Resizable = false,
            Draggable = false,
            Width = "90vw",
            Style = "max-width: 90%;",
            ContentCssClass = "custom-dialog"
        };
        var parameters = new Dictionary<string, object>
        {
            { "ButtonText", "Cập nhật" },
            { "Student", student },
            { "Faculties", faculties },
            { "StudentStatuses", studentStatuses },
            { "StudyPrograms", studyPrograms   },
            { "IsUpdateMode", true }
        };

        var result = await DialogService.OpenAsync<StudentForm>("Cập nhật thông tin sinh viên", parameters, options);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

            try
            {
                await _studentServices.UpdateStudent(student);
                await LoadStudents();
                Snackbar.Add($"Thay đổi thông tin thành công !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task DeleteStudent(string id)
    {
        var parameters = new Dictionary<string, object>
    {
        { "ContentText", "Bạn có chắc chắn muốn xóa không? Sau khi xóa không thể khôi phục!" },
        { "ButtonText", "Xóa" }
    };

        var result = await DialogService.OpenAsync<DeleteConfirmationDialog>(
            "Xác nhận xóa", parameters
        );

        Console.WriteLine($"Dialog result: {result}");

        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

            try
            {
                await _studentServices.DeleteStudent(id);
                currentPage = 1;
                await LoadStudents();
                Snackbar.Add("Xóa sinh viên thành công!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task ImportFile(string format)
    {
        DialogService.Close(); // Đóng dialog hiện tại nếu có

        var parameters = new Dictionary<string, object>
        {
            { "AllowedExtensions", new string[] { format } }
        };

        var result = await DialogService.OpenAsync<UploadFile>(
            $"Thêm sinh viên từ file {GetFileFormat(format)}",
            parameters,
            new Radzen.DialogOptions() { Width = "600px", CloseDialogOnOverlayClick = false }
        );

        if (result is IReadOnlyList<IBrowserFile> files && files.Any())
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                foreach (var file in files)
                {
                    var sendFormat = GetFileFormat(format);
                    await _studentServices.UploadFiles(file, sendFormat);
                }
                currentPage = 1;
                await LoadStudents();
                Snackbar.Add("Thêm sinh viên thành công!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private string GetFileFormat(string extension)
    {
        return extension.ToLower() switch
        {
            ".xlsx" => "excel",
            ".json" => "json",
            _ => "unknown" 
        };
    }


    private async Task ExportFile(string format)
    {
        try
        {
            var sendFormat = GetFileFormat(format);
            await _studentServices.DownloadFile(sendFormat);
            Snackbar.Add(" Xuất file thành công", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private void ToggleFilter()
    {
        showFilter = !showFilter;
    }


    private void RowClickEvent(TableRowClickEventArgs<StudentHomePageModel> tableRowClickEventArgs)
    {
        Console.WriteLine("Row Clicked");
        Console.WriteLine(tableRowClickEventArgs.Item.Id);
        // Open popup full information
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
