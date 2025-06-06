using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;
using student_management_fe.Resources;
using student_management_fe.Resources;

namespace student_management_fe.Models;

public class StudentModel
{
    [Required(ErrorMessageResourceName = "student_model_id_required",
              ErrorMessageResourceType = typeof(Content))]
    [RegularExpression(@"^\d{8}$",
                ErrorMessageResourceName = "student_model_id_invalid_format",
                ErrorMessageResourceType = typeof(Content))]
    public string Id { get; set; } // 8-digit student ID (e.g., 22010001)


    [Required(ErrorMessageResourceName= "student_model_fullname_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "student_model_fullname_maxlength",
                       ErrorMessageResourceType = typeof(Content))]
    public string? FullName { get; set; }


    [Required(ErrorMessageResourceName = "student_model_date_of_birth_required",
              ErrorMessageResourceType = typeof(Content))]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(StudentModel), nameof(ValidateDateOfBirth))]
    public DateTime? DateOfBirth { get; set; }


    [Required(ErrorMessageResourceName = "student_model_gender_required",
              ErrorMessageResourceType = typeof(Content))]
    public string? Gender { get; set; }


    [Required(ErrorMessageResourceName = "student_model_faculty_required",
              ErrorMessageResourceType = typeof(Content))]
    public int? FacultyId { get; set; }


    [Required(ErrorMessageResourceName = "student_model_intake_year_required",
              ErrorMessageResourceType = typeof(Content))]
    [Range(1900, int.MaxValue, ErrorMessageResourceName = "student_model_intake_year_invalid",
                               ErrorMessageResourceType = typeof(Content))]
    [CustomValidation(typeof(StudentModel), nameof(ValidateIntakeYear))]
    public int? IntakeYear { get; set; }


    [Required(ErrorMessageResourceName = "student_model_program_required",
              ErrorMessageResourceType = typeof(Content))]
    public int? ProgramId { get; set; }


    [Required(ErrorMessageResourceName = "student_model_addresses_required",
              ErrorMessageResourceType = typeof(Content))]
    [CustomValidation(typeof(StudentModel), nameof(ValidateAddresses))]
    public List<Address> Addresses { get; set; }


    [Required(ErrorMessageResourceName = "student_model_identity_info_required",
              ErrorMessageResourceType = typeof(Content))]
    public IdentityInfo IdentityInfo { get; set; }


    [Required(ErrorMessageResourceName = "student_model_email_required",
              ErrorMessageResourceType = typeof(Content))]
    public string? Email { get; set; }


    [Required(ErrorMessageResourceName = "student_model_phone_required",
              ErrorMessageResourceType = typeof(Content))]
    public string? PhoneNumber { get; set; }


    [Required(ErrorMessageResourceName = "student_model_status_required",
              ErrorMessageResourceType = typeof(Content))]
    public int? StatusId { get; set; }


    [Required(ErrorMessageResourceName = "student_model_nationality_required",
              ErrorMessageResourceType = typeof(Content))]
    public string? Nationality { get; set; }


    public static ValidationResult ValidateDateOfBirth(DateTime? date, ValidationContext context)
    {
        if (date.HasValue && date.Value > DateTime.Now)
        {
            return new ValidationResult(Content.student_model_date_of_birth_future_invalid);
        }
        return ValidationResult.Success;
    }

    public static ValidationResult ValidateIntakeYear(int? year, ValidationContext context)
    {
        if (year.HasValue && year.Value > DateTime.Now.Year)
        {
            return new ValidationResult(Content.student_model_intake_year_in_future);
        }
        return ValidationResult.Success;
    }

    public static ValidationResult ValidateAddresses(List<Address> value, ValidationContext validationContext)
    {

        bool hasThuongTru = value.Any(a => a.Type == "thuong_tru");
        if (!hasThuongTru)
        {
            return new ValidationResult(Content.student_model_address_must_have_thuong_tru);
        }
        return ValidationResult.Success;
    }
}