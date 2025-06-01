using Microsoft.AspNetCore.Components;
using MudBlazor;
using ServiceStack.Messaging;
using student_management_fe.Models;
using student_management_fe.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using student_management_fe.Localization;

namespace student_management_fe.Views.Pages.Settings;
public partial class EmailSetting
{
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private readonly ConfigurationsService _configurationsService;
    private string newDomain { get; set; } = string.Empty;
    private readonly IStringLocalizer<Content> _localizer;

    private ConfigurationsModel<List<string>> configInformations = new()
    {
        Value = new List<string>()
    };

    public EmailSetting(ConfigurationsService configurationsService, IStringLocalizer<Content> localizer)
    {
        _configurationsService = configurationsService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadEmailSetting();
    }

    private async Task AddEmailSetting()
    {
        var regex = new System.Text.RegularExpressions.Regex(@"^[^@]+\.[^@]+$");
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

        if (string.IsNullOrWhiteSpace(newDomain))
        {
            Snackbar.Add(_localizer["email_setting_domain_error_1"].Value, Severity.Error);
            return;
        }
        else if (!regex.IsMatch(newDomain))
        {
            Snackbar.Add(_localizer["email_setting_domain_error_2"].Value, Severity.Error);
            return;
        }
        else if (configInformations.Value.Contains(newDomain))
        {
            Snackbar.Add(_localizer["email_setting_domain_error_3"].Value, Severity.Error);
            newDomain = string.Empty;
            return;
        }
        else if (!string.IsNullOrWhiteSpace(newDomain))
        {
            configInformations.Value.Add(newDomain);
            newDomain = string.Empty;
        }

        await UpdateEmailSetting();
    }

    private async Task DeleteEmailSetting(string domain)
    {
        configInformations.Value.Remove(domain);
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        await UpdateEmailSetting();
    }

    private async Task LoadEmailSetting()
    {
        configInformations = await _configurationsService.GetEmailConfig();
    }


    private async Task UpdateEmailSetting()
    {

        var message = await _configurationsService.UpdateEmailConfig(configInformations);
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        Snackbar.Add("Cập nhật thành công!", Severity.Success);
    }

    private async Task OnSwitchChange(bool value)
    {
        configInformations.IsActive = value;
        await UpdateEmailSetting();
    }


}

