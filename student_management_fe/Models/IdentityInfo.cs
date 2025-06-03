using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static student_management_fe.Views.Shared.UploadFile;

namespace student_management_fe.Models;

public class IdentityInfo
{
    

    [Required(ErrorMessageResourceName = "identity_info_type_required",
              ErrorMessageResourceType = typeof(Content))]
    [RegularExpression("^(cmnd|cccd|passport)$", ErrorMessageResourceName = "identity_info_type_invalid",
                                                 ErrorMessageResourceType = typeof(Content))]
    public string Type { get; set; }


    [Required(ErrorMessageResourceName = "identity_info_number_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(12, ErrorMessageResourceName = "identity_info_number_length",
                      ErrorMessageResourceType = typeof(Content))]
    public string Number { get; set; }


    [Required(ErrorMessageResourceName = "identity_info_place_of_issue_required",
              ErrorMessageResourceType = typeof(Content))]
    public string PlaceOfIssue { get; set; }


    [Required(ErrorMessageResourceName = "identity_info_date_of_issue_required",
              ErrorMessageResourceType = typeof(Content))]
    [DataType(DataType.Date)]
    public DateTime? DateOfIssue { get; set; }


    [Required(ErrorMessageResourceName = "identity_info_expiry_date_required",
              ErrorMessageResourceType = typeof(Content))]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(IdentityInfo), nameof(ValidateExpiryDate))]
    public DateTime? ExpiryDate { get; set; }


    public Dictionary<string, string>? AdditionalInfo { get; set; }

    private AdditionalInfoForIdentityInfo? _additionalInfoForIdentityInfo;

    [NotMapped]
    public AdditionalInfoForIdentityInfo? AdditionalInfoForIdentityInfo
    {
        get
        {
            if (_additionalInfoForIdentityInfo == null)
            {
                _additionalInfoForIdentityInfo = new AdditionalInfoForIdentityInfo
                {
                    HasChip = AdditionalInfo?.GetValueOrDefault("has_chip") ?? "",
                    CountryOfIssue = AdditionalInfo?.GetValueOrDefault("country_of_issue") ?? "",
                    Note = AdditionalInfo?.GetValueOrDefault("note") ?? ""
                };
            }
            return _additionalInfoForIdentityInfo;
        }
        set
        {
            if (value == null)
            {
                value = new AdditionalInfoForIdentityInfo();
                value.HasChip = AdditionalInfo?.GetValueOrDefault("has_chip") ?? "";
                value.CountryOfIssue = AdditionalInfo?.GetValueOrDefault("country_of_issue") ?? "";
                value.Note = AdditionalInfo?.GetValueOrDefault("note") ?? "";
            }

            _additionalInfoForIdentityInfo = value;
        }
    }


    public IdentityInfo DeepCopy()
    {
        var copy = (IdentityInfo)this.MemberwiseClone();

        if (this.AdditionalInfo != null)
        {
            copy.AdditionalInfo = new Dictionary<string, string>(this.AdditionalInfo);
        }

        return copy;
    }

    public static ValidationResult ValidateExpiryDate(DateTime? expiryDate, ValidationContext context)
    {
        var instance = context.ObjectInstance as IdentityInfo;
        if (instance == null) return ValidationResult.Success;

        if (expiryDate.HasValue && instance.DateOfIssue.HasValue && expiryDate < instance.DateOfIssue)
        {
            return new ValidationResult(Content.identity_info_expiry_date_invalid);
        }

        return ValidationResult.Success;
    }
}
