using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class CourseClass
{
    [Required (ErrorMessage = "Mã lớp học không được để trống")]
    public string? Id { get; set; }

    [Required (ErrorMessage = "Khóa học không được để trống")]
    public string? CourseId { get; set; }

    [Required]
    public int? SemesterId { get; set; }

    [Required(ErrorMessage = "Giảng viên không được để trống")]
    public string? LecturerId { get; set; }

    [Required (ErrorMessage = "Số lượng sinh viên tối đa không được để trống")]
    public int? MaxStudents { get; set; }

    [Required (ErrorMessage = "Lịch học không được để trống")]
    public string? Schedule { get; set; }

    [Required (ErrorMessage = "Phòng học không được để trống")]
    public string? Room { get; set; }
}
