using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class UpdateStudentGradeRequest
{
    [Required]
    public string StudentId { get; set; }

    [Required]
    public string CourseId { get; set; }

    public float? Grade { get; set; }
}
