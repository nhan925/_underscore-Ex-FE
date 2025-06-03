using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using student_management_fe.Resources;
using student_management_fe.Models;
using student_management_fe.Services;
using static ServiceStack.Diagnostics.Events;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages.Settings;

public partial class StudentStatusSetting
{
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    private readonly ConfigurationsService _configurationsService;
    private readonly StudentStatusService _studentStatusService;
    private readonly IStringLocalizer<Content> _localizer;
  
    private ConfigurationsModel<Dictionary<string, List<int>>> configInformations = new()
    {
        Value = new Dictionary<string, List<int>>()
    };

    private List<StudentStatus> studentStatuses = new();
    private List<StudentStatus> availableNextStatus = new(); //For table
    private IEnumerable<StudentStatus> studentStatusesValidTransfer = Enumerable.Empty<StudentStatus>(); //For Combobox

    private StudentStatus selectedStudentStatus { get; set; } = null;
    private StudentStatus selectedTransferStudentStatus { get; set; } = null;


    public StudentStatusSetting(
        ConfigurationsService configurationsService, 
        StudentStatusService studentServices,
        IStringLocalizer<Content> localizer)
    {
        _configurationsService = configurationsService;
        _studentStatusService = studentServices;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        studentStatuses = await _studentStatusService.GetStudentStatuses();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        await LoadStudentStatusSetting();
        if(studentStatuses.Count != 0)
        {
            selectedStudentStatus = studentStatuses[0];
            await OnSelectedStudentStatusChanged(selectedStudentStatus);
        }
    }

    // Chỉ gọi trong OnInitializedAsync
    private async Task LoadStudentStatusSetting()
    {
        configInformations = await _configurationsService.GetStudentStatusConfig();
    }

    private async Task OnSelectedStudentStatusChanged(StudentStatus status)
    {
        selectedStudentStatus = status;
        if (status != null)
        {
            var tempAvailableNextStatus = await _configurationsService.GetNextStatuses(status.Id);
            availableNextStatus = tempAvailableNextStatus.Where(c => c.Id != selectedStudentStatus.Id).ToList();
            SearchValidTransferStudentStatus();
        }
        else
        {
            availableNextStatus.Clear();
        }
    }

    private void SearchValidTransferStudentStatus()
    {
        selectedTransferStudentStatus = null;
        var key = selectedStudentStatus.Id.ToString();

        if (!configInformations.Value.ContainsKey(key))
        {
            studentStatusesValidTransfer = studentStatuses.Where(c => c.Id != selectedStudentStatus.Id);
        }
        else
        {
            studentStatusesValidTransfer = studentStatuses.Where(c =>
                                               !configInformations.Value[key].Contains(c.Id) &&
                                               c.Id != selectedStudentStatus.Id);
        }
        
    }

    private void OnSelectedTransferStudentStatusChanged(StudentStatus status)
    {
        selectedTransferStudentStatus = status;
    }

    private void OnTransferStatusSelectOpened()
    {
        if (!studentStatusesValidTransfer.Any())
        {
            Snackbar.Add(_localizer["student_status_setting_no_valid_status_warning"], Severity.Warning);
        }
    }

    private async Task AddStudentStatusSetting()
    {
        if (selectedStudentStatus == null)
        {
            Snackbar.Add(_localizer["student_status_setting_source_required_error"], Severity.Error);
            return;
        }
        else
        {
            if (selectedTransferStudentStatus == null)
            {
                Snackbar.Add(_localizer["student_status_setting_destination_required_error"], Severity.Error);
                return;
            }
            else
            {
                var key = selectedStudentStatus.Id.ToString();
                if (!configInformations.Value.ContainsKey(key))
                {
                    configInformations.Value.Add(key, new List<int> { selectedTransferStudentStatus.Id });
                }
                else
                {
                    configInformations.Value[key].Add(selectedTransferStudentStatus.Id);
                }
                availableNextStatus.Add(selectedTransferStudentStatus);
                selectedTransferStudentStatus = null;
            }
        }

        await UpdateStudentStatusSetting();
    }

    private async Task DeleteStudentStatusSetting(int studentStatusId)
    {
        var result = await DialogService.OpenAsync<DeleteConfirmationDialog>(
        title: _localizer["delete_confirmation_dialog_header"],
        parameters: new Dictionary<string, object>
        {
            { "ContentText", _localizer["delete_dialog_confirmation_content"].Value },
            { "ButtonText", _localizer["all_actions_delete_button_text"].Value }
        }
        );
        if (result is bool confirmed && confirmed)
        {
            configInformations.Value[selectedStudentStatus.Id.ToString()].Remove(studentStatusId);
            availableNextStatus.RemoveAll(c => c.Id == studentStatusId);
            await UpdateStudentStatusSetting();
        }
    }

    private async Task UpdateStudentStatusSetting()
    {
        try
        {
            var message = await _configurationsService.UpdateStudentStatusConfig(configInformations);
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Add(message, Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task OnSwitchChange(bool value)
    {
        configInformations.IsActive = value;
        await UpdateStudentStatusSetting();
    }
}
