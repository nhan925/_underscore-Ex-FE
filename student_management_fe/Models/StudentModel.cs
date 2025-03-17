using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class StudentModel
{
    [Required(ErrorMessage = "Mã sinh viên không được để trống.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Mã sinh viên phải có đúng 8 chữ số.")]
    public string Id { get; set; } // 8-digit student ID (e.g., 22010001)

    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    [StringLength(100, ErrorMessage = "Họ và tên không được dài quá 100 ký tự.")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Ngày sinh không được để trống.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(StudentModel), nameof(ValidateDateOfBirth))]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống.")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Khoa không được để trống.")]
    public int? FacultyId { get; set; }

    [Required(ErrorMessage = "Khóa học không được để trống.")]
    [Range(1900, int.MaxValue, ErrorMessage = "Khóa học không hợp lệ.")]
    [CustomValidation(typeof(StudentModel), nameof(ValidateIntakeYear))]
    public int? IntakeYear { get; set; }

    //[Required(ErrorMessage = "Chương trình học không được để trống.")]
    //public int? ProgramId { get; set; }

    [Required(ErrorMessage = "Chương trình học không được để trống.")]
    public string? Program { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống.")]
    [StringLength(200, ErrorMessage = "Địa chỉ không được dài quá 200 ký tự.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Trạng thái sinh viên không được để trống.")]
    public int? StatusId { get; set; }

    
    public static ValidationResult ValidateDateOfBirth(DateTime? date, ValidationContext context)
    {
        if (date.HasValue && date.Value > DateTime.Now)
        {
            return new ValidationResult("Ngày sinh không thể lớn hơn ngày hiện tại.");
        }
        return ValidationResult.Success;
    }

    public static ValidationResult ValidateIntakeYear(int? year, ValidationContext context)
    {
        if (year.HasValue && year.Value > DateTime.Now.Year)
        {
            return new ValidationResult("Khóa học không thể lớn hơn năm hiện tại.");
        }
        return ValidationResult.Success;
    }
}