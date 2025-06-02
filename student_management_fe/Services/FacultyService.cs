using student_management_fe.Models;
using student_management_fe.Models.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;

public class FacultyService
{
    private readonly AuthService _authService;
    public FacultyService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<Faculty>> GetFaculties()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/faculty");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse<string>>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);
        }

        return await response.Content.ReadFromJsonAsync<List<Faculty>>() ?? new List<Faculty>();
    }

    public async Task<string> UpdateFaculty(Faculty faculty)
    {
        var json = JsonSerializer.Serialize(faculty);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/faculty")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse<string>>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);
           
        }
        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("message", out var message))
        {
            return message;
        }

        throw new Exception("Đã có lỗi xảy ra!");
    }

    public async Task<int> AddFaculty(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/faculty/{name}");

        var response = await _authService.SendRequestWithAuthAsync(request);
      
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse<string>>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);

        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        if (responseObj != null && responseObj.TryGetValue("id", out var facultyID))
        {
            return facultyID;
        }

        throw new Exception("Đã có lỗi xảy ra!");
    }


}
