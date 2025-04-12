using Microsoft.JSInterop;
using student_management_fe.Models;
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

    public async Task<string> RegisterAndUnregisterClass(string action, CourseEnrollmentRequest courseEnrollmentRequest)
    {
        var apiEndpoint = $"/api/course-enrollments?action={action}";
        var json = JsonSerializer.Serialize(courseEnrollmentRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var response = await _authService.SendRequestWithAuthAsync(request);
        var message = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(message);
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> UpdateStudentGrade(UpdateStudentGradeRequest updateStudentGradeRequest)
    {
        var apiEndpoint = $"/api/course-enrollments/update-grade";
        var json = JsonSerializer.Serialize(updateStudentGradeRequest);
        var request = new HttpRequestMessage(HttpMethod.Put, apiEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật điểm không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task DownloadTranscript(string studentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/course-enrollments/transcript/{studentId}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Lỗi khi tải bảng điểm");
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
