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

namespace student_management_fe.Views.Pages;

public partial class Home
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private string? searchText;


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
    private List<StudyProgram> programs = new List<StudyProgram>();
    private List<StudentStatus> studentStatuses = new List<StudentStatus>();
    private List<StudentHomePageModel> students = new List<StudentHomePageModel>();
    private StudentHomePageModel SelectedStudent { get; set; }

    private readonly FacultyService _facultyService;
    private readonly StudyProgramService _programService;
    private readonly StudentStatusService _studentStatusService;
    private readonly StudentServices _studentServices;

    public Home(StudentServices studentServices, FacultyService facultyService, StudentStatusService studentStatusService, StudyProgramService studyProgramService)
    {
        _studentServices = studentServices;
        _facultyService = facultyService;
        _studentStatusService = studentStatusService;
        _programService = studyProgramService;
    }

    protected override async Task OnInitializedAsync()
    {
        await GenerateMockStudents(30);
        //await LoadStudents();
        //faculties = await _facultyService.GetFaculties();
        //studentStatuses = await _studentStatusService.GetStudentStatuses();

    }

    private string ConvertIdToFacultyName(int? id)
    {
        var faculty = faculties.FirstOrDefault(x => x.Id == id);
        return faculty?.Name ?? "";
    }

    private string ConvertIdToStudyProgramName(int? id)
    {
        var program = programs.FirstOrDefault(x => x.Id == id);
        return program?.Name ?? "";

    }

    private string ConvertIdToStudentStatusName(int? id)
    {
        var studentStatus = studentStatuses.FirstOrDefault(x => x.Id == id);
        return studentStatus?.Name ?? "";
    }


    private async Task GenerateMockStudents(int n)
    {
        var random = new Random();

        // Dữ liệu mẫu cho faculty, program, status
        faculties = new List<Faculty>
        {
            new Faculty { Id = 1, Name = "Information Technology" },
            new Faculty { Id = 2, Name = "Business Administration" },
            new Faculty { Id = 3, Name = "Electrical Engineering" }
        };

        programs = new List<StudyProgram>
        {
            new StudyProgram { Id = 1, Name = "Computer Science" },
            new StudyProgram { Id = 2, Name = "Software Engineering" },
            new StudyProgram { Id = 3, Name = "Business Management" }
        };

        studentStatuses = new List<StudentStatus>
        {
            new StudentStatus { Id = 1, Name = "Active" },
            new StudentStatus { Id = 2, Name = "Graduated" },
            new StudentStatus { Id = 3, Name = "Dropped Out" }
        };

        students = new List<StudentHomePageModel>();

        for (int i = 0; i < n; i++)
        {
            var id = (22010000 + i + 1).ToString();
            var gender = random.Next(0, 2) == 0 ? "Male" : "Female";
            var dob = new DateTime(random.Next(1999, 2005), random.Next(1, 13), random.Next(1, 28));
            var facultyId = faculties[random.Next(faculties.Count)].Id;
            var programId = programs[random.Next(programs.Count)].Id;
            var statusId = studentStatuses[random.Next(studentStatuses.Count)].Id;

            students.Add(new StudentHomePageModel
            {
                Id = id,
                FullName = $"Student {i + 1}",
                DateOfBirth = dob,
                Gender = gender,
                FacultyId = facultyId,
                IntakeYear = random.Next(2018, 2024),
                ProgramId = programId,
                StatusId = statusId
            });
        }

        await Task.CompletedTask;
    }




    private async Task LoadStudents(string? search = null)
    {
        //Add API call to get students

        //var result = await _studentServices.GetAllStudents(currentPage, pageSize, search);
        //students = result.Items;
        //totalCount = result.TotalCount;


    }


    private async Task ExportToExcel()
    {

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
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var newStudent = new StudentModel();
        var parameters = new DialogParameters<StudentForm>
        {
            { x => x.ButtonText, "Thêm" },
            { x => x.Student, newStudent},
            { x => x.Faculties, faculties},
            { x => x.StudentStatuses, studentStatuses}
        };

        var dialog = await DialogService.ShowAsync<StudentForm>("Thêm sinh viên", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
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
    }

    private async Task ImportExcel()
    {
        Snackbar.Add("Import Excel clicked!", Severity.Info);

    }

    private async Task EditStudent(string mssv)
    {
        var student = await _studentServices.GetStudentById(mssv);
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var parameters = new DialogParameters<StudentForm>
        {
            { x => x.ButtonText, "Thay đổi" },
            { x => x.Student, student},
            { x => x.Faculties, faculties},
            { x => x.StudentStatuses, studentStatuses}
        };

        var dialog = await DialogService.ShowAsync<StudentForm>("Thay đổi thông tin", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
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
        var parameters = new DialogParameters<DeleteConfirmationDialog>
        {
            { x => x.ContentText, "Bạn có chắc chắn muốn xóa không? Sau khi xóa không thể khôi phục!" },
            { x => x.ButtonText, "Xóa" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Xác nhận xóa", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
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
