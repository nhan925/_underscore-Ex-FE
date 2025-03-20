using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class Faculty
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên khoa không được để trống")]
    public string Name { get; set; }
}
