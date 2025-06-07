using Radzen.Blazor.Rendering;
using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;
public class CourseService
{
    private readonly AuthService _authService;
    private readonly IStringLocalizer<Content> _localizer;

    public CourseService(AuthService authService, IStringLocalizer<Content> localizer)
    {
        _authService = authService;
        _localizer = localizer;
    }

    public async Task<List<CourseModel>> GetAllCourses()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/course");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);
        }
        return await response.Content.ReadFromJsonAsync<List<CourseModel>>() ?? new List<CourseModel>();
    }

    public async Task<String> AddCourse(CourseModel course)
    {
        var json = JsonSerializer.Serialize(course);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/course")
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

        var responseObj = await response.Content.ReadFromJsonAsync<ApiResponse<CourseModel>>();
        if (responseObj != null && responseObj.Message != null)
        {
            return responseObj.Message;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);

    }

    // Lớp để parse response từ API
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public async Task<string> UpdateCourse(CourseModel course)
    {
        var json = JsonSerializer.Serialize(course);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/course")
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

    public async Task<string> DeleteCourse(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/course/{id}");
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

  
    public async Task<bool> CheckCourseHasStudents(string courseId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/course/{courseId}/has-students");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(_localizer["check_course_has_students_request_failed"]);
        }

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var result = JsonSerializer.Deserialize<HasStudentsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result?.HasStudents ?? false;
        }
        catch (JsonException)
        {
            throw new Exception(_localizer["check_course_has_students_response_parse_failed"]);
        }
    }

    public class HasStudentsResponse
    {
        public bool HasStudents { get; set; }
    }
}