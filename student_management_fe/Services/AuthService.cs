using Microsoft.AspNetCore.Components.Authorization;
using student_management_fe.Authentication;
using student_management_fe.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<bool> Login(LoginModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", user);

        if (!response.IsSuccessStatusCode)
            return false;

        var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (tokens is null || string.IsNullOrEmpty(tokens.AccessToken))
            return false;

        await _authStateProvider.SetUserAuthenticated(tokens.AccessToken);
        return true;
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

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await Logout();
        }

        return response;
    }
}
