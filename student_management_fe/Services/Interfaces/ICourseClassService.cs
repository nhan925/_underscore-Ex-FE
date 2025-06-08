using Microsoft.Extensions.Localization;
using student_management_fe.Helpers;
using student_management_fe.Resources;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static MudBlazor.Defaults;
using ServiceStack.Web;

namespace student_management_fe.Services;

public interface ICourseClassService
{
    Task<List<GetCourseClassResult>> GetAllCourseClassesBySemester(int semesterId);

    Task<GetCourseClassResult> GetCourseClassByIdAndCourseAndSemester(string classId, string courseId, int semesterId);

    Task<string> AddCourseClass(CourseClass courseClass);

    Task<List<StudentInClass>> GetStudentsInClass(GetCourseClassResult courseClass);
}

