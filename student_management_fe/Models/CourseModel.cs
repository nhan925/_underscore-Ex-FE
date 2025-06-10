using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using student_management_fe.Resources;

namespace student_management_fe.Models;

public class CourseModel
{
    [Required(ErrorMessageResourceName = "course_model_id_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "course_model_id_length",
                       ErrorMessageResourceType = typeof(Content))]
    public string Id { get; set; } = string.Empty;
    
    [Required(ErrorMessageResourceName = "course_model_name_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "course_model_name_length",
                       ErrorMessageResourceType = typeof(Content))]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessageResourceName = "course_model_credits_required",
              ErrorMessageResourceType = typeof(Content))]
    [Range(3, int.MaxValue, ErrorMessageResourceName = "course_model_credits_range",
                            ErrorMessageResourceType = typeof(Content))]
    public int Credits { get; set; }
    
    [Required(ErrorMessageResourceName = "course_model_facultyid_required",
              ErrorMessageResourceType = typeof(Content))]
    public int? FacultyId { get; set; }
    
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<string> PrerequisitesId { get; set; } = new();

    public bool IsActive { get; set; }

    public string IdWithName => $"{Id} - {Name}";
}