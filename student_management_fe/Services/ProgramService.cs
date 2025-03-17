using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class ProgramService
{
    private readonly AuthService _authService;

    public ProgramService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<ProgramModel>> GetPrograms()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/study-program");
        var response = await _authService.SendRequestWithAuthAsync(request);

        return await response.Content.ReadFromJsonAsync<List<ProgramModel>>() ?? new List<ProgramModel>();
    }
}
