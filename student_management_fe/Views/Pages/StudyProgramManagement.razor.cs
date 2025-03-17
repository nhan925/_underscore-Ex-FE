using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;

namespace student_management_fe.Views.Pages;
public partial class StudyProgramManagement
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private int _currentPage = 1;
    private int currentPage
    {
        get
        {
            if (totalPages == 0)
            {
                return 1;
            }

            return _currentPage;
        }

        set
        {
            if (value > 0 && value <= totalPages)
            {
                _currentPage = value;
            }
        }
    }
    private int pageSize = 10;
    private int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    private int totalCount { get; set; } = 100;


    private string? searchText;

    private List<StudyProgram> studyPrograms = new List<StudyProgram>();

    private readonly StudyProgramService _facultyService;

    public StudyProgramManagement(StudyProgramService studyProgramService)
    {
        _facultyService = studyProgramService;
    }

    protected override async Task OnInitializedAsync()
    {

        await GenerateMockStudyProgram(10);

        // await LoadFaculties();

    }

    private async Task GenerateMockStudyProgram(int n)
    {
        for (int i = 0; i < n; i++)
        {
            var item = new StudyProgram
            {
                Id = i + 1,
                Name = $"StudyProgram {i + 1}",
            };
            studyPrograms.Add(item);
        }
    }

    private async Task LoadStudyPrograms(string? search = null)
    {
        //Add API call to get students

        //var result = await  _facultyService.GetAllFaculties(currentPage, pageSize, search);
        //faculties = result.Items;
        //totalCount = result.TotalCount;


    }

    private async Task SearchStudyProgram()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            searchText = null;
        }

        currentPage = 1;

        Console.WriteLine($"Search Text: {searchText}");

        await LoadStudyPrograms(searchText);
    }

    private async Task HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await LoadStudyPrograms();
        }
    }

    private async Task AddStudyProgram()
    {

    }

    private async Task EditStudyProgram(int id)
    {

    }


    private async Task NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            await LoadStudyPrograms();
        }
    }

    private async Task PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            await LoadStudyPrograms();
        }
    }
}
