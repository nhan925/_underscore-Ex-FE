using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;
using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;
using student_management_fe.Localization;


namespace student_management_fe.Views.Pages.AcademicManagements;

public partial class CourseClassesManagement 
{
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
    private readonly IStringLocalizer<Content> _localizer;

    private bool firstLoad = true;

    public CourseClassesManagement(
        CourseClassService courseClassService, 
        YearAndSemesterService yearAndSemesterService,
        CourseService courseService,
        LecturerService lecturerService,
        DataService dataService,
        IStringLocalizer<Content> localizer)
    {
        _courseClassService = courseClassService;
        _yearAndSemesterService = yearAndSemesterService;
        _courseService = courseService;
        _lecturerService = lecturerService;
        _dataService = dataService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        years = await _yearAndSemesterService.GetAllYears();
       
        if (years.Any())
        {
            await OnSelectedYearChanged(years.Last());
            firstLoad = false;
        }

        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
    }

    private async Task AddCourseClass()
    {
        var options = new Radzen.DialogOptions()
        {
            Resizable = false,
            Draggable = false,
            Width = "70%",
            Height = "70%",
            ContentCssClass = "custom-dialog"
        };

        var courses = await _courseService.GetAllCourses();
        var activeCourses = courses.Where(c => c.IsActive == true).ToList();
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
            { "courses", activeCourses },
            { "lecturers", lecturers },
            { "ButtonText", "Thêm lớp học" }
        };

        var result = await DialogService.OpenAsync<CourseClassForm>("Thêm lớp học", parameters, options);
        if (result is not null)
        {
            try
            {
                await OnSelectedSemesterChanged(selectedSemester);
                Snackbar.Add($"Đã thêm lớp học với mã {result}!", Severity.Success);
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
            filteredCourseClasses = courseClasses
            .Where(s => 
                s.Id.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                || s.Course.Name.Contains(searchText,StringComparison.OrdinalIgnoreCase)
            )
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
                var semester = firstLoad ? semesters.Last() : semesters.First();
                await OnSelectedSemesterChanged(semester);
            }
        }
    }

    private async Task RowClickEvent(TableRowClickEventArgs<GetCourseClassResult> tableRowClickEventArgs)
    {
        if (tableRowClickEventArgs.Item is not null) 
        {
            GetCourseClassResult myObject = tableRowClickEventArgs.Item;
            _dataService.SetData(myObject);
            await LocalStorage.SetItemAsync("cachedCourseClassSelected", myObject);
            NavigationManager.NavigateTo("/student-registered");
        }
        else
        {
            Snackbar.Add("Selected course class is null.", Severity.Warning); 
        }
    }
}
