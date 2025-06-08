using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using System.Security.AccessControl;
using student_management_fe.Views.Shared.ManagementsForm;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Views.Pages.Managements;
public partial class StudyProgramManagement : ComponentBase
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private string? searchText;

    private List<StudyProgram> studyPrograms = new List<StudyProgram>();
    private List<StudyProgram> tempStudyPrograms = new List<StudyProgram>();

    private readonly IStudyProgramService _studyProgramService;
    private readonly IStringLocalizer<Content> _localizer;

    public StudyProgramManagement(IStudyProgramService studyProgramService, IStringLocalizer<Content> localizer)
    {
        _studyProgramService = studyProgramService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStudyPrograms();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
    }

    private async Task LoadStudyPrograms()
    {
        studyPrograms = await _studyProgramService.GetPrograms();
        tempStudyPrograms = studyPrograms;
    }

    private void SearchStudyProgram()
    {
        if (String.IsNullOrEmpty(searchText))
        {
            tempStudyPrograms = studyPrograms;
            searchText = null;
        }
        else
        {
            searchText = searchText.Trim();
            tempStudyPrograms = studyPrograms.Where(x => x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                                                         || x.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    private void HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            SearchStudyProgram();
        }
    }

    private async Task EditStudyProgram(StudyProgram program)
    {
        var editProgram = new StudyProgram
        {
            Id = program.Id,
            Name = program.Name
        };

        var parameters = new Dictionary<string, object>
        {
            {"StudyProgram", editProgram },
        };

        var result = await DialogService.OpenAsync<StudyProgramForm>(_localizer["study_program_management_header_form_add"].Value, parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var message = await _studyProgramService.UpdateProgram(editProgram);
                await LoadStudyPrograms();
                Snackbar.Add(message, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task AddStudyProgram()
    {
        var program = new StudyProgram();
        var parameters = new Dictionary<string, object>
        {
            {"StudyProgram", program },
        };

        var result = await DialogService.OpenAsync<StudyProgramForm>(_localizer["study_program_management_header_form_update"].Value, parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            try
            {
                var studyProgramId = await _studyProgramService.AddProgram(program.Name);
                await LoadStudyPrograms();
                Snackbar.Add($"{_localizer["study_program_management_add_success_noti"].Value}: {studyProgramId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }
}
