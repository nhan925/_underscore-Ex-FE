using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class AdditionalInfoForIdentityInfo
{
    [Required(ErrorMessage = "Thông tin này không được để trống.")]
    public string HasChip { get; set; }

    [Required(ErrorMessage = "Thông tin này không được để trống.")]
    public string CountryOfIssue { get; set; }

    [Required]
    public string Note { get; set; }

    public AdditionalInfoForIdentityInfo Copy()
    {
        return (AdditionalInfoForIdentityInfo)this.MemberwiseClone();
    }
}
