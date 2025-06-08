using Radzen.Blazor.Rendering;
using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;
public interface ICourseService
{

    Task<List<CourseModel>> GetAllCourses();

    Task<String> AddCourse(CourseModel course);

    Task<string> UpdateCourse(CourseModel course);

    Task<string> DeleteCourse(string id);

    Task<bool> CheckCourseHasStudents(string courseId);

    public class HasStudentsResponse
    {
        public bool HasStudents { get; set; }
    }
}