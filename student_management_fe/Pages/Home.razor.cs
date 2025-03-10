using System.Diagnostics;
using student_management_fe.Models;

namespace student_management_fe.Pages
{
    public partial class Home
    {
        private string searchText;
        private List<StudentModel> students = new List<StudentModel>();
        private List<StudentModel> PaginatedStudents => students.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

        private int currentPage = 1;
        private int pageSize = 25;
        private int totalPages => (int)Math.Ceiling((double)students.Count / pageSize);

        public Home()
        {
            GenerateStudents(20);
        }

        private void GenerateStudents(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                students.Add(new StudentModel
                {
                    StudentId = $"22120{200 + i}",
                    FullName = $"Sinh viên {i}",
                    BirthDate = DateTime.Now.AddYears(-20).AddMonths(i),
                    Gender = i % 2 == 0 ? "Nam" : "Nữ",
                    Faculty = "Công nghệ thông tin",
                    IntakeYear = $"K{40 + (i % 5)}",
                    Program = i % 2 == 0 ? "Chính quy" : "Liên thông",
                    Address = $"Quận {i % 12 + 1}, TP. Hồ Chí Minh",
                    Email = $"sinhvien{i}@student.hcmus.edu.vn",
                    PhoneNumber = $"031276{i % 1000}{i % 10000}",
                    Status = i % 3 == 0 ? "Bảo lưu" : "Đang học"
                });
            }
        }

        private void SearchStudents()
        {
            //Call api to search students or handle search in client side
        }

        private void AddStudent()
        {
            //Navigation to Add page
        }

        private void EditStudent(string mssv)
        {
            //Navigation to Edit page
        }

        private void DeleteStudent(string mssv)
        {
            //Call api to delete student
        }



        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
            }
        }

        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
            }
        }

    }
}
