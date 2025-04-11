using Microsoft.AspNetCore.Components;
using Radzen;
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
    [Parameter] public string ButtonText { get; set; } = "Lưu";
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

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
        // Khởi tạo danh sách khoa
        await LoadFaculties();
        // Khởi tạo danh sách khóa học tiên quyết
        await LoadCoursePrerequisites();
        // Nếu là update mode
        if (IsUpdateMode && Course != null)
        {
            // Hiển thị các khóa học tiên quyết đã chọn
            if (Course.PrerequisitesId?.Any() == true)
            {
                SelectedPrerequisiteIds = Course.PrerequisitesId;
            }
            // Kiểm tra xem khóa học có sinh viên đăng ký không
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
                NotificationService.Notify(NotificationSeverity.Info,
                    "Thông báo",
                    "Khóa học này đã có sinh viên đăng ký, một số thông tin sẽ không thể chỉnh sửa.");
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error,
                "Lỗi",
                $"Không thể kiểm tra thông tin đăng ký: {ex.Message}");
        }
    }

    private async Task LoadFaculties()
    {
        Faculties = await _facultyService.GetFaculties();
    }

    private async Task LoadCoursePrerequisites()
    {
        var allCourses = await _courseService.GetAllCourses();
        // Nếu đang ở chế độ cập nhật, loại bỏ khóa học hiện tại khỏi danh sách khóa học tiên quyết
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
        // Gán lại PrerequisiteId cho Course
        Course.PrerequisitesId = SelectedPrerequisiteIds.ToList();
        // Đóng dialog và trả về true (success)
        DialogService.Close(true);
    }

    private void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}