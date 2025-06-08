using student_management_fe.Helpers;
using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class YearAndSemesterService : IYearAndSemesterService
{
    private readonly IAuthService _authService;
    public YearAndSemesterService(IAuthService authService)
    {
        _authService = authService;
    }
    
    public async Task<List<Year>> GetAllYears()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/year");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }
        return await response.Content.ReadFromJsonAsync<List<Year>>() ?? new();
    }

    public async Task<List<Semester>> GetSemestersByYear(int yearId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/year/{yearId}/semesters");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }
        return await response.Content.ReadFromJsonAsync<List<Semester>>() ?? new();
    }
}
