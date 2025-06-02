using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using student_management_fe.Localization;

namespace student_management_fe.Services;

public class FacultyService
{
    private readonly AuthService _authService;
    private readonly IStringLocalizer<Content> _localizer;
    public FacultyService(AuthService authService, IStringLocalizer<Content> localizer)
    {
        _authService = authService;
        _localizer = localizer;
    }

    public async Task<List<Faculty>> GetFaculties()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/faculty");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
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
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);
           
        }
        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("message", out var message))
        {
            return message;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
    }

    public async Task<int> AddFaculty(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/faculty/{name}");

        var response = await _authService.SendRequestWithAuthAsync(request);
      
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);

        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        if (responseObj != null && responseObj.TryGetValue("id", out var facultyID))
        {
            return facultyID;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
    }


}
