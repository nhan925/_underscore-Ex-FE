using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;

public class CountryPhoneCodeService
{
    private readonly AuthService _authService;

    public CountryPhoneCodeService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<CountryPhoneCodeModel>> GetCountryPhoneCodes()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/phone-code");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            var errorMessage = errorResponse?.Message;

            throw new Exception(errorMessage);

        }
        return await response.Content.ReadFromJsonAsync<List<CountryPhoneCodeModel>>() ?? new List<CountryPhoneCodeModel>();
    }
}