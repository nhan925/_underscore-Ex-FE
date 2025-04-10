using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class YearAndSemesterService
{
    private readonly AuthService _authService;
    public YearAndSemesterService(AuthService authService)
    {
        _authService = authService;
    }
    
    public async Task<List<Year>> GetAllYears()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/year");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Lỗi khi lấy danh sách năm học!");
        }
        return await response.Content.ReadFromJsonAsync<List<Year>>() ?? new();
    }

    public async Task<List<Semester>> GetSemestersByYear(int yearId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/year/{yearId}/semesters");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Lỗi khi lấy danh sách học kỳ!");
        }
        return await response.Content.ReadFromJsonAsync<List<Semester>>() ?? new();
    }
}
