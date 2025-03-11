using student_management_fe.Models;
using System.Net.Http.Json;

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

        return await response.Content.ReadFromJsonAsync<List<StudentStatus>>() ?? new List<StudentStatus>();
    }
}
