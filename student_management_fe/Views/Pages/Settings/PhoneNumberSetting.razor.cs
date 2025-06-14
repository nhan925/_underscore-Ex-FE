﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using student_management_fe.Resources;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages.Settings
{
    public partial class PhoneNumberSetting
    {
        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        private Radzen.DialogService DialogService { get; set; } = default!;

        private ConfigurationsModel<List<string>> configInformations = new()
        {
            Value = new List<string>()
        };
        private List<CountryPhoneCodeModel> countryPhoneCodeInformations = new List<CountryPhoneCodeModel>();
        private CountryPhoneCodeModel selectedCountry { get; set; } = null!;
        private List<CountryPhoneCodeModel> mappedCountries = new();

        private readonly IConfigurationsService _configurationsService;
        private readonly ICountryPhoneCodeService _countryPhoneCodeService;
        private readonly IStringLocalizer<Content> _localizer;

        public PhoneNumberSetting(
            IConfigurationsService configurationsService, 
            ICountryPhoneCodeService countryPhoneCodeService, 
            IStringLocalizer<Content> localizer)
        {
            _configurationsService = configurationsService;
            _countryPhoneCodeService = countryPhoneCodeService;
            _localizer = localizer;
        }


        protected override async Task OnInitializedAsync()
        {
            await LoadPhoneNumberSetting();
            MapConfigToPhoneInformation();
        }

        private void MapConfigToPhoneInformation()
        {
            mappedCountries = new List<CountryPhoneCodeModel>();
            foreach (var code in configInformations.Value)
            {
                var countryInfo = countryPhoneCodeInformations.FirstOrDefault(c => c.Code == code);
                if (countryInfo != null)
                {
                    mappedCountries.Add(countryInfo);
                }
            }
        }

        private async Task<IEnumerable<CountryPhoneCodeModel>> SearchCountries(string value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(value))
                return countryPhoneCodeInformations;

            return countryPhoneCodeInformations
                .Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                           c.Code.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                           c.CallingCode.Contains(value, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        private async Task AddPhoneNumberSetting()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;

            if (selectedCountry == null)
            {
                Snackbar.Add(_localizer["phone_number_country_error_1"].Value, Severity.Error);
                return;
            }

            if (configInformations.Value.Contains(selectedCountry.Code))
            {
                Snackbar.Add(_localizer["phone_number_country_error_2"].Value, Severity.Warning);
                selectedCountry = null!;
                return;
            }

            configInformations.Value.Add(selectedCountry.Code);
            MapConfigToPhoneInformation();
            selectedCountry = null!;
            await UpdatePhoneNumberSetting();
        }

        private async Task DeletePhoneNumberSetting(string code)
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
                configInformations.Value.Remove(code);
                MapConfigToPhoneInformation();
                await UpdatePhoneNumberSetting();
            }
        }

        private async Task LoadPhoneNumberSetting()
        {
            configInformations = await _configurationsService.GetPhoneNumberConfig();
            countryPhoneCodeInformations = await _countryPhoneCodeService.GetCountryPhoneCodes();

        }

        private async Task UpdatePhoneNumberSetting()
        {
            try
            {
                var message = await _configurationsService.UpdatePhoneNumberConfig(configInformations);
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
            await UpdatePhoneNumberSetting();
        }

    }
}
