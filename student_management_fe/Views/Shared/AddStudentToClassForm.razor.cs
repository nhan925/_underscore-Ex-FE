using Microsoft.AspNetCore.Components;
using MudBlazor;
using Radzen;
using student_management_fe.Models;
using student_management_fe.Services;

namespace student_management_fe.Views.Shared;

public partial class AddStudentToClassForm
{
    private StudentModel _student = new StudentModel()
    {
        Addresses = new List<Address>(),
        IdentityInfo = new IdentityInfo()
    };
    public StudentModel Student
    {
        get => _student;
        set
        {
            _student = value;
            StateHasChanged(); 
        }
    }

    [Parameter] public string ButtonText { get; set; }

    [Parameter] public GetCourseClassResult CourseClass { get; set; }

    [Inject] public Radzen.DialogService DialogService { get; set; } = default!;

    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    private List<Faculty> faculties = new();
    private List<StudentStatus> studentStatuses = new();
    private List<StudyProgram> studyPrograms = new();

    private readonly FacultyService _facultyService;
    private readonly StudyProgramService _studyProgramService;
    private readonly StudentStatusService _studentStatusService;
    private readonly StudentServices _studentService;
    private readonly CourseEnrollmentService _courseEnrollmentService;

    public bool popup = false;

    public AddStudentToClassForm(FacultyService facultyService, StudyProgramService studyProgramService, StudentStatusService studentStatusService, StudentServices studentServices, CourseEnrollmentService courseEnrollmentService)
    {
        _facultyService = facultyService;
        _studyProgramService = studyProgramService;
        _studentStatusService = studentStatusService;
        _studentService = studentServices;
        _courseEnrollmentService = courseEnrollmentService;
    }

    protected override async Task OnInitializedAsync()
    {
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

    private async Task OnStudentIdChange()
    {
        if (Student.Id != null)
        {
            var student = await _studentService.GetStudentById(Student.Id);
            if (student != null)
            {
                Student = student;
            }
        }
    }

    private async Task ValidateAndSubmit()
    {
        try
        {
            var result = await _courseEnrollmentService.RegisterAndUnregisterClass("register", new CourseEnrollmentRequest
            {
                StudentId = Student.Id,
                ClassId = CourseClass.Id,
                CourseId = CourseClass.Course.Id,
                SemesterId = CourseClass.Semester.Id
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Snackbar.Add(e.Message, Severity.Error);
            return;
        }
        OnSubmit();
    }

    void OnSubmit()
    {
        DialogService.Close(true);
    }

    private async Task OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
