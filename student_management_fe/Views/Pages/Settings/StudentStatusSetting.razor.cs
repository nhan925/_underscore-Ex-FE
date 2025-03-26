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
        if (selectedTransferStudentStatus == null)
        {
            Snackbar.Add("Tên trạng thái không được để trống", Severity.Error);
            return;
        }
        if (configInformations.Value.ContainsKey(selectedStudentStatus.Id.ToString()) &&  
            configInformations.Value[selectedStudentStatus.Id.ToString()].Contains(selectedTransferStudentStatus.Id))
        {
            Snackbar.Add("Trạng thái này đã được thêm vào danh sách", Severity.Error);
            selectedTransferStudentStatus = null;
            return;
        }
        else if (selectedTransferStudentStatus != null)
        {
            configInformations.Value[selectedStudentStatus.Id.ToString()].Add(selectedTransferStudentStatus.Id);
            selectedTransferStudentStatus = null;
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
        var studentStatusesValidTransfer = studentStatuses.Where(c => 
                                           !configInformations.Value[selectedStudentStatus.Id.ToString()].Contains(c.Id) &&
                                           c.Id != selectedStudentStatus.Id);

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
