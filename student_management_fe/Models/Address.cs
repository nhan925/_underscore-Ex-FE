using student_management_fe.Resources;
using System.ComponentModel.DataAnnotations;
namespace student_management_fe.Models;

public class Address
{
    [Required(ErrorMessageResourceName = "address_model_type_required",
              ErrorMessageResourceType = typeof(Content))]
    public string Type { get; set; }

    [Required(ErrorMessageResourceName = "address_model_other_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(200, ErrorMessageResourceName = "address_model_other_maxlength",
                       ErrorMessageResourceType = typeof(Content))]
    public string? Other { get; set; } 

    [Required(ErrorMessageResourceName = "address_model_village_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "address_model_village_maxlength",
                       ErrorMessageResourceType = typeof(Content))]
    public string Village { get; set; }

    [Required(ErrorMessageResourceName = "address_model_district_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "address_model_district_maxlength",
                       ErrorMessageResourceType = typeof(Content))]
    public string District { get; set; }

    [Required(ErrorMessageResourceName = "address_model_city_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "address_model_city_maxlength",
                       ErrorMessageResourceType = typeof(Content))]
    public string City { get; set; }

    [Required(ErrorMessageResourceName = "address_model_country_required",
              ErrorMessageResourceType = typeof(Content))]
    [StringLength(100, ErrorMessageResourceName = "address_model_country_maxlength",
                       ErrorMessageResourceType = typeof(Content))]
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
