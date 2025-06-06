using Microsoft.JSInterop;
using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;

public class CourseEnrollmentService
{
    private readonly AuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private readonly IStringLocalizer<Content> _localizer;
    public CourseEnrollmentService(AuthService authService, IJSRuntime jSRuntime, IStringLocalizer<Content> localizer)
    {
        _authService = authService;
        _jsRuntime = jSRuntime;
        _localizer = localizer;
    }

    public static class EnrollmentActions
    {
        public const string Register = "register";
        public const string Unregister = "unregister";
    }

    public async Task<string> RegisterAndUnregisterClass(string action, CourseEnrollmentRequest courseEnrollmentRequest)
    {
        var apiEndpoint = $"/api/course-enrollments?action={action}";
        var json = JsonSerializer.Serialize(courseEnrollmentRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var response = await _authService.SendRequestWithAuthAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

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

    public async Task<string> UpdateStudentGrade(UpdateStudentGradeRequest updateStudentGradeRequest)
    {
        var apiEndpoint = $"/api/student/update-grade";
        var json = JsonSerializer.Serialize(updateStudentGradeRequest);
        var request = new HttpRequestMessage(HttpMethod.Put, apiEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
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

    public async Task DownloadTranscript(string studentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/student/{studentId}/transcript");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);
        }

        using var fileStream = await response.Content.ReadAsStreamAsync();
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/pdf";
        var fileName = $"{studentId}_transcript.pdf";

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var fileBytes = memoryStream.ToArray();

        await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, contentType, fileBytes);
    }
}
