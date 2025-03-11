using Microsoft.AspNetCore.Components;
using student_management_fe.Models;
namespace student_management_fe.Pages;

public partial class AddStudent
{
    private StudentModel NewStudent = new StudentModel();
    private bool IsSaving = false;
    private StudentForm? StudentFormRef;

    private async Task Add()
    {
        IsSaving = true;

        try
        {
            // Giả sử có service xử lý việc thêm sinh viên
            //await StudentService.AddStudentAsync(NewStudent);

            Console.WriteLine("Thông tin sinh viên mới trước khi thêm:");
            Console.WriteLine($"FullName: {NewStudent.FullName}");
            Console.WriteLine($"BirthDate: {NewStudent.BirthDate}");
            Console.WriteLine($"Gender: {NewStudent.Gender}");
            Console.WriteLine($"FacultyId: {NewStudent.FacultyId}");
            Console.WriteLine($"IntakeYear: {NewStudent.IntakeYear}");
            Console.WriteLine($"Program: {NewStudent.Program}");
            Console.WriteLine($"Address: {NewStudent.Address}");
            Console.WriteLine($"Email: {NewStudent.Email}");
            Console.WriteLine($"PhoneNumber: {NewStudent.PhoneNumber}");
            Console.WriteLine($"StatusId: {NewStudent.StatusId}");

            StudentFormRef?.CloseOverlay();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi thêm sinh viên: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }
}
