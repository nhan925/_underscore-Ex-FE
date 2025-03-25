using Microsoft.AspNetCore.Components.Authorization;
using student_management_fe.Authentication;
using student_management_fe.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;

namespace student_management_fe.Services;

public class ConfigurationsService
{
    private readonly AuthService _authService;

    public ConfigurationsService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<ConfigurationModel<T>> GetConfigurations<T>(string type)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/config/{type}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        return await response.Content.ReadFromJsonAsync<ConfigurationModel<T>>() ?? new ConfigurationModel<T>();
    }

    public async Task<bool> CheckConfig(string type, string value)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/config/check/{type}/{value}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Thông tin không hợp lệ!");
        }

        var responseObj = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();
        if (responseObj != null && responseObj.TryGetValue("result", out var result))
        {
            Console.WriteLine(result);
            return result;
        }
        throw new Exception("Đã có lỗi xảy ra!");
       
    }
}
