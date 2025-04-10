namespace student_management_fe.Models;

public class Lecturer
{
    public string Id { get; set; }

    public string FullName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public int FacultyId { get; set; }

    public string IdWithName => $"{Id} - {FullName}";
}
