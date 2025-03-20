namespace student_management_fe.Models
{
    public class StudentHomePageModel
    {
        public string Id { get; set; } // 8-digit student ID (e.g., 22010001)

        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public int? FacultyId { get; set; }

        public int? IntakeYear { get; set; }

        public int? ProgramId { get; set; }

        public int? StatusId { get; set; }
    }
}
