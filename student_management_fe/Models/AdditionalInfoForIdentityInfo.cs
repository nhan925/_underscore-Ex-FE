using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;

namespace student_management_fe.Models;

public class AdditionalInfoForIdentityInfo
{
    [Required(ErrorMessageResourceName = "identity_additional_has_chip_required",
              ErrorMessageResourceType = typeof(Content))]
    public string HasChip { get; set; }

    [Required(ErrorMessageResourceName = "identity_additional_country_of_issue_required",
              ErrorMessageResourceType = typeof(Content))]
    public string CountryOfIssue { get; set; }

    public string Note { get; set; }

    public AdditionalInfoForIdentityInfo Copy()
    {
        return (AdditionalInfoForIdentityInfo)this.MemberwiseClone();
    }
}
