using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;

public interface IFacultyService
{
    Task<List<Faculty>> GetFaculties();

    Task<string> UpdateFaculty(Faculty faculty);

    Task<int> AddFaculty(string name);
}
