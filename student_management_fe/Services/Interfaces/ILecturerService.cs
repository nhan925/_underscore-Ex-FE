using student_management_fe.Helpers;
using student_management_fe.Models;
using System.Net.Http.Json;

namespace student_management_fe.Services;

public interface ILecturerService
{
    Task<List<Lecturer>> GetAllLecturers();
}
