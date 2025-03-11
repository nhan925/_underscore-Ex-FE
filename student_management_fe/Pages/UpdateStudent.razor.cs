using Microsoft.AspNetCore.Components;
using student_management_fe.Models;
namespace student_management_fe.Pages;
public partial class UpdateStudent
{
    private StudentModel ExistingStudent = new StudentModel();
    private StudentForm? StudentFormRef;
    private bool IsSaving = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            //ExistingStudent = await StudentService.GetStudentByIdAsync(StudentId) ?? new StudentModel();
            ExistingStudent = new StudentModel
            {
                StudentId = 1,
                FullName = "Nguyễn Văn A",
                BirthDate = new DateTime(2002, 5, 20),
                Gender = "Nam",
                FacultyId = 1, 
                IntakeYear = 2020,
                Program = "Kỹ thuật phần mềm",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Email = "nguyenvana@example.com",
                PhoneNumber = "0123456789",
                StatusId = 1 
            };

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tải sinh viên: {ex.Message}");
        }
    }

    private async Task Update()
    {
        IsSaving = true;

        try
        {
            //await StudentService.UpdateStudentAsync(ExistingStudent);

            Console.WriteLine("Thông tin sinh viên trước khi cập nhật:");
            Console.WriteLine($"StudentId: {ExistingStudent.StudentId}");
            Console.WriteLine($"FullName: {ExistingStudent.FullName}");
            Console.WriteLine($"BirthDate: {ExistingStudent.BirthDate}");
            Console.WriteLine($"Gender: {ExistingStudent.Gender}");
            Console.WriteLine($"FacultyId: {ExistingStudent.FacultyId}");
            Console.WriteLine($"IntakeYear: {ExistingStudent.IntakeYear}");
            Console.WriteLine($"Program: {ExistingStudent.Program}");
            Console.WriteLine($"Address: {ExistingStudent.Address}");
            Console.WriteLine($"Email: {ExistingStudent.Email}");
            Console.WriteLine($"PhoneNumber: {ExistingStudent.PhoneNumber}");
            Console.WriteLine($"StatusId: {ExistingStudent.StatusId}");

            StudentFormRef?.CloseOverlay();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi cập nhật sinh viên: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

}
