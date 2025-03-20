using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class IdentityInfo
{
    [Required(ErrorMessage = "Loại giấy tờ không được để trống.")]
    [RegularExpression("^(cmnd|cccd|passport)$", ErrorMessage = "Loại giấy tờ phải là 'cmnd', 'cccd' hoặc 'passport'.")]
    public string Type { get; set; }


    [Required(ErrorMessage = "Số giấy tờ không được để trống.")]
    [StringLength(12, ErrorMessage = "Số giấy tờ không được quá 12 ký tự.")]
    public string Number { get; set; }


    [Required(ErrorMessage = "Nơi cấp không được để trống.")]
    public string PlaceOfIssue { get; set; }


    [Required(ErrorMessage = "Ngày cấp không được để trống.")]
    [DataType(DataType.Date)]
    public DateTime? DateOfIssue { get; set; }


    [Required(ErrorMessage = "Ngày hết hạn không được để trống.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(IdentityInfo), nameof(ValidateExpiryDate))]
    public DateTime? ExpiryDate { get; set; }


    public Dictionary<string, string>? AdditionalInfo { get; set; }

    public static ValidationResult ValidateExpiryDate(DateTime? expiryDate, ValidationContext context)
    {
        var instance = context.ObjectInstance as IdentityInfo;
        if (instance == null) return ValidationResult.Success;

        if (expiryDate.HasValue && instance.DateOfIssue.HasValue && expiryDate < instance.DateOfIssue)
        {
            return new ValidationResult("Ngày hết hạn phải lớn hơn hoặc bằng ngày cấp.");
        }

        return ValidationResult.Success;
    }
}
