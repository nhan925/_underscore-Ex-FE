using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using student_management_fe.Localization;
using student_management_fe.Models;
using student_management_fe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace student_management_fe.Views.Shared;
public partial class CourseForm
{
    [Parameter] public bool IsUpdateMode { get; set; } = false;
    [Parameter] public CourseModel Course { get; set; } = new();
    [Parameter] public string ButtonText { get; set; } = string.Empty;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private IStringLocalizer<Content> _localizer { get; set; }
    private int SelectedFacultyId { get; set; }
    private IEnumerable<string> SelectedPrerequisiteIds { get; set; } = new List<string>();
    bool popup = false;
    private readonly CourseService _courseService;
    private readonly FacultyService _facultyService;
    private List<CourseModel> CoursePrerequisites { get; set; } = new();
    private List<Faculty> Faculties { get; set; } = new();
    private bool HasEnrolledStudents { get; set; } = false;

    public CourseForm(CourseService courseService, FacultyService facultyService)
    {
        _courseService = courseService;
        _facultyService = facultyService;
    }

    protected override async Task OnInitializedAsync()
    {
        // Initialize ButtonText using _localizer after dependency injection is complete
        ButtonText = _localizer["all_actions_save_button_text"];

        // Initialize faculties and prerequisites
        await LoadFaculties();
        await LoadCoursePrerequisites();

        // If in update mode
        if (IsUpdateMode && Course != null)
        {
            // Display selected prerequisites
            if (Course.PrerequisitesId?.Any() == true)
            {
                SelectedPrerequisiteIds = Course.PrerequisitesId;
            }
            // Check if the course has enrolled students
            await CheckCourseEnrollmentStatus();
        }
    }

    private async Task CheckCourseEnrollmentStatus()
    {
        try
        {
            HasEnrolledStudents = await _courseService.CheckCourseHasStudents(Course.Id);
            if (HasEnrolledStudents)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = "Thông báo",
                    Detail = "Khóa học này đã có sinh viên đăng ký, một số thông tin sẽ không thể chỉnh sửa.",
                    Duration = 2000,
                    Style = "margin-bottom: 1rem; margin-right: 1rem; position: fixed; bottom: 0; right: 0;"
                });
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Lỗi",
                Detail = $"Không thể kiểm tra thông tin đăng ký: {ex.Message}",
                Duration = 2000,
                Style = "margin-bottom: 1rem; margin-right: 1rem; position: fixed; bottom: 0; right: 0;"
            });
        }
    }

    private async Task LoadFaculties()
    {
        Faculties = await _facultyService.GetFaculties();
    }

    private async Task LoadCoursePrerequisites()
    {
        var allCourses = await _courseService.GetAllCourses();
        // If in update mode, exclude the current course from prerequisites
        if (IsUpdateMode && !string.IsNullOrEmpty(Course.Id))
        {
            CoursePrerequisites = allCourses.Where(c => c.Id != Course.Id).ToList();
        }
        else
        {
            CoursePrerequisites = allCourses;
        }
    }

    private void ValidateAndSubmit()
    {
        // Assign PrerequisiteId to Course
        Course.PrerequisitesId = SelectedPrerequisiteIds.ToList();
        // Close dialog and return true (success)
        DialogService.Close(true);
    }

    private void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
