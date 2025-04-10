using Microsoft.AspNetCore.Components;
using student_management_fe.Models;

namespace student_management_fe.Views.Shared;

public partial class CourseClassForm
{
    [Parameter] public CourseClass courseClass { get; set; } = new();

    [Parameter] public List<CourseModel> courses { get; set; } = new();

    [Parameter] public List<Lecturer> lecturers { get; set; } = new();

    [Parameter] public string ButtonText { get; set; }

    [Inject] Radzen.DialogService DialogService { get; set; }

    private bool popup = false;
    private bool ShowStartTimeError { get; set; } = false;
    private bool ShowEndTimeError { get; set; } = false;

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

    //private void ValidateStartTime()
    //{
    //    TimeSpan? time = courseClass.ScheduleParsed.StartTime;
    //    bool isDefaultTime = time == null || time == default;
    //    bool isGreaterThanEndTime = !isDefaultTime &&
    //                                courseClass.ScheduleParsed.EndTime != default &&
    //                                time > courseClass.ScheduleParsed.EndTime;

    //    ShowStartTimeError = isDefaultTime || isGreaterThanEndTime;
    //}

    //private void ValidateEndTime()
    //{
    //    TimeSpan? time = courseClass.ScheduleParsed.EndTime;
    //    ShowEndTimeError = time == null || time == default;

    //    if (!ShowEndTimeError && courseClass.ScheduleParsed.StartTime != default)
    //    {
    //        ShowStartTimeError = courseClass.ScheduleParsed.StartTime > time;
    //    }
    //}

    private void OnSubmit()
    {
        if (courseClass.ScheduleParsed.StartTime == default || courseClass.ScheduleParsed.EndTime == default )
        {
            ShowStartTimeError = true;
            ShowEndTimeError = true;
            return;
        }

        ShowStartTimeError = false;
        ShowEndTimeError = false;
        courseClass.ConvertScheduleParsedToString();
        DialogService.Close(true);
    }

    private void InvalidSubmit()
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
