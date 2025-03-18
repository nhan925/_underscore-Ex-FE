using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages;
public partial class FacultyManagement
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

    private List<Faculty> faculties = new List<Faculty>();

    private readonly FacultyService _facultyService;

    public FacultyManagement(FacultyService facultyService)
    {
        _facultyService = facultyService;
    }

    protected override async Task OnInitializedAsync()
    {

        await GenerateMockFaculties(10);

        // await LoadFaculties();

    }

    private async Task GenerateMockFaculties(int n)
    {
        for (int i = 0; i < n; i++)
        {
            var faculty = new Faculty
            {
                Id = i + 1,
                Name = $"Faculty {i + 1}",
            };
            faculties.Add(faculty);
        }
    }

    private async Task LoadFaculties(string? search = null)
    {
        //Add API call to get students

        //var result = await  _facultyService.GetAllFaculties(currentPage, pageSize, search);
        //faculties = result.Items;
        //totalCount = result.TotalCount;


    }

    private async Task SearchFaculty()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            searchText = null;
        }

        currentPage = 1;

        Console.WriteLine($"Search Text: {searchText}");

        await LoadFaculties(searchText);
    }

    private async Task HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchFaculty();
        }
    }

    private async Task AddFaculty()
    {
            
    }

    private async Task EditFaculty(int id)
    { 

    }


    private async Task NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            await LoadFaculties();
        }
    }

    private async Task PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            await LoadFaculties();
        }
    }
}
