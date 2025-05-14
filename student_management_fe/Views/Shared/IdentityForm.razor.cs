using Microsoft.AspNetCore.Components;
using student_management_fe.Models;
using System.ComponentModel.DataAnnotations;
using static MudBlazor.Colors;

namespace student_management_fe.Views.Shared;

public partial class IdentityForm
{
    [Parameter] public IdentityInfo Value { get; set; }

    [Parameter] public EventCallback<IdentityInfo> OnIdentityInfoUpdated { get; set; } = default!;

    [Parameter] public EventCallback<IdentityInfo> ValueChanged { get; set; } = default!;

    bool popup = false;

    protected override void OnInitialized()
    {
        if (Value.AdditionalInfoForIdentityInfo == null)
        {
            Value.AdditionalInfoForIdentityInfo = new AdditionalInfoForIdentityInfo();
        }
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
