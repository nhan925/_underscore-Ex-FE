using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class ScheduleForCourseClass
{
    [Required (ErrorMessageResourceName = "schedule_for_course_class_dateofweek_required",
               ErrorMessageResourceType = typeof(Content))]
    public string DateOfWeek { get; set; } = string.Empty;

    [Required (ErrorMessageResourceName = "schedule_for_course_class_starttime_required",
               ErrorMessageResourceType = typeof(Content))]
    public TimeSpan? StartTime { get; set; }

    [Required (ErrorMessageResourceName = "schedule_for_course_class_endtime_required",
               ErrorMessageResourceType = typeof(Content))]
    public TimeSpan? EndTime { get; set; }
}