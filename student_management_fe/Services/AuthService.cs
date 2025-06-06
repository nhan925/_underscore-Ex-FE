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

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, IJSRuntime js)
    {
        _httpClient = httpClient;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        _js = js;
    }

    public async Task Login(LoginModel user)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = JsonContent.Create(user)
        };

        var lang = await _js.InvokeAsync<string>("blazorCulture.get") ?? "vi";
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message ?? "Login failed");
        }

        var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (tokens is null || string.IsNullOrEmpty(tokens.AccessToken))
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorResponse?.Message);
        }
        
        await _authStateProvider.SetUserAuthenticated(tokens.AccessToken);
    }

    public async Task Logout()
    {
        await _authStateProvider.Logout();
    }

    public async Task<HttpResponseMessage> SendRequestWithAuthAsync(HttpRequestMessage request)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!user.Identity.IsAuthenticated)
        {
            await Logout();
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
        }

        var token = await _authStateProvider.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        var lang = await _js.InvokeAsync<string>("blazorCulture.get") ?? "vi";
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await Logout();
        }

        return response;
    }
}
