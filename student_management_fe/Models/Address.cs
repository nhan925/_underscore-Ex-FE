using System.ComponentModel.DataAnnotations;
namespace student_management_fe.Models;

public class Address
{
    [Required(ErrorMessage = "Loại địa chỉ không được để trống.")]
    public string Type { get; set; }

    [Required(ErrorMessage = "Số nhà, Tên đường không được để trống.")]
    [StringLength(200, ErrorMessage = "Số nhà, Tên đường không được dài quá 200 ký tự.")]
    public string? Other { get; set; } 

    [Required(ErrorMessage = "Phường/Xã không được để trống.")]
    [StringLength(100, ErrorMessage = "Phường/Xã không được dài quá 100 ký tự.")]
    public string Village { get; set; }

    [Required(ErrorMessage = "Quận/Huyện không được để trống.")]
    [StringLength(100, ErrorMessage = "Quận/Huyện không được dài quá 100 ký tự.")]
    public string District { get; set; }

    [Required(ErrorMessage = "Tỉnh/Thành phố không được để trống.")]
    [StringLength(100, ErrorMessage = "Tỉnh/Thành phố không được dài quá 100 ký tự.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Quốc gia không được để trống.")]
    [StringLength(100, ErrorMessage = "Quốc gia không được dài quá 100 ký tự.")]
    public string Country { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Other) &&
               !string.IsNullOrWhiteSpace(Village) &&
               !string.IsNullOrWhiteSpace(District) &&
               !string.IsNullOrWhiteSpace(City) &&
               !string.IsNullOrWhiteSpace(Country);
    }
}
