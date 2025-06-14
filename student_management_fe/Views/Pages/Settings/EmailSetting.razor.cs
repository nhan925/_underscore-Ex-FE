﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using ServiceStack.Messaging;
using student_management_fe.Models;
using student_management_fe.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using student_management_fe.Resources;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages.Settings;
public partial class EmailSetting
{
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    private string newDomain { get; set; } = string.Empty;

    private ConfigurationsModel<List<string>> configInformations = new()
    {
        Value = new List<string>()
    };

    private readonly IStringLocalizer<Content> _localizer;
    private readonly IConfigurationsService _configurationsService;

    public EmailSetting(IConfigurationsService configurationsService, IStringLocalizer<Content> localizer)
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
            configInformations.Value.Remove(domain);
            await UpdateEmailSetting();
        }
    }

    private async Task LoadEmailSetting()
    {
        configInformations = await _configurationsService.GetEmailConfig();
    }


    private async Task UpdateEmailSetting()
    {
        try
        {
            var message = await _configurationsService.UpdateEmailConfig(configInformations);
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
        await UpdateEmailSetting();
    }


}

