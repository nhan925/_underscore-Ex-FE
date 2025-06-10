using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class StudentStatus
{
    public int Id { get; set; }

    [Required(ErrorMessageResourceName = "student_status_model_name_required",
              ErrorMessageResourceType = typeof(Content))]
    public string Name { get; set; }
}
