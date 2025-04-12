using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class ScheduleForCourseClass
{
    [Required (ErrorMessage = "Thứ không được để trống")]
    public string DateOfWeek { get; set; } = string.Empty;

    [Required (ErrorMessage = "Thời gian bắt đầu không được để trống")]
    public TimeSpan? StartTime { get; set; }

    [Required (ErrorMessage = "Thời gian kết thúc không được để trống")]
    public TimeSpan? EndTime { get; set; }
}