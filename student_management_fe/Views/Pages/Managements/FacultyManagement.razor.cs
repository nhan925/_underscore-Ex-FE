﻿using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using student_management_fe.Views.Shared.ManagementsForm;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Views.Pages.Managements;
public partial class FacultyManagement
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private string? searchText;

    private List<Faculty> faculties = new List<Faculty>();
    private List<Faculty> tempFaculties = new List<Faculty>();

    private readonly IFacultyService _facultyService;
    private readonly IStringLocalizer<Content> _localizer;

    public FacultyManagement(IFacultyService facultyService, IStringLocalizer<Content> localizer)
    {
        _facultyService = facultyService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadFaculties();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
    }

    private async Task LoadFaculties(string? search = null)
    {
        faculties = await _facultyService.GetFaculties();
        tempFaculties = faculties;
    }

    private void SearchFaculty()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            tempFaculties = faculties;
            searchText = null;
        }
        else
        {
            searchText = searchText.Trim();
            tempFaculties = faculties.Where(f => f.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                                                 || f.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    private void HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            SearchFaculty();
        }
    }

    private async Task EditFaculty(Faculty faculty)
    {
        var editFaculty = new Faculty
        {
            Id = faculty.Id,
            Name = faculty.Name
        };

        var parameters = new Dictionary<string, object>
        {
            {"Faculty", editFaculty }
        };

        var result = await DialogService.OpenAsync<FacultyForm>(_localizer["faculty_management_header_form_add"].Value, parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var message = await _facultyService.UpdateFaculty(editFaculty);
                await LoadFaculties();
                Snackbar.Add(message, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task AddFaculty()
    {
        var faculty = new Faculty();
        var parameters = new Dictionary<string, object>
        {
            {"Faculty", faculty }
        };

        var result = await DialogService.OpenAsync<FacultyForm>(_localizer["faculty_management_header_form_update"].Value, parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var facultyId = await _facultyService.AddFaculty(faculty.Name);
                await LoadFaculties();
                Snackbar.Add($"{_localizer["faculty_management_add_success_noti"].Value}: {facultyId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

}
