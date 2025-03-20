using student_management_fe.Models;

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace student_management_fe.Services;

public class StudyProgramService
{
    private readonly AuthService _authService;

    public StudyProgramService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<StudyProgram>> GetPrograms()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/study-program");
        var response = await _authService.SendRequestWithAuthAsync(request);

        return await response.Content.ReadFromJsonAsync<List<StudyProgram>>() ?? new List<StudyProgram>();
    }

    public async Task<int> AddProgram(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/study-program/{name}");

        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Thêm không thành công!");
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();

        if (responseObj != null && responseObj.TryGetValue("id", out var studyProgramID))
        {
            return studyProgramID;
        }

        throw new Exception("Đã có lỗi xảy ra!");
    }



    public async Task<string> UpdateProgram(StudyProgram program)
    {
        var json = JsonSerializer.Serialize(program);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/study-program")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }
}

