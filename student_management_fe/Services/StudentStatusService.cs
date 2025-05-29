using student_management_fe.Helpers;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;

public class StudentStatusService
{
    private readonly AuthService _authService;

    public StudentStatusService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<StudentStatus>> GetStudentStatuses()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/student-status");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        return await response.Content.ReadFromJsonAsync<List<StudentStatus>>() ?? new List<StudentStatus>();
    }

    public async Task<int> AddStudentStatus(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/student-status/{name}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        if (responseObj != null && responseObj.TryGetValue("id", out var studentStatusID))
        {
            return studentStatusID;
        }

        throw new Exception("Đã có lỗi xảy ra!");
    }

    public async Task<string> UpdateStudentStatus(StudentStatus status)
    {
        var json = JsonSerializer.Serialize(status);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/student-status")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        return await response.Content.ReadAsStringAsync();
    }

}
