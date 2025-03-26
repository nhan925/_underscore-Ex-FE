
﻿using Microsoft.AspNetCore.Components.Authorization;
using student_management_fe.Authentication;
using student_management_fe.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
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

    public async Task<List<StudentStatus>> GetNextStatuses(int? StatusId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/config/next-statuses/{StatusId}");
        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Lỗi khi lấy danh sách trạng thái tiếp theo!");
        }

        var result = await response.Content.ReadFromJsonAsync<List<StudentStatus>>();
        return result ?? new List<StudentStatus>();
    }
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
