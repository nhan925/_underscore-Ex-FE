using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Radzen;
using Radzen.Blazor;
using ServiceStack;
using ServiceStack.Text;
using student_management_fe.Models;
using student_management_fe.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace student_management_fe.Views.Shared;

public partial class StudentForm
{
    [Parameter]
    public StudentModel Student { get; set; } = new StudentModel
    {
        Addresses = new List<Address>(),
        IdentityInfo = new IdentityInfo()
    };

    [Parameter] public bool IsUpdateMode { get; set; } = false;

    [Parameter] public List<Faculty> Faculties { get; set; } = new();

    [Parameter] public List<StudentStatus> StudentStatuses { get; set; } = new();

    [Parameter] public List<StudyProgram> StudyPrograms { get; set; } = new();

    [Parameter] public string ButtonText { get; set; }

    [Inject] private Radzen.DialogService DialogService { get; set; } = default!;


    string errorEmailMessage = string.Empty;
    string errorPhoneNumberMessage = string.Empty;
    bool popup = false;
    private bool ShowAddressError { get; set; } = false;

    private ConfigurationsService _configurationsService;

    private Address PermanentAddress { get; set; } = new() { Type = "thuong_tru" };
    private Address TemporaryAddress { get; set; } = new() { Type = "tam_tru" };
    private Address MailingAddress { get; set; } = new() { Type = "nhan_thu" };
    private IdentityInfo IdentityInfo { get; set; } = new();

    class AdditionalInfo
    {
        [Required(ErrorMessage = "Thông tin này không được để trống.")]
        public string HasChip { get; set; }

        [Required(ErrorMessage = "Thông tin này không được để trống.")]
        public string CountryOfIssue { get; set; }

        [Required]
        public string Note { get; set; }
    }
    private AdditionalInfo AdditionalInfoModel { get; set; } = new();

    public StudentForm(ConfigurationsService configurationsService)
    {
        _configurationsService = configurationsService;
    }

    protected override void OnInitialized()
    {
        if (Student != null && Student.Addresses != null)
        {
            PermanentAddress = Student.Addresses.FirstOrDefault(a => a.Type == "thuong_tru") ?? new Address { Type = "thuong_tru" };
            TemporaryAddress = Student.Addresses.FirstOrDefault(a => a.Type == "tam_tru") ?? new Address { Type = "tam_tru" };
            MailingAddress = Student.Addresses.FirstOrDefault(a => a.Type == "nhan_thu") ?? new Address { Type = "nhan_thu" };
        }
        if (Student != null && Student.IdentityInfo != null)
        {
            IdentityInfo = Student.IdentityInfo;

            if (IdentityInfo.AdditionalInfo != null)
            {
                IdentityInfo.AdditionalInfo.TryGetValue("country_of_issue", out var country);
                IdentityInfo.AdditionalInfo.TryGetValue("has_chip", out var hasChip);
                IdentityInfo.AdditionalInfo.TryGetValue("note", out var note);

                AdditionalInfoModel.CountryOfIssue = country;
                AdditionalInfoModel.HasChip = hasChip;
                AdditionalInfoModel.Note = note;
            }
        }
    }

    private void HandleAddressUpdate(Address updatedAddress)
    {
        var existing = Student.Addresses.FirstOrDefault(a => a.Type == updatedAddress.Type);

        bool hasEmptyField = string.IsNullOrWhiteSpace(updatedAddress.Other) ||
                             string.IsNullOrWhiteSpace(updatedAddress.Village) ||
                             string.IsNullOrWhiteSpace(updatedAddress.District) ||
                             string.IsNullOrWhiteSpace(updatedAddress.City) ||
                             string.IsNullOrWhiteSpace(updatedAddress.Country);

        if (hasEmptyField)
        {
            if (existing != null)
            {
                Student.Addresses.Remove(existing);
            }
        }
        else
        {
            if (existing == null)
            {
                Student.Addresses.Add(updatedAddress);
            }
        }
    }

    private void HandleIdentityInfoUpdate(IdentityInfo updatedIdentityInfo)
    {
        if (updatedIdentityInfo.Type == "cccd")
        {
            updatedIdentityInfo.AdditionalInfo = new Dictionary<string, string>
            {
                ["has_chip"] = AdditionalInfoModel.HasChip,
            };
        }
        else if (updatedIdentityInfo.Type == "passport")
        {
            updatedIdentityInfo.AdditionalInfo = new Dictionary<string, string>
            {
                ["country_of_issue"] = AdditionalInfoModel.CountryOfIssue,
                ["note"] = AdditionalInfoModel.Note
            };
        }
    }

    private async Task<bool> HandleEmailChange(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            errorEmailMessage = string.Empty;
            return false;
        }

        var emailRegex = @"^([a-zA-Z0-9._%-]+@[^@]+\.[^@]+)$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            errorEmailMessage = "Email không hợp lệ.";
            return false;
        }

        try
        {
            var result = await _configurationsService.CheckConfig("email", email);
            errorEmailMessage = result ? string.Empty : "Email không hợp lệ.";
            return result;
        }
        catch (Exception e)
        {
            errorEmailMessage = "Email không hợp lệ.";
            return false;
        }
    }

    private async Task<bool> HandlePhoneNumberChange(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            errorPhoneNumberMessage = string.Empty;
            return false;
        }

        try
        {
            var result = await _configurationsService.CheckConfig("phone-number", phoneNumber);
            errorPhoneNumberMessage = result ? string.Empty : "Số điện thoại không hợp lệ.";
            return result;
        }
        catch (Exception e)
        {
            errorPhoneNumberMessage = "Số điện thoại không hợp lệ.";
            return false;
        }
    }

    private async Task ValidateAndSubmit()
    {
        if (Student.Addresses == null || !Student.Addresses.Any(a => a.Type == "thuong_tru"))
        {
            ShowAddressError = true;
            return;
        }
        ShowAddressError = false;

        bool isEmailValid = await HandleEmailChange(Student.Email);
        bool isPhoneValid = await HandlePhoneNumberChange(Student.PhoneNumber);

        if (!isEmailValid || !isPhoneValid)
        {
            return;
        }

        HandleIdentityInfoUpdate(IdentityInfo);
        OnSubmit(Student);
    }

    void OnSubmit(StudentModel student)
    {
        DialogService.Close(true);
    }

    private async Task OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
