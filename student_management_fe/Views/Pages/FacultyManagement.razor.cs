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
    private Radzen.DialogService DialogService { get; set; } = default!;

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
        await LoadFaculties();

    }

    private async Task LoadFaculties(string? search = null)
    {
        faculties = await _facultyService.GetFaculties();
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
        var faculty = new Faculty();
        var parameters = new Dictionary<string, object>
        {
            {"Faculty", faculty },
            {"ButtonText", "Lưu" },
            {"TitleText", "Tên khoa" },
        };

        var result = await DialogService.OpenAsync<FacultyForm>("Thêm khoa", parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var facultyId = await _facultyService.AddFaculty(faculty.Name);
                await LoadFaculties();
                Snackbar.Add($"Đã thêm khoa thành công với id: {facultyId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task EditFaculty(Faculty faculty)
    {
        var parameters = new Dictionary<string, object>
        {
            {"Faculty", faculty },
            {"ButtonText", "Cập nhật" },
            {"TitleText", "Tên khoa" },
        };

        var result = await DialogService.OpenAsync<FacultyForm>("Cập nhật khoa", parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var message = await _facultyService.UpdateFaculty(faculty);
                await LoadFaculties();
                Snackbar.Add($"Đã cập nhật khoa thành công !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
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
