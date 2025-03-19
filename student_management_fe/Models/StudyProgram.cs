using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class StudyProgram
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên chương trình học không được để trống")]
    public string Name { get; set; }

}
