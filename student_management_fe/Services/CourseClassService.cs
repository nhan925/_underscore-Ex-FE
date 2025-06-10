using Microsoft.Extensions.Localization;
using student_management_fe.Helpers;
using student_management_fe.Resources;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static MudBlazor.Defaults;
using ServiceStack.Web;

namespace student_management_fe.Services;

public class CourseClassService : ICourseClassService
{
    private readonly IAuthService _authService;
    private readonly IStringLocalizer<Content> _localizer;
    public CourseClassService(IAuthService authService, IStringLocalizer<Content> localizer)
    {
        _authService = authService;
        _localizer = localizer;
    }

    public async Task<List<GetCourseClassResult>> GetAllCourseClassesBySemester(int semesterId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/classes/in-semester/{semesterId}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }
        return await response.Content.ReadFromJsonAsync<List<GetCourseClassResult>>() ?? new();
    }

    public async Task<GetCourseClassResult> GetCourseClassByIdAndCourseAndSemester(string classId, string courseId, int semesterId)
    {
        var compositedId = $"{classId}-{courseId}-{semesterId}";
        var apiEndpoint = $"api/classes/{compositedId}";

        var request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var courseClass = await response.Content.ReadFromJsonAsync<GetCourseClassResult>();
        return courseClass ?? new GetCourseClassResult();
    }


    public async Task<string> AddCourseClass(CourseClass courseClass)
    {
        var json = JsonSerializer.Serialize(courseClass);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/classes")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        HttpResponseMessage? response = null;
        response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("courseClassId", out var courseClassId))
        {
            return courseClassId;
        }
        else
        {
            throw new Exception(_localizer["an_unexpected_error_occurred_Please_try_again_later"]);
        } 
    }

    public async Task<List<StudentInClass>> GetStudentsInClass(GetCourseClassResult courseClass)
    {
        string apiEndpoint = $"/api/classes/students?ClassId={courseClass.Id}&CourseId={courseClass.Course.Id}&SemesterId={courseClass.Semester.Id}";
        var request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }

        var result = await response.Content.ReadFromJsonAsync<List<StudentInClass>>();
        return result ?? new List<StudentInClass>();
    }
}

