using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using static ServiceStack.Diagnostics.Events;

namespace student_management_fe.Views.Pages.Settings;

public partial class StudentStatusSetting
{
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private readonly ConfigurationsService _configurationsService;
    private readonly StudentStatusService _studentStatusService;

    private ConfigurationsModel<Dictionary<string, List<int>>> configInformations = new()
    {
        Value = new Dictionary<string, List<int>>()
    };

    private List<StudentStatus> studentStatuses = new();

    private StudentStatus selectedStudentStatus { get; set; } = null;
    private StudentStatus selectedTransferStudentStatus { get; set; } = null;



    public StudentStatusSetting(ConfigurationsService configurationsService, StudentStatusService studentServices)
    {
        _configurationsService = configurationsService;
        _studentStatusService = studentServices;
    }

    protected override async Task OnInitializedAsync()
    {
        studentStatuses = await _studentStatusService.GetStudentStatuses();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        await LoadStudentStatusSetting();
    }

    private async Task AddStudentStatusSetting()
    {
        if (selectedStudentStatus == null)
        {
            Snackbar.Add("Tên trạng thái không được để trống", Severity.Error);
            return;
        }
        else
        {
            if (selectedTransferStudentStatus == null)
            {
                Snackbar.Add("Tên trạng thái chuyển đến không được để trống", Severity.Error);
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
                selectedTransferStudentStatus = null;
            }
        }

        await UpdateStudentStatusSetting();
    }

    private async Task DeleteStudentStatusSetting(int studentStatusId)
    {
        configInformations.Value[selectedStudentStatus.Id.ToString()].Remove(studentStatusId);
        await UpdateStudentStatusSetting();
    }

    // Chỉ gọi trong OnInitializedAsync
    private async Task LoadStudentStatusSetting()
    {
        configInformations = await _configurationsService.GetStudentStatusConfig();
    }


    private async Task UpdateStudentStatusSetting()
    {
        var message = await _configurationsService.UpdateStudentStatusConfig(configInformations);
        Snackbar.Add("Cập nhật thành công!", Severity.Success);
    }

    private async Task OnSwitchChange(bool value)
    {
        configInformations.IsActive = value;
        await UpdateStudentStatusSetting();
    }

    private async Task<IEnumerable<StudentStatus>> SearchStudentStatus(string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(value))
            return studentStatuses;

        return studentStatuses
            .Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                       c.Id.Equals(value))
            .ToList();
    }

    private async Task<IEnumerable<StudentStatus>> SearchTransferStudentStatus(string value, CancellationToken cancellationToken)
    {
        IEnumerable<StudentStatus> studentStatusesValidTransfer = Enumerable.Empty<StudentStatus>();
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
        if (!studentStatusesValidTransfer.Any())
        {
            Snackbar.Add("Không còn trạng thái hợp lệ nào để thêm!", Severity.Warning);
            return Enumerable.Empty<StudentStatus>();
        }

        if (string.IsNullOrEmpty(value))
            return studentStatusesValidTransfer;

        return studentStatusesValidTransfer
            .Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                       c.Id.Equals(value))
            .ToList();
    }
}
