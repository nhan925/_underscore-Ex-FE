using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages;

public partial class StudentStatusManagement
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

    private List<StudentStatus> studentStatuses = new List<StudentStatus>();
    private List<StudentStatus> tempStudentStatuses = new List<StudentStatus>();

    private readonly StudentStatusService _studentStatusService;

    public StudentStatusManagement(StudentStatusService studentStatusService)
    {
        _studentStatusService = studentStatusService;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStudentStatuses();

    }

    private async Task LoadStudentStatuses(string? search = null)
    {
        studentStatuses = await _studentStatusService.GetStudentStatuses();
        tempStudentStatuses = studentStatuses;
    }

    private async Task SearchStudentStatus()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            tempStudentStatuses = studentStatuses;
            searchText = null;
        }
        else
        {
            tempStudentStatuses = studentStatuses.Where(s => s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }
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
        var studentStatus = new StudentStatus();
        var parameters =new Dictionary<string, object>
        {
            {"TitleText", "Tên trạng thái sinh viên"},
            {"ButtonText", "Lưu"},
            {"StudentStatus", studentStatus}
        };

        var result = await DialogService.OpenAsync<StudentStatusForm>("Thêm trạng thái sinh viên", parameters);

        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var studentStatusId = await _studentStatusService.AddStudentStatus(studentStatus.Name);
                await LoadStudentStatuses();
                Snackbar.Add($"Đã thêm trạng thái sinh viên với id: {studentStatusId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task EditStudentStatus(StudentStatus studentStatus)
    {
        var parameters = new Dictionary<string, object>
        {
            {"StudentStatus", studentStatus },
            {"ButtonText", "Cập nhật" },
            {"TitleText", "Tên trạng thái sinh viên" },
        };

        var result = await DialogService.OpenAsync<StudentStatusForm>("Cập nhật trạng thái sinh viên", parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var message = await _studentStatusService.UpdateStudentStatus(studentStatus);
                await LoadStudentStatuses();
                Snackbar.Add($"Đã cập nhật trạng thái sinh viên thành công !", Severity.Success);
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