using student_management_fe.Models;
using student_management_fe.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Services;

public interface IStudyProgramService
{
    Task<List<StudyProgram>> GetPrograms();

    Task<string> UpdateProgram(StudyProgram program);

    Task<int> AddProgram(string name);
}

