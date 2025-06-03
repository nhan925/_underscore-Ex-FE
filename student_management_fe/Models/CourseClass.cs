using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace student_management_fe.Models;

public class CourseClass
{
    [Required (ErrorMessageResourceName = "course_class_model_id_required",
               ErrorMessageResourceType = typeof(Content))]
    [StringLength(20, MinimumLength = 4, ErrorMessageResourceName = "course_class_model_id_length",
                                         ErrorMessageResourceType = typeof(Content))]
    public string? Id { get; set; }

    [Required (ErrorMessageResourceName = "course_class_model_courseid_required",
               ErrorMessageResourceType = typeof(Content))]
    public string? CourseId { get; set; }

    [Required]
    public int? SemesterId { get; set; }

    [Required(ErrorMessageResourceName = "course_class_model_lecturerid_required",
              ErrorMessageResourceType = typeof(Content))]
    public string? LecturerId { get; set; }

    [Required (ErrorMessageResourceName = "course_class_model_maxstudents_required",
               ErrorMessageResourceType = typeof(Content))]
    [Range(1, 500, ErrorMessageResourceName = "course_class_model_maxstudents_range",
                   ErrorMessageResourceType = typeof(Content))]
    public int? MaxStudents { get; set; }

    [Required (ErrorMessageResourceName = "course_class_model_schedule_required",
               ErrorMessageResourceType = typeof(Content))]
    public string? Schedule { get; set; }

    [Required (ErrorMessageResourceName = "course_class_model_room_required",
               ErrorMessageResourceType = typeof(Content))]
    [StringLength(20, MinimumLength = 4, ErrorMessageResourceName = "course_class_model_room_length",
                                         ErrorMessageResourceType = typeof(Content))]
    public string? Room { get; set; }


    [NotMapped]
    public ScheduleForCourseClass? ScheduleParsed { get; set; }

    public void ParseScheduleToStructuredObject()
    {
        if (string.IsNullOrWhiteSpace(Schedule))
        {
            ScheduleParsed = null;
            return;
        }

        try
        {
            var parts = Schedule.Split(',');

            if (parts.Length != 2) return;

            var dateOfWeek = parts[0].Trim(); 
            var timeParts = parts[1].Trim().Split('-');

            if (timeParts.Length != 2) return;

            var startTime = TimeSpan.Parse(timeParts[0].Trim()); 
            var endTime = TimeSpan.Parse(timeParts[1].Trim());   

            ScheduleParsed = new ScheduleForCourseClass
            {
                DateOfWeek = dateOfWeek,
                StartTime = startTime,
                EndTime = endTime
            };
        }
        catch
        {
            ScheduleParsed = null; 
        }
    }

    public void ConvertScheduleParsedToString()
    {
        if (ScheduleParsed == null ||
            string.IsNullOrWhiteSpace(ScheduleParsed.DateOfWeek) ||
            ScheduleParsed.StartTime == default ||
            ScheduleParsed.EndTime == default)
        {
            Schedule = null;
            return;
        }

        Schedule = $"{ScheduleParsed.DateOfWeek}, {ScheduleParsed.StartTime:hh\\:mm}-{ScheduleParsed.EndTime:hh\\:mm}";
    }
}
