using student_management_fe.Models;
using student_management_fe.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages.AcademicManagements;

public partial class RegistrationManagement
{
    private GetCourseClassResult courseClass = new();

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    private List<StudentInClass> studentsInClass = new();
    private List<Year> years = new();
    private string yearAndSemesterText = string.Empty;
    private StudentInClass? editingStudent;
    private float? editingGrade;
    private float? originalGrade;
    private bool isLoading = true;

    private readonly CourseClassService _courseClassService;
    private readonly DataService _dataService;
    private readonly YearAndSemesterService _yearAndSemesterService;
    private readonly CourseEnrollmentService _courseErollmentService;

    public RegistrationManagement(
        CourseClassService courseClassService, 
        DataService dataService, 
        YearAndSemesterService yearAndSemesterService, 
        CourseEnrollmentService courseErollmentService)
    {
        _courseClassService = courseClassService;
        _dataService = dataService;
        _yearAndSemesterService = yearAndSemesterService;
        _courseErollmentService = courseErollmentService;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            courseClass = _dataService.GetData<GetCourseClassResult>();
            if (courseClass == null)
            {
                courseClass = await LocalStorage.GetItemAsync<GetCourseClassResult>("cachedCourseClassSelected");
                if (courseClass == null)
                {
                    Snackbar.Add("Không tìm thấy thông tin lớp học", Severity.Warning);
                    return;
                }
            }
            years = await _yearAndSemesterService.GetAllYears();
            if (years != null && courseClass.Semester != null)
            {
                var yearSelected = years.FirstOrDefault(x => x.Id == courseClass.Semester.YearId);
                if (yearSelected != null)
                {
                    yearAndSemesterText = $"{yearSelected.Name}/{courseClass.Semester.SemesterNum}";
                }
            }

            studentsInClass = new List<StudentInClass>();
            if (courseClass != null)
            {
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass) ?? new List<StudentInClass>();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Đã xảy ra lỗi: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task AddStudentToClass()
    {
        var options = new Radzen.DialogOptions()
        {
            Resizable = false,
            Draggable = false,
            Width = "45%",
            Height = "90%",
            ContentCssClass = "custom-dialog"
        };

        var result = await DialogService.OpenAsync<AddStudentToClassForm>("Thêm sinh viên vào lớp học", new Dictionary<string, object>
        {
            { "ButtonText", "Thêm sinh viên" },
            { "CourseClass", courseClass }
        }, options);

        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
                Snackbar.Add("Thêm sinh viên thành công!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task DeleteStudentFromClass(string id)
    {
        var parameters = new Dictionary<string, object>
        {
            { "ContentText", "Bạn có chắc chắn muốn hủy đăng ký không? Sau khi hủy đăng ký không thể khôi phục!" },
            { "ButtonText", "Hủy đăng ký" }
        };

        var resultVerify = await DialogService.OpenAsync<DeleteConfirmationDialog>(
            "Xác nhận hủy đăng ký", parameters
        );

        Console.WriteLine($"Dialog result: {resultVerify}");

        if (resultVerify is bool isConfirmed && isConfirmed)
        {
            try
            {
                var studentUnregistered = new CourseEnrollmentRequest
                {
                    StudentId = id,
                    ClassId = courseClass.Id,
                    CourseId = courseClass.Course.Id,
                    SemesterId = courseClass.Semester.Id
                };
                var result = await _courseErollmentService.RegisterAndUnregisterClass(CourseEnrollmentService.EnrollmentActions.Unregister, studentUnregistered);
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
                Snackbar.Add("Hủy đăng ký lớp cho sinh viên thành công!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private void StartEditingGrade(StudentInClass student)
    {
        editingStudent = student;
        editingGrade = student.Grade;
        originalGrade = student.Grade;
    }

    private async Task SaveGrade()
    {
        if (editingStudent != null)
        {
            try
            {
                editingStudent.Grade = editingGrade;
                var updateStudentGradeRequest = new UpdateStudentGradeRequest
                {
                    StudentId = editingStudent.Id,
                    CourseId = courseClass.Course.Id,
                    Grade = editingStudent.Grade
                };
                await _courseErollmentService.UpdateStudentGrade(updateStudentGradeRequest);
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
                Snackbar.Add($"Đã cập nhật điểm số của sinh viên có MSSV {editingStudent.Id}", Severity.Success);
            }
            catch (Exception ex)
            {
                if (editingStudent != null && originalGrade.HasValue)
                    editingStudent.Grade = originalGrade;

                Snackbar.Add($"Lỗi khi cập nhật điểm số: {ex.Message}", Severity.Error);
            }
            finally
            {
                editingStudent = null;
                editingGrade = null;
                originalGrade = null;
            }
        }
    }

    private void CancelEdit()
    {
        editingStudent = null;
        editingGrade = null;
        originalGrade = null;
    }
}