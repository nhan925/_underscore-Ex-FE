using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace student_management_fe.Models;

public class CourseClass
{
    [Required (ErrorMessage = "Mã lớp học không được để trống")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Mã lớp học phải từ 4 đến 20 ký tự")]
    public string? Id { get; set; }

    [Required (ErrorMessage = "Khóa học không được để trống")]
    public string? CourseId { get; set; }

    [Required]
    public int? SemesterId { get; set; }

    [Required(ErrorMessage = "Giảng viên không được để trống")]
    public string? LecturerId { get; set; }

    [Required (ErrorMessage = "Số lượng sinh viên tối đa không được để trống")]
    [Range(1, 500, ErrorMessage = "Số lượng sinh viên phải từ 1 đến 500")]
    public int? MaxStudents { get; set; }

    [Required (ErrorMessage = "Lịch học không được để trống")]
    public string? Schedule { get; set; }

    [Required (ErrorMessage = "Phòng học không được để trống")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Phòng học phải từ 4 đến 20 ký tự")]
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
