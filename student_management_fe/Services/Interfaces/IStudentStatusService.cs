using Microsoft.Extensions.Localization;
using student_management_fe.Helpers;
using student_management_fe.Resources;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace student_management_fe.Services;

public interface IStudentStatusService
{
    Task<List<StudentStatus>> GetStudentStatuses();

    Task<int> AddStudentStatus(string name);

    Task<string> UpdateStudentStatus(StudentStatus status);
}
