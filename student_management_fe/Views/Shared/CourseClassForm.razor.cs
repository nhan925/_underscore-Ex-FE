using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using student_management_fe.Resources;
using student_management_fe.Models;
using student_management_fe.Services;

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

    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    private bool popup = false;
    private bool ShowTimeError { get; set; } = false;

    private List<string> DateOfWeek { get; set; } 

    private readonly CourseClassService _courseClassService;
    private readonly IStringLocalizer<Content> _localizer;

    public CourseClassForm(CourseClassService courseClassService, IStringLocalizer<Content> localizer)
    {
        _courseClassService = courseClassService;
        _localizer = localizer;
        DateOfWeek = new List<string>
        {
            _localizer["monday"],
            _localizer["tuesday"],
            _localizer["wednesday"],
            _localizer["thursday"],
            _localizer["friday"],
            _localizer["saturday"],
            _localizer["sunday"]
        };
    }


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

    private async Task ValidateAndSubmit()
    {
        var result = String.Empty;
        try
        {
            if (!isValidTimeRange())
            {
                ShowTimeError = true;
                return;
            }

            ShowTimeError = false;
            courseClass.ConvertScheduleParsedToString();
            result = await _courseClassService.AddCourseClass(courseClass);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return;
        }
        OnSubmit(result);
    }

    private void OnSubmit(string result)
    {
        DialogService.Close(result);
    }

    private void InvalidSubmit()
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
