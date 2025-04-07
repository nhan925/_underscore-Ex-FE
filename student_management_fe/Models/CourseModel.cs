using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class CourseModel
{
    [Required(ErrorMessage = "Mã khóa học không được để trống")]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên khóa học không được để trống")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số tín chỉ không được để trống")]
    public int Credits { get; set; }

    [Required(ErrorMessage = "Khoa phụ trách không được để trống")]
    public int FacultyId { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<string> PrerequisitesId { get; set; } = new();

    public bool IsActive { get; set; }
}