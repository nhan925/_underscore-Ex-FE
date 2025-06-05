using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;
using student_management_fe.Models;
using System.ComponentModel.DataAnnotations;
using static MudBlazor.Colors;

namespace student_management_fe.Views.Shared;

public partial class IdentityForm
{
    [Parameter] public IdentityInfo Value { get; set; }

    [Parameter] public EventCallback<IdentityInfo> OnIdentityInfoUpdated { get; set; } = default!;

    [Parameter] public EventCallback<IdentityInfo> ValueChanged { get; set; } = default!;
    [Inject] private IStringLocalizer<Content> _localizer { get; set; }
    bool popup = false;

    protected override void OnInitialized()
    {
        var _ = Value.AdditionalInfoForIdentityInfo;
    }

    private void UpdateAdditionalInfo()
    {
        if (Value.Type == "cccd")
        {
            Value.AdditionalInfo = new Dictionary<string, string>
            {
                ["has_chip"] = Value.AdditionalInfoForIdentityInfo.HasChip,
            };
        }
        else if (Value.Type == "passport")
        {
            Value.AdditionalInfo = new Dictionary<string, string>
            {
                ["country_of_issue"] = Value.AdditionalInfoForIdentityInfo.CountryOfIssue,
                ["note"] = Value.AdditionalInfoForIdentityInfo.Note
            };
        }
        else if (Value.Type == "cmnd")
        {
            Value.AdditionalInfo = null;
        }
        ValidateAndUpdate();
    }

    private async Task ValidateAndUpdate()
    {
        if (OnIdentityInfoUpdated.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
            await OnIdentityInfoUpdated.InvokeAsync(Value);
        }
    }
}
