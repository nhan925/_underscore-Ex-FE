using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class FacultyService
{
    private readonly AuthService _authService;
    public FacultyService(AuthService authService)
    {
        _authService = authService;
    }
    public async Task<List<Faculty>> GetFaculties()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/faculty");
        var response = await _authService.SendRequestWithAuthAsync(request);
        
        return await response.Content.ReadFromJsonAsync<List<Faculty>>() ?? new List<Faculty>();
    }


}
