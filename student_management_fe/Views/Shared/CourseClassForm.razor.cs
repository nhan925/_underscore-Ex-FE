using Microsoft.AspNetCore.Components;
using MudBlazor;
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

    //bool startTimeError = false;
    //string startTimeErrorText = "";

    //bool endTimeError = false;
    //string endTimeErrorText = "";

    //MudForm form;
    //bool isValid;
    //string[] errors = { };

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

    //private IEnumerable<string> ValidateStartAndEndTime(TimeSpan? startTime, TimeSpan? endTime)
    //{

    //    if (startTime == null)
    //    {
    //        startTimeError = true;
    //        startTimeErrorText = "Giờ bắt đầu không được để trống!";
    //    }
    //    else
    //    {
    //        startTimeError = false;
    //        startTimeErrorText = "";
    //    }

    //    if (endTime == null)
    //    {
    //        endTimeError = true;
    //        endTimeErrorText = "Giờ kết thúc không được để trống!";
    //    }
    //    else
    //    {
    //        endTimeError = false;
    //        endTimeErrorText = "";
    //    }

    //    if (startTime != null && endTime != null && startTime >= endTime)
    //    {
    //        startTimeError = true;
    //        startTimeErrorText = "Giờ bắt đầu phải nhỏ hơn giờ kết thúc!";
    //        endTimeError = true;
    //        endTimeErrorText = "Giờ kết thúc phải lớn hơn giờ bắt đầu!";
    //    }

    //    return errors;
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
