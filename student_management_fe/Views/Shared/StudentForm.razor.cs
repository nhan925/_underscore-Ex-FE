using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using student_management_fe.Models;


namespace student_management_fe.Views.Shared;

public partial class StudentForm
{
    [Parameter] public StudentModel Student { get; set; } = new();
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

    bool popup = true;
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
