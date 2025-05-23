﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;

namespace student_management_fe.Views.Pages.Settings
{
    public partial class PhoneNumberSetting
    {

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        private readonly ConfigurationsService _configurationsService;
        private readonly CountryPhoneCodeService _countryPhoneCodeService;

        private ConfigurationsModel<List<string>> configInformations = new()
        {
            Value = new List<string>()
        };
        private List<CountryPhoneCodeModel> countryPhoneCodeInformations = new List<CountryPhoneCodeModel>();
        private CountryPhoneCodeModel selectedCountry { get; set; } = null!;
        private List<CountryPhoneCodeModel> mappedCountries = new();

        public PhoneNumberSetting(ConfigurationsService configurationsService, CountryPhoneCodeService countryPhoneCodeService)
        {
            _configurationsService = configurationsService;
            _countryPhoneCodeService = countryPhoneCodeService;
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
                Snackbar.Add("Vui lòng chọn quốc gia", Severity.Error);
                return;
            }

            if (configInformations.Value.Contains(selectedCountry.Code))
            {
                Snackbar.Add("Quốc gia này đã được thêm vào danh sách", Severity.Warning);
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
            configInformations.Value.Remove(code);
            MapConfigToPhoneInformation();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            await UpdatePhoneNumberSetting();
        }

        private async Task LoadPhoneNumberSetting()
        {
            configInformations = await _configurationsService.GetPhoneNumberConfig();
            countryPhoneCodeInformations = await _countryPhoneCodeService.GetCountryPhoneCodes();

        }

        private async Task UpdatePhoneNumberSetting()
        {

            var message = await _configurationsService.UpdatePhoneNumberConfig(configInformations);
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Add("Cập nhật thành công", Severity.Success);
        }

        private async Task OnSwitchChange(bool value)
        {
            configInformations.IsActive = value;
            await UpdatePhoneNumberSetting();
        }

    }
}
