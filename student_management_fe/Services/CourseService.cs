using Radzen.Blazor.Rendering;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;
public class CourseService
{
    private readonly AuthService _authService;

    public CourseService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<CourseModel>> GetAllCourses()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/course");
        var response = await _authService.SendRequestWithAuthAsync(request);
        return await response.Content.ReadFromJsonAsync<List<CourseModel>>() ?? new List<CourseModel>();
    }

    public async Task<int> AddCourse(CourseModel course)
    {
        var json = JsonSerializer.Serialize(course);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/course")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Thêm khóa học không thành công!");
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        if (responseObj != null && responseObj.TryGetValue("id", out var courseId))
        {
            return courseId;
        }
        throw new Exception("Đã có lỗi xảy ra!");
    }

    public async Task<string> UpdateCourse(string id, CourseModel course)
    {
        var json = JsonSerializer.Serialize(course);
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/course/{id}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật khóa học không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteCourse(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/course/{id}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Xóa khóa học không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }
}