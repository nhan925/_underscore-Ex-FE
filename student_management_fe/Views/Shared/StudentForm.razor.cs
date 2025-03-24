using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Radzen;
using student_management_fe.Models;
using student_management_fe.Services;
using System.ComponentModel.DataAnnotations;


namespace student_management_fe.Views.Shared;

public partial class StudentForm
{
    [Parameter]
    public StudentModel Student { get; set; } = new StudentModel
    {
        Addresses = new List<Address>(),
        IdentityInfo = new IdentityInfo()
    };
    //[Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public bool IsUpdateMode { get; set; } = false;

    [Parameter]
    public List<Faculty> Faculties { get; set; } = new();

    [Parameter]
    public List<StudentStatus> StudentStatuses { get; set; } = new();

    [Parameter]
    public List<StudyProgram> StudyPrograms { get; set; } = new();

    [Inject] private Radzen.DialogService DialogService { get; set; } = default!;

    [Parameter]
    public string ButtonText { get; set; }

    string errorEmailMessage = string.Empty;
    string errorPhoneMessage = string.Empty;
    bool popup = true;

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

    private readonly ConfigurationsService _configurationsService;
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
        if (existing == null)
        {
            Student.Addresses.Add(updatedAddress);
        }
        else
        {
            existing.Other = updatedAddress.Other;
            existing.Village = updatedAddress.Village;
            existing.District = updatedAddress.District;
            existing.City = updatedAddress.City;
            existing.Country = updatedAddress.Country;
        }
    }

    private void HandleIdentityInfoUpdate(IdentityInfo updatedIdentityInfo)
    {
        if(updatedIdentityInfo.Type == "cccd")
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
        if (Student.IdentityInfo == null)
        {
            Student.IdentityInfo = updatedIdentityInfo;
        }
        else
        {
            Student.IdentityInfo.Type = updatedIdentityInfo.Type;
            Student.IdentityInfo.Number = updatedIdentityInfo.Number;
            Student.IdentityInfo.PlaceOfIssue = updatedIdentityInfo.PlaceOfIssue;
            Student.IdentityInfo.DateOfIssue = updatedIdentityInfo.DateOfIssue;
            Student.IdentityInfo.ExpiryDate = updatedIdentityInfo.ExpiryDate;
            Student.IdentityInfo.AdditionalInfo = updatedIdentityInfo.AdditionalInfo;
        }
    }

    private async Task ValidateEmail()
    {
        if (string.IsNullOrWhiteSpace(Student.Email))
        {
            errorEmailMessage = "Email không được để trống.";
            return;

        }
        try
        {
            var result = await _configurationsService.CheckConfig("email", Student.Email);
            if (!result)
            {
                errorEmailMessage = "Email không hợp lệ.";
            }
            else {
                errorEmailMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            errorEmailMessage = ex.Message;
            Console.WriteLine(ex.Message);
        }
    }
    private bool IsEmailValid()
    {
        return !string.IsNullOrWhiteSpace(errorEmailMessage);
    }

    private async Task ValidatePhoneNumber()
    {
        if (string.IsNullOrWhiteSpace(Student.PhoneNumber))
        {
            errorPhoneMessage = "Số điện thoại không được để trống.";
            return;

        }
        try
        {
            var result = await _configurationsService.CheckConfig("phone-number", Student.PhoneNumber);
            if (!result)
            {
                errorPhoneMessage = "Số điện thoại không hợp lệ.";
            }
            else
            {
                errorPhoneMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            errorPhoneMessage = ex.Message;
            Console.WriteLine(ex.Message);
        }
    }
    private bool IsPhoneValid()
    {
        return !string.IsNullOrWhiteSpace(errorPhoneMessage);
    }

    private void ValidateAndSubmit()
    {
        
        if (Student.Addresses == null || !Student.Addresses.Any(a => a.Type == "thuong_tru"))
        {
            ShowAddressError = true;
            return; 
        }
       
        ShowAddressError = false;
        HandleIdentityInfoUpdate(IdentityInfo);
        OnSubmit(Student);
    }

    private bool ShowAddressError { get; set; } = false;

    void OnSubmit(StudentModel student)
    {
        DialogService.Close(true);
    }

    void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("Invalid submit");
    }

    private void Cancel() => DialogService.Close(false);

}
