namespace student_management_fe.Models
{
    public class StudentModel
    {
        public int StudentId { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public int? FacultyId { get; set; }
        public int? IntakeYear { get; set; }
        public string? Program { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? StatusId { get; set; }
      
        public StudentModel()
        {

        }
        public StudentModel(StudentModel s)
        {
            StudentId = s.StudentId;
            FullName = s.FullName;
            BirthDate = s.BirthDate;
            Gender = s.Gender;
            Faculty = s.Faculty;
            IntakeYear = s.IntakeYear;
            Program = s.Program;
            Address = s.Address;
            Email = s.Email;
            PhoneNumber = s.PhoneNumber;
            Status = s.Status;
        }
    }
}

