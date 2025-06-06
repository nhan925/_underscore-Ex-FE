using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;
using Radzen;
using ServiceStack.Messaging;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Views.Pages.AcademicManagements;

public partial class CourseManagement
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private Radzen.DialogService DialogService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private int _currentPage = 1;
    private int currentPage
    {
        get => totalPages == 0 ? 1 : _currentPage;
        set
        {
            if (value > 0 && value <= totalPages)
                _currentPage = value;
        }
    }
    private int pageSize = 10;
    private int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    private int totalCount { get; set; } = 100;


    private string? searchText;
    private List<CourseModel> courses = new();
    private List<CourseModel> tempCourses = new();
    private List<Faculty> faculties = new();

    private readonly CourseService _courseService;
    private readonly FacultyService _facultyService;
    private readonly IStringLocalizer<Content> _localizer;


    public CourseManagement(
        CourseService courseService, 
        FacultyService facultyService, 
        IStringLocalizer<Content> localizer)
    {
        _courseService = courseService;
        _facultyService = facultyService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadFaculties();
        await LoadCourses();

        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
    }

    private async Task LoadFaculties()
    {
        faculties = await _facultyService.GetFaculties();
    }


    private async Task LoadCourses()
    {
        courses = await _courseService.GetAllCourses();
        tempCourses = courses;
    }

    private string GetFacultyName(int? id)
    {
        var faculty = faculties.FirstOrDefault(x => x.Id == id);
        return faculty?.Name ?? _localizer["course_management_undefined_noti"];
    }

    private void SearchCourse()
    {
        if (string.IsNullOrEmpty(searchText))
        {
            tempCourses = courses;
            searchText = null;
        }
        else
        {
            searchText = searchText.Trim();
            tempCourses = courses.Where(c =>
                c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                c.Id.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }

    private void HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            SearchCourse();
    }

    private Radzen.DialogOptions GetDefaultDialogOptions() => new()
    {
        Resizable = false,
        Draggable = false,
        Width = "70%",
        Height = "80%",
        ContentCssClass = "custom-dialog"
    };

    private async Task AddCourse()
    {
        var newCourse = new CourseModel();
        var options = GetDefaultDialogOptions();

        var parameters = new Dictionary<string, object>
        {
            ["IsUpdateMode"] = false,
            ["Course"] = newCourse,
            ["ButtonText"] = _localizer["all_actions_save_button_text"].Value
        };

        var result = await DialogService.OpenAsync<CourseForm>(
            _localizer["course_management_header_form_add"].Value,
            parameters,
            options
        );

        if (result is bool success && success)
        {
            try
            {
                var message = await _courseService.AddCourse(newCourse);
                await LoadCourses();
                Snackbar.Add(message, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task EditCourse(CourseModel courseModel)
    {
        var editCourse = new CourseModel
        {
            Id = courseModel.Id,
            Name = courseModel.Name,
            Credits = courseModel.Credits,
            FacultyId = courseModel.FacultyId,
            Description = courseModel.Description,
            PrerequisitesId = courseModel.PrerequisitesId?.ToList() ?? new List<string>()
        };

        var options = GetDefaultDialogOptions();

        var parameters = new Dictionary<string, object>
        {
            ["IsUpdateMode"] = true,
            ["Course"] = editCourse,
            ["ButtonText"] = _localizer["all_actions_save_button_text"].Value
        };

        var result = await DialogService.OpenAsync<CourseForm>(
            _localizer["course_management_header_form_update"].Value,
            parameters,
            options
        );

        if (result is bool success && success)
        {
            try
            {
                var message = await _courseService.UpdateCourse(editCourse);
                Snackbar.Add(message, Severity.Success);
                await LoadCourses();
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task DeleteCourse(CourseModel course)
    {
        var parameters = new Dictionary<string, object>
        {
            { "ContentText", $"{_localizer["course_management_delete_course_confirmation_content"].Value} {course.Name}" },
            { "ButtonText", _localizer["all_actions_delete_button_text"].Value }
        };

        var result = await DialogService.OpenAsync<DeleteConfirmationDialog>(
            _localizer["delete_confirmation_dialog_header"], parameters
        );

        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var message = await _courseService.DeleteCourse(course.Id);
                Snackbar.Add(message, Severity.Success);
                await LoadCourses();
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }
}