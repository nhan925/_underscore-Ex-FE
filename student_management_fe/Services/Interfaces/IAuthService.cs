using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.JSInterop;
using student_management_fe.Authentication;
using student_management_fe.Helpers;
using student_management_fe.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public interface IAuthService
{
    Task Login(LoginModel user);
    Task Logout();
    Task<HttpResponseMessage> SendRequestWithAuthAsync(HttpRequestMessage request);
}
