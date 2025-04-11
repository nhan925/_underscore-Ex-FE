using student_management_fe.Models;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace student_management_fe.Services;

public class CourseEnrollmentService
{
    private readonly AuthService authService;
    public CourseEnrollmentService(AuthService authService)
    {
        this.authService = authService;
    }

    public async Task<string> RegisterAndUnregisterClass(string action, CourseEnrollmentRequest courseEnrollmentRequest)
    {
        var apiEndpoint = $"/api/course-enrollments?action={action}";
        var json = JsonSerializer.Serialize(courseEnrollmentRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var response = await authService.SendRequestWithAuthAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new Exception("Đăng ký lớp học không thành công!");
        }
    }

    public async Task<string> UpdateStudentGrade(UpdateStudentGradeRequest updateStudentGradeRequest)
    {
        var apiEndpoint = $"/api/course-enrollments/update-grade";
        var json = JsonSerializer.Serialize(updateStudentGradeRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var response = await authService.SendRequestWithAuthAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new Exception("Cập nhật điểm không thành công!");
        }
    }
}
