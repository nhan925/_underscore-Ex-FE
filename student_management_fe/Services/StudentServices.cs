using Microsoft.AspNetCore.Components.Web;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace student_management_fe.Services;

public class StudentServices
{
    private readonly AuthService _authService;
    
    public StudentServices(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<PagedResult<StudentModel>> GetAllStudents(int page, int pageSize, string? search = null)
    {
        string apiEndpoint = $"/api/student?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(search))
        {
            apiEndpoint += $"&search={search}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"error fetching students: {response.StatusCode}");
        }

        // Deserialize response content into the object
        var result = await response.Content.ReadFromJsonAsync<PagedResult<StudentModel>>();
        return result ?? new PagedResult<StudentModel>(); 
    }

    public async Task DeleteStudent(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/student/{id}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new Exception($"Không tìm thấy sinh viên !");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Đã có lỗi xảy ra !");
        }
    }

    public async Task<string> AddStudent(StudentModel student)
    {
        var json = JsonSerializer.Serialize(student);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        // Remove a specific key (e.g., "unwantedKey")
        dictionary.Remove("Id");

        // Convert back to JSON
        var updatedJson = JsonSerializer.Serialize(dictionary);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/student");
        request.Content = new StringContent(updatedJson, System.Text.Encoding.UTF8, "application/json");

        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Thêm không thành công !");
        }

        // Deserialize the response
        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        if (responseObj != null && responseObj.TryGetValue("studentId", out var studentId))
        {
            return studentId;
        }

        throw new Exception("Đã có lỗi xảy ra !");
    }

    public async Task<StudentModel> GetStudentById(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/student/{id}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Không tìm thấy sinh viên !");
        }

        var student = await response.Content.ReadFromJsonAsync<StudentModel>();
        return student ?? new StudentModel();
    }

    public async Task UpdateStudent(StudentModel student)
    {
        var json = JsonSerializer.Serialize(student);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        dictionary.Remove("Id");

        var updatedJson = JsonSerializer.Serialize(dictionary);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/student/{student.Id}");
        request.Content = new StringContent(updatedJson, System.Text.Encoding.UTF8, "application/json");
        var response = await _authService.SendRequestWithAuthAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật không thành công !");
        }
    }
}
