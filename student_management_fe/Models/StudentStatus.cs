using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class StudentStatus
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên trạng thái sinh viên không được để trống")]
    public string Name { get; set; }
}
