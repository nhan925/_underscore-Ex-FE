using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class Faculty
{
    public int Id { get; set; }

    [Required(ErrorMessageResourceName = "faculty_model_name_required",
              ErrorMessageResourceType = typeof(Content))]
    public string Name { get; set; }
}
