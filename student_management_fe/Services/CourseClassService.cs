﻿using ServiceStack;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;

public class CourseClassService
{
    private readonly AuthService _authService;
    public CourseClassService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<GetCourseClassResult>> GetAllCourseClassesBySemester(int semesterId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/classes/{semesterId}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Lỗi khi lấy danh sách lớp học!");
        }
        return await response.Content.ReadFromJsonAsync<List<GetCourseClassResult>>() ?? new();
    }

    public async Task<string> AddCourseClass(CourseClass courseClass)
    {
        var json = JsonSerializer.Serialize(courseClass);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/classes")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (responseObj != null && responseObj.TryGetValue("courseClassId", out var courseClassId))
        {
            return courseClassId;
        }
        throw new Exception("Thêm lớp học không thành công!");
    }

    public async Task<List<StudentInClass>> GetStudentsInClass(GetCourseClassResult courseClass)
    {
        string apiEndpoint = $"/api/classes/students?ClassId={courseClass.Id}&CourseId={courseClass.Course.Id}&SemesterId={courseClass.Semester.Id}";
        var request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"error fetching students: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<List<StudentInClass>>();
        return result ?? new List<StudentInClass>();
    }
}

