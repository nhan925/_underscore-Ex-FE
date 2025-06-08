using Microsoft.JSInterop;
using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;

public interface ICourseEnrollmentService
{
    public static class EnrollmentActions
    {
        public const string Register = "register";
        public const string Unregister = "unregister";
    }

    Task<string> RegisterAndUnregisterClass(string action, CourseEnrollmentRequest courseEnrollmentRequest);

    Task<string> UpdateStudentGrade(UpdateStudentGradeRequest updateStudentGradeRequest);

    Task DownloadTranscript(string studentId);
}
