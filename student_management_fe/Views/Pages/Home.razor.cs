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
using System.Threading.Tasks;


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
    private readonly ConfigurationsService _configService;

    public Home(StudentServices studentServices, FacultyService facultyService, StudentStatusService studentStatusService, StudyProgramService studyProgramService, ConfigurationsService configService)
    {
        _studentServices = studentServices;
        _facultyService = facultyService;
        _studentStatusService = studentStatusService;
        _studyProgramService = studyProgramService;
        _configService = configService;
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

    private async Task LoadStudents()
    {
        var result = await _studentServices.GetAllStudents(currentPage, pageSize, searchText, 
            new StudentFilter { FacultyIds =
                faculties
                    .Where(faculty => selectedFaculties.Contains(faculty.Name))
                    .Select(faculty => faculty.Id)
                    .ToList()
            });
        students = result.Items;
        totalCount = result.TotalCount;
        
        if(!selectedFaculties.Any())
        {
            showFilter = false;
        }
    }

    private async Task SearchStudents()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            searchText = null;
        }

        await ResetPaging(keepFilter: true, keepSearch: true);
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
            Width = "90%",
            Height = "90%",
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
                await ResetPaging();
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
        var studentStatusesValid = student != null
                              ? await _configService.GetNextStatuses(student.StatusId)
                              : new List<StudentStatus>();
        studentStatusesValid.Add(studentStatuses.FirstOrDefault(x => x.Id == student.StatusId));

        var options = new Radzen.DialogOptions()
        {
            Resizable = false,
            Draggable = false,
            Width = "90%",
            Height = "90%",
            ContentCssClass = "custom-dialog"
        };

        var parameters = new Dictionary<string, object>
        {
            { "ButtonText", "Cập nhật" },
            { "Student", student },
            { "Faculties", faculties },
            { "StudentStatuses", studentStatusesValid },
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
                await ResetPaging();
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
                await ResetPaging();
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
        DialogService.Close();

        var parameters = new Dictionary<string, object>
        {
            { "AllowedExtensions", new string[] { format } }
        };

        var result = await DialogService.OpenAsync<UploadFile>(
            $"Thêm sinh viên từ file {GetFileFormat(format).ToUpperInvariant()}",
            parameters,
            new Radzen.DialogOptions() { Width = "40%", CloseDialogOnOverlayClick = false }
        );

        if (result is IBrowserFile file && file != null)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var sendFormat = GetFileFormat(format);
                await _studentServices.UploadFiles(file, sendFormat);
                await ResetPaging();
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

    private async Task ToggleFilter()
    {
        showFilter = !showFilter;

        if (!showFilter && selectedFaculties.Any()) { await ResetPaging(keepSearch: true); }
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

    private async Task ResetPaging(bool keepSearch = false, bool keepFilter = false)
    {
        currentPage = 1;

        if (!keepSearch) { searchText = null; }
        
        if (!keepFilter) 
        {
            selectedFaculties = new HashSet<string>();
            showFilter = false; 
        }

        await LoadStudents();
    }
}
