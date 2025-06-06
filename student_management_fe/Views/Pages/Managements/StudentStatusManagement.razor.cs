using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared.ManagementsForm;
using student_management_fe.Resources;
using Microsoft.Extensions.Localization;

namespace student_management_fe.Views.Pages.Managements;

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
    private readonly IStringLocalizer<Content> _localizer;

    public StudentStatusManagement(StudentStatusService studentStatusService, IStringLocalizer<Content> localizer)
    {
        _studentStatusService = studentStatusService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStudentStatuses();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
    }

    private async Task LoadStudentStatuses(string? search = null)
    {
        studentStatuses = await _studentStatusService.GetStudentStatuses();
        tempStudentStatuses = studentStatuses;
    }

    private void SearchStudentStatus()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            tempStudentStatuses = studentStatuses;
            searchText = null;
        }
        else
        {
            searchText = searchText.Trim();
            tempStudentStatuses = studentStatuses.Where(s => s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                                                             || s.Id.ToString().Contains(searchText,StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    private void HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            SearchStudentStatus();
        }
    }

    private async Task AddStudentStatus()
    {
        var studentStatus = new StudentStatus();
        var parameters =new Dictionary<string, object>
        {
            {"StudentStatus", studentStatus}
        };

        var result = await DialogService.OpenAsync<StudentStatusForm>(_localizer["student_status_management_header_form_add"].Value, parameters);

        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var studentStatusId = await _studentStatusService.AddStudentStatus(studentStatus.Name);
                await LoadStudentStatuses();
                Snackbar.Add($"{_localizer["student_status_management_add_success_noti"].Value}: {studentStatusId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task EditStudentStatus(StudentStatus studentStatus)
    {
        var editStudentStatus = new StudentStatus()
        {
            Id = studentStatus.Id,
            Name = studentStatus.Name
        };

        var parameters = new Dictionary<string, object>
        {
            {"StudentStatus", editStudentStatus },
        };

        var result = await DialogService.OpenAsync<StudentStatusForm>(_localizer["student_status_management_header_form_update"].Value, parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var message = await _studentStatusService.UpdateStudentStatus(editStudentStatus);
                await LoadStudentStatuses();
                Snackbar.Add(message, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

}