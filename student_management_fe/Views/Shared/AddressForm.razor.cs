﻿using Microsoft.AspNetCore.Components;
using student_management_fe.Models;

namespace student_management_fe.Views.Shared;

public partial class AddressForm
{
    [Parameter] public Address Value { get; set; }
    [Parameter] public EventCallback<Address> ValueChanged { get; set; }
    [Parameter] public EventCallback<Address> OnAddressUpdated { get; set; }

    private async Task ValidateAndUpdate()
    {
        if (OnAddressUpdated.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value); 
            await OnAddressUpdated.InvokeAsync(Value); 
        }
    }

    bool popup = false;
}
