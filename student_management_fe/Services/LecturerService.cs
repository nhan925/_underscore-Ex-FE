using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class LecturerService
{
    private readonly AuthService _authService;

    public LecturerService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<Lecturer>> GetAllLecturers()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/lecturers");
        var response = await _authService.SendRequestWithAuthAsync(request);

        return await response.Content.ReadFromJsonAsync<List<Lecturer>>() ?? new ();
    }
}
