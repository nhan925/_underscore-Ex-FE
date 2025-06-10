using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Helpers;
using student_management_fe.Resources;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace student_management_fe.Services;

public interface IStudentServices
{
    Task<PagedResult<StudentHomePageModel>> GetAllStudents(int page, int pageSize, string? search = null, StudentFilter? filter = null);

    Task<string> DeleteStudent(string id);

    Task<string> AddStudent(StudentModel student);

    Task<StudentModel> GetStudentById(string id);

    Task<string> UpdateStudent(StudentModel student);

    // For add students from file
    Task<string> UploadFiles(IBrowserFile file, string format);

    Task DownloadFile(string format);
}
