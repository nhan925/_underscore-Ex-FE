using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using student_management_fe.Models;
using student_management_fe.Services;
using System.Security.AccessControl;
using student_management_fe.Views.Shared;

namespace student_management_fe.Views.Pages.Managements;
public partial class StudyProgramManagement : ComponentBase
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private int _currentPage = 1;
    private int currentPage
    {
        get
        {
            if (totalPages == 0)
            {
                return 1;
            }

            return _currentPage;
        }

        set
        {
            if (value > 0 && value <= totalPages)
            {
                _currentPage = value;
            }
        }
    }
    private int pageSize = 10;
    private int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    private int totalCount { get; set; } = 100;


    private string? searchText;

    private List<StudyProgram> studyPrograms = new List<StudyProgram>();
    private List<StudyProgram> tempStudyPrograms = new List<StudyProgram>();

    private readonly StudyProgramService _studyProgramService;

    public StudyProgramManagement(StudyProgramService studyProgramService)
    {
        _studyProgramService = studyProgramService;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStudyPrograms();
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
            tempStudyPrograms = studyPrograms.Where(x => x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    private void HandleKeyPressSearch(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            SearchStudyProgram();
        }
    }

    private async Task AddStudyProgram()
    {
        var program = new StudyProgram();
        var parameters = new Dictionary<string, object>
        {
            {"StudyProgram", program },
            {"ButtonText", "Lưu" },
            {"TitleText", "Tên chương trình học" },
        };

        var result = await DialogService.OpenAsync<StudyProgramForm>("Thêm chương trình học", parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var studyProgramId = await _studyProgramService.AddProgram(program.Name);
                await LoadStudyPrograms();
                Snackbar.Add($"Đã thêm chương trình học với id {studyProgramId} !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
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
            {"ButtonText", "Cập nhật" },
            {"TitleText", "Tên chương trình học" },
        };

        var result = await DialogService.OpenAsync<StudyProgramForm>("Cập nhật chương trình học", parameters);
        if (result is bool isConfirmed && isConfirmed)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            try
            {
                var message = await _studyProgramService.UpdateProgram(editProgram);
                await LoadStudyPrograms();
                Snackbar.Add($"Đã cập nhật chương trình học thành công !", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }


    //private async Task NextPage()
    //{
    //    if (currentPage < totalPages)
    //    {
    //        currentPage++;
    //        await LoadStudyPrograms();
    //    }
    //}

    //private async Task PreviousPage()
    //{
    //    if (currentPage > 1)
    //    {
    //        currentPage--;
    //        await LoadStudyPrograms();
    //    }
    //}
}
