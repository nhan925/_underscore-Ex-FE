using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Radzen;
using Radzen.Blazor;
using ServiceStack;
using ServiceStack.Text;
using student_management_fe.Resources;
using student_management_fe.Models;
using student_management_fe.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace student_management_fe.Views.Shared;

public partial class StudentForm
{
    [Parameter] public StudentModel Student { get; set; }

    [Parameter] public bool IsUpdateMode { get; set; } = false;

    [Parameter] public List<Faculty> Faculties { get; set; } = new();

    [Parameter] public List<StudentStatus> StudentStatuses { get; set; } = new();

    [Parameter] public List<StudyProgram> StudyPrograms { get; set; } = new();

    [Parameter] public string ButtonText { get; set; }

    [Inject] private IStringLocalizer<Content> _localizer { get; set; }

    [Inject] private Radzen.DialogService DialogService { get; set; } = default!;

    string errorEmailMessage = string.Empty;
    string errorPhoneNumberMessage = string.Empty;
    private bool ShowAddressError { get; set; } = false;
    bool popup = false;

    private Address PermanentAddress { get; set; } = new() { Type = "thuong_tru" };
    private Address TemporaryAddress { get; set; } = new() { Type = "tam_tru" };
    private Address MailingAddress { get; set; } = new() { Type = "nhan_thu" };

    private IdentityInfo IdentityInfo { get; set; }

    private IConfigurationsService _configurationsService;

    public StudentForm(IConfigurationsService configurationsService)
    {
        _configurationsService = configurationsService;
    }

    protected override void OnInitialized()
    {
        if (Student == null)
        {
            Student = new StudentModel
            {
                Addresses = new List<Address>(),
                IdentityInfo = new IdentityInfo(),
            };
        }
        if (Student != null && Student.Addresses != null)
        {
            PermanentAddress = Student.Addresses.FirstOrDefault(a => a.Type == "thuong_tru") ?? new Address { Type = "thuong_tru" };
            TemporaryAddress = Student.Addresses.FirstOrDefault(a => a.Type == "tam_tru") ?? new Address { Type = "tam_tru" };
            MailingAddress = Student.Addresses.FirstOrDefault(a => a.Type == "nhan_thu") ?? new Address { Type = "nhan_thu" };
        }

        if (Student != null && Student.IdentityInfo != null)
        {
            IdentityInfo = Student.IdentityInfo.DeepCopy();
        }
        else
        {
            IdentityInfo = new IdentityInfo();
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
        // Verify updatedIdentityInfo before copy to Student.IdentityInfo if needed
        Student.IdentityInfo = updatedIdentityInfo.DeepCopy();
    }

    private async Task<bool> HandleEmailChange(string email)
    {
        var InvalidEmailMessage = _localizer["student_form_invalid_email"].Value;

        if (string.IsNullOrEmpty(email))
        {
            errorEmailMessage = string.Empty;
            return false;
        }

        var emailRegex = @"^([a-zA-Z0-9._%-]+@[^@]+\.[^@]+)$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            errorEmailMessage = InvalidEmailMessage;
            return false;
        }

        try
        {
            var result = await _configurationsService.CheckConfig("email", email);
            errorEmailMessage = result ? string.Empty : InvalidEmailMessage;
            return result;
        }
        catch (Exception e)
        {
            errorEmailMessage = InvalidEmailMessage;
            return false;
        }
    }

    private async Task<bool> HandlePhoneNumberChange(string phoneNumber)
    {
        var InvalidPhoneMessage = _localizer["student_form_invalid_phone_number"].Value;

        if (string.IsNullOrEmpty(phoneNumber))
        {
            errorPhoneNumberMessage = string.Empty;
            return false;
        }

        try
        {
            var result = await _configurationsService.CheckConfig("phone-number", phoneNumber);
            errorPhoneNumberMessage = result ? string.Empty : InvalidPhoneMessage;
            return result;
        }
        catch (Exception e)
        {
            errorPhoneNumberMessage = InvalidPhoneMessage;
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
        Console.WriteLine($"student id: {Student.Id}");
        OnSubmit();
    }

    void OnSubmit()
    {
        DialogService.Close(true);
    }

    private async Task OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);
}
