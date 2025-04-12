using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;
using System.Net.WebSockets;
using Blazored.LocalStorage;


namespace student_management_fe.Views.Pages.AcademicManagements;

public partial class CourseClassesManagement 
{
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    public Radzen.DialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private string? searchText;
    private Year selectedYear = new();
    private Semester selectedSemester = new();

    private List<GetCourseClassResult> courseClasses = new();
    private List<GetCourseClassResult> filteredCourseClasses = new();
    private List<Year> years = new();
    private List<Semester> semesters = new();

    private readonly CourseClassService _courseClassService;
    private readonly YearAndSemesterService _yearAndSemesterService;
    private readonly CourseService _courseService;
    private readonly LecturerService _lecturerService;
    private readonly DataService _dataService;

    public CourseClassesManagement(CourseClassService courseClassService, YearAndSemesterService yearAndSemesterService, CourseService courseService, LecturerService lecturerService, DataService dataService)
    {
        _courseClassService = courseClassService;
        _yearAndSemesterService = yearAndSemesterService;
        _courseService = courseService;
        _lecturerService = lecturerService;
        _dataService = dataService;
    }

    protected override async Task OnInitializedAsync()
    {
        years = await _yearAndSemesterService.GetAllYears();
       
        if (years.Any())
        {
            await OnSelectedYearChanged(years[0]);
        }
    }

    private async Task AddCourseClass()
    {
        var options = new Radzen.DialogOptions()
        {
            Resizable = false,
            Draggable = false,
            Width = "90%",
            Height = "70%",
            ContentCssClass = "custom-dialog"
        };

        var courses = await _courseService.GetAllCourses();
        var lecturers = await _lecturerService.GetAllLecturers();
        var courseClass = new CourseClass()
        {
            SemesterId = selectedSemester.Id,
            ScheduleParsed = new(),
            CourseId = null,
            LecturerId = null
        };
        var parameters = new Dictionary<string, object>
        {
            { "courseClass", courseClass },
            { "courses", courses },
            { "lecturers", lecturers },
            { "ButtonText", "Thêm lớp học" }
        };

        var result = await DialogService.OpenAsync<CourseClassForm>("Thêm lớp học", parameters, options);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

            try
            {
                var courseClassId = await _courseClassService.AddCourseClass(courseClass);
                await OnSelectedSemesterChanged(selectedSemester);
                Snackbar.Add($"Đã thêm lớp học với mã {courseClassId}!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private void SearchCourseClasses()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            filteredCourseClasses = courseClasses;
            searchText = null;
        }
        else
        {
            filteredCourseClasses = courseClasses.Where(s => s.Id.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                                             s.Course.Name.Contains(searchText,StringComparison.OrdinalIgnoreCase))
                                    .ToList();
        }
    }

    private void HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            SearchCourseClasses();
        }
    }

    private async Task OnSelectedSemesterChanged(Semester newValue)
    {
        selectedSemester = newValue;

        if (selectedSemester != null)
        {
            courseClasses = await _courseClassService.GetAllCourseClassesBySemester(selectedSemester.Id);
            filteredCourseClasses = courseClasses;
        }
    }

    private async Task OnSelectedYearChanged(Year newValue)
    {
        selectedYear = newValue;

        if (selectedYear != null)
        {
            semesters = await _yearAndSemesterService.GetSemestersByYear(newValue.Id);

            if (semesters.Any())
            {
                await OnSelectedSemesterChanged(semesters[0]);
            }
        }
    }

    private async void RowClickEvent(TableRowClickEventArgs<GetCourseClassResult> tableRowClickEventArgs)
    {
        GetCourseClassResult myObject = tableRowClickEventArgs.Item;
        _dataService.SetData(myObject);
        await LocalStorage.SetItemAsync("cachedCourseClassSelected", myObject);
        NavigationManager.NavigateTo("/student-registered");
    }
}
