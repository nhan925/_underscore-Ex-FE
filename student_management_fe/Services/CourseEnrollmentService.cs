using Microsoft.JSInterop;
using student_management_fe.Models;
using student_management_fe.Models.Helpers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace student_management_fe.Services;

public class CourseEnrollmentService
{
    private readonly AuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    public CourseEnrollmentService(AuthService authService, IJSRuntime jSRuntime)
    {
        _authService = authService;
        _jsRuntime = jSRuntime;
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

        //if (!response.IsSuccessStatusCode)
        //{
        //    try
        //    {
        //        var errorDetail = JsonSerializer.Deserialize<JsonElement>(responseContent).GetProperty("details").GetString();
        //        throw new Exception(errorDetail);
        //    }
        //    catch (JsonException)
        //    {
        //        throw new Exception($"Đã có lỗi xảy ra. Vui lòng thử lại sau!");
        //    }

        //}

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse<string>>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);
        }

        try
        {
            var message = JsonSerializer.Deserialize<JsonElement>(responseContent)
                             .GetProperty("message").GetString();
            return message ?? "Thao tác thành công.";
        }
        catch (JsonException)
        {
            return "Thao tác thành công."; 
        }
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

    public async Task DownloadTranscript(string studentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/student/{studentId}/transcript");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse<string>>();
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
