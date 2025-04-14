using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class CourseModel
{
    [Required(ErrorMessage = "Mã khóa học không được để trống")]
    [StringLength(100, ErrorMessage = "Mã khoa không được dài quá.")]
    public string Id { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tên khóa học không được để trống")]
    [StringLength(100, ErrorMessage = "Tên khóa học không được dài quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Số tín chỉ không được để trống")]
    [Range(3, int.MaxValue, ErrorMessage = "Số tín chỉ phải lớn hơn 2")]
    public int Credits { get; set; }
    
    [Required(ErrorMessage = "Khoa phụ trách không được để trống")]
    public int? FacultyId { get; set; }
    
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<string> PrerequisitesId { get; set; } = new();

    public bool IsActive { get; set; }

    public string IdWithName => $"{Id} - {Name}";
}