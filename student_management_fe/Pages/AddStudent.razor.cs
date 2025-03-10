using Microsoft.AspNetCore.Components;
using student_management_fe.Models;
namespace student_management_fe.Pages;

public partial class AddStudent
{
    private StudentModel Student = new StudentModel();
    [Inject] private NavigationManager Navigation { get; set; }

    private void CloseOverlay()
    {
        Navigation.NavigateTo("/", forceLoad: false);
    }

    private void Add()
    {

    }
}
