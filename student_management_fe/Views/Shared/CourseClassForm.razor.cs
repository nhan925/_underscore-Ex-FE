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
    private TimeSpan startTime = new TimeSpan(0, 0, 0);

    private async Task ValidateAndSubmit()
    {
        OnSubmit();
    }

    private void OnSubmit()
    {
        DialogService.Close(true);
    }

    private void Cancel() => DialogService.Close(false);
}
