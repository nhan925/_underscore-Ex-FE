
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
using student_management_fe.Helpers;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;

public interface IConfigurationsService
{

    Task<bool> CheckConfig(string type, string value);

    Task<List<StudentStatus>> GetNextStatuses(int? StatusId);

    // Email Configuration Methods
    Task<ConfigurationsModel<List<string>>> GetEmailConfig();

    Task<string> UpdateEmailConfig(ConfigurationsModel<List<string>> emailConfig);

    // Phone Number Configuration Methods
    Task<ConfigurationsModel<List<string>>> GetPhoneNumberConfig();

    Task<string> UpdatePhoneNumberConfig(ConfigurationsModel<List<string>> phoneConfig);

    // Student Status Configuration Methods
    Task<ConfigurationsModel<Dictionary<string, List<int>>>> GetStudentStatusConfig();

    Task<string> UpdateStudentStatusConfig(ConfigurationsModel<Dictionary<string, List<int>>> statusConfig);
}
