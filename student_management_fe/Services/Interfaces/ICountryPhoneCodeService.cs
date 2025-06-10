using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;

public interface ICountryPhoneCodeService
{
    Task<List<CountryPhoneCodeModel>> GetCountryPhoneCodes();
}