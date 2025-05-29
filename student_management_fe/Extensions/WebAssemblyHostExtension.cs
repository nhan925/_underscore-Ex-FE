using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace student_management_fe.Extensions;

public static class WebAssemblyHostExtension
{
    public async static Task SetDefaultCulture(this WebAssemblyHost host)
    {
        if (host != null)
        {
            const string defaultCulture = "vi-VN";

            var js = host.Services.GetRequiredService<IJSRuntime>();
            var result = await js.InvokeAsync<string>("blazorCulture.get");
            var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

            if (result == null)
            {
                await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
