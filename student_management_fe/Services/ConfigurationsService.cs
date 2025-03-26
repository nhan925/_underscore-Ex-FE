using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace student_management_fe.Services;

public class ConfigurationsService
{
    private readonly AuthService _authService;

    public ConfigurationsService(AuthService authService)
    {
        _authService = authService;
    }

    // Email Configuration Methods
    public async Task<ConfigurationsModel<List<string>>> GetEmailConfig()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/config/email");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Không thể lấy cấu hình email!");
        }

        return await response.Content.ReadFromJsonAsync<ConfigurationsModel<List<string>>>()
            ?? new ConfigurationsModel<List<string>> { Value = new List<string>() };
    }

    public async Task<string> UpdateEmailConfig(ConfigurationsModel<List<string>> emailConfig)
    {
        var json = JsonSerializer.Serialize(emailConfig);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/config/update/email")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật cấu hình email không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }

    // Phone Number Configuration Methods
    public async Task<ConfigurationsModel<List<string>>> GetPhoneNumberConfig()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/config/phone-number");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Không thể lấy cấu hình số điện thoại!");
        }

        return await response.Content.ReadFromJsonAsync<ConfigurationsModel<List<string>>>()
            ?? new ConfigurationsModel<List<string>> { Value = new List<string>() };
    }

    public async Task<string> UpdatePhoneNumberConfig(ConfigurationsModel<List<string>> phoneConfig)
    {
        var json = JsonSerializer.Serialize(phoneConfig);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/config/update/phone-number")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật cấu hình số điện thoại không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }

    // Student Status Configuration Methods
    public async Task<ConfigurationsModel<Dictionary<string, List<int>>>> GetStudentStatusConfig()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/config/student-status");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Không thể lấy cấu hình trạng thái sinh viên!");
        }

        return await response.Content.ReadFromJsonAsync<ConfigurationsModel<Dictionary<string, List<int>>>>()
            ?? new ConfigurationsModel<Dictionary<string, List<int>>> { Value = new Dictionary<string, List<int>>() };
    }

    public async Task<string> UpdateStudentStatusConfig(ConfigurationsModel<Dictionary<string, List<int>>> statusConfig)
    {
        var json = JsonSerializer.Serialize(statusConfig);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/config/update/student-status")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Cập nhật cấu hình trạng thái sinh viên không thành công!");
        }

        return await response.Content.ReadAsStringAsync();
    }
}