using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Helpers;
using student_management_fe.Resources;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace student_management_fe.Services;

public class StudentServices
{
    private readonly AuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private readonly IStringLocalizer<Content> _localizer;

    public StudentServices(AuthService authService, IJSRuntime jsRuntime, IStringLocalizer<Content> localizer)
    {
        _authService = authService;
        _jsRuntime = jsRuntime;
        _localizer = localizer;
    }

    public async Task<PagedResult<StudentHomePageModel>> GetAllStudents(int page, int pageSize, string? search = null, StudentFilter? filter = null)
    {
        string apiEndpoint = $"/api/student?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(search))
        {
            apiEndpoint += $"&search={search}";
        }
        if (filter != null)
        {
            if (filter.FacultyIds != null && filter.FacultyIds.Any())
            {
                foreach (var facultyId in filter.FacultyIds)
                {
                    apiEndpoint += $"&FacultyIds={facultyId}";
                }
            }
            Console.WriteLine(apiEndpoint);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        // Deserialize response content into the object
        var result = await response.Content.ReadFromJsonAsync<PagedResult<StudentHomePageModel>>();
        return result ?? new PagedResult<StudentHomePageModel>();
    }

    public async Task<string> DeleteStudent(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/student/{id}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("message", out var message))
        {
            return message;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
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
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        // Deserialize the response
        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        if (responseObj != null && responseObj.TryGetValue("studentId", out var studentId))
        {
            return studentId;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
    }

    public async Task<StudentModel> GetStudentById(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/student/{id}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var student = await response.Content.ReadFromJsonAsync<StudentModel>();
        return student ?? new StudentModel();
    }

    public async Task<string> UpdateStudent(StudentModel student)
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
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("message", out var message))
        {
            return message;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
    }

    // For add students from file
    public async Task<string> UploadFiles(IBrowserFile file, string format)
    {
        using var content = new MultipartFormDataContent();

        // Mở stream với giới hạn tối đa là 5MB (hoặc giới hạn của server)
        using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
        content.Add(new StreamContent(stream), "file", file.Name);

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/student/import/{format}")
        {
            Content = content
        };

        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("message", out var message))
        {
            return message;
        }

        throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
    }

    public async Task DownloadFile(string format)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/student/export/{format}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        using var fileStream = await response.Content.ReadAsStreamAsync(); 
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = format.ToLower() == "json" ? "students.json" : "students.xlsx";

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream); 
        memoryStream.Position = 0;

        var fileBytes = memoryStream.ToArray(); 

        await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, contentType, fileBytes);
    }

}
