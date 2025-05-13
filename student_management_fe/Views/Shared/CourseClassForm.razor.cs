using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Models;

namespace student_management_fe.Views.Shared;

public partial class CourseClassForm
{
    [Parameter] public CourseClass courseClass { get; set; } = new CourseClass
    {
        ScheduleParsed = new ScheduleForCourseClass()
    };

    [Parameter] public List<CourseModel> courses { get; set; } = new();

    [Parameter] public List<Lecturer> lecturers { get; set; } = new();

    [Parameter] public string ButtonText { get; set; }

    [Inject] Radzen.DialogService DialogService { get; set; }

    private bool popup = false;
    private bool ShowTimeError { get; set; } = false;

    private List<string> DateOfWeek { get; set; } = new()
    {
        "Thứ 2",
        "Thứ 3",
        "Thứ 4",
        "Thứ 5",
        "Thứ 6",
        "Thứ 7",
        "Chủ nhật"
    };

    private bool isValidTimeRange()
    {
        bool isValid = true;

        if (courseClass.ScheduleParsed.StartTime == null)
        {
            isValid = false;
        }

        if (courseClass.ScheduleParsed.EndTime == null)
        {
            isValid = false;
        }

        if (courseClass.ScheduleParsed.StartTime != null &&
            courseClass.ScheduleParsed.EndTime != null &&
            courseClass.ScheduleParsed.StartTime > courseClass.ScheduleParsed.EndTime)
        {
            isValid = false;
        }

        return isValid;
    }

    private void OnSubmit()
    {
        if (!isValidTimeRange())
        {
            ShowTimeError = true;
            return;
        }

        ShowTimeError = false;
        courseClass.ConvertScheduleParsedToString();
        DialogService.Close(true);
    }

    private void InvalidSubmit()
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
