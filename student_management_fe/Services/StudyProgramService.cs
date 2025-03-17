
using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class StudyProgramService
{
    private readonly AuthService _authService;

    public StudyProgramService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<Program>> GetPrograms()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/study_program");
        var response = await _authService.SendRequestWithAuthAsync(request);

        return await response.Content.ReadFromJsonAsync<List<Program>>() ?? new List<Program>();
    }
}


