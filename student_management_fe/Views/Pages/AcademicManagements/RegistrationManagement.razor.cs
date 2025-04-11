using student_management_fe.Models;
using student_management_fe.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Views.Shared;
using System.Net.WebSockets;

namespace student_management_fe.Views.Pages.AcademicManagements;

public partial class RegistrationManagement
{
    private GetCourseClassResult courseClass;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    private List<StudentInClass> studentsInClass = new();
    private List<Year> years = new();
    private string yearAndSemesterText = string.Empty;

    private readonly CourseClassService _courseClassService;
    private readonly DataService _dataService;
    private readonly YearAndSemesterService _yearAndSemesterService;

    public RegistrationManagement(CourseClassService courseClassService, DataService dataService, YearAndSemesterService yearAndSemesterService)
    {
        _courseClassService = courseClassService;
        _dataService = dataService;
        _yearAndSemesterService = yearAndSemesterService;
    }

    protected override async Task OnInitializedAsync()
    {
        courseClass = _dataService.GetData<GetCourseClassResult>();
        if (courseClass == null)
        {
            courseClass = await LocalStorage.GetItemAsync<GetCourseClassResult>("cachedCourseClassSelected");
        }

        years = await _yearAndSemesterService.GetAllYears();
        var yearSelected = years.FirstOrDefault(x => x.Id == courseClass.Semester.YearId);
        yearAndSemesterText = $"{yearSelected.Name}/{courseClass.Semester.SemesterNum}";
        studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
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
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
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

    //private async Task DeleteStudent(string id)
    //{
    //    var parameters = new Dictionary<string, object>
    //    {
    //        { "ContentText", "Bạn có chắc chắn muốn xóa không? Sau khi xóa không thể khôi phục!" },
    //        { "ButtonText", "Xóa" }
    //    };

    //    var result = await DialogService.OpenAsync<DeleteConfirmationDialog>(
    //        "Xác nhận xóa", parameters
    //    );

    //    Console.WriteLine($"Dialog result: {result}");

    //    if (result is bool isConfirmed && isConfirmed)
    //    {
    //        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

    //        try
    //        {
    //            await _studentServices.DeleteStudent(id);
    //            await ResetPaging();
    //            Snackbar.Add("Xóa sinh viên thành công!", Severity.Success);
    //        }
    //        catch (Exception ex)
    //        {
    //            Snackbar.Add(ex.Message, Severity.Error);
    //        }
    //    }
    //}
}