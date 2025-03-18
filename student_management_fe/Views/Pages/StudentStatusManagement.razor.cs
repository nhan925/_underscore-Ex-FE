using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;

namespace student_management_fe.Views.Pages;

public partial class StudentStatusManagement
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

    private List<StudentStatus> studentStatuses = new List<StudentStatus>();

    private readonly StudentStatusService _studentStatusService;

    public StudentStatusManagement(StudentStatusService studentStatusService)
    {
        _studentStatusService = studentStatusService;
    }

    protected override async Task OnInitializedAsync()
    {

        await GenerateMockStudentStatus(10);

        // await LoadFaculties();

    }

    private async Task GenerateMockStudentStatus(int n)
    {
        for (int i = 0; i < n; i++)
        {
            var item = new StudentStatus
            {
                Id = i + 1,
                Name = $"Student status {i + 1}",
            };
            studentStatuses.Add(item);
        }
    }

    private async Task LoadStudentStatuses(string? search = null)
    {
        //Add API call to get students

        //var result = await  _studentStatusService.GetAllFaculties(currentPage, pageSize, search);
        //studentStatuses = result.Items;
        //totalCount = result.TotalCount;


    }

    private async Task SearchStudentStatus()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            searchText = null;
        }

        currentPage = 1;

        Console.WriteLine($"Search Text: {searchText}");

        await LoadStudentStatuses(searchText);
    }

    private async Task HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await LoadStudentStatuses();
        }
    }

    private async Task AddStudentStatus()
    {

    }

    private async Task EditStudentStatus(int id)
    {

    }


    private async Task NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            await LoadStudentStatuses();
        }
    }

    private async Task PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            await LoadStudentStatuses();
        }
    }
}