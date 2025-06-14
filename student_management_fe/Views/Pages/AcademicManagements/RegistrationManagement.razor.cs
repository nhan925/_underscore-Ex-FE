﻿using student_management_fe.Models;
using student_management_fe.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using student_management_fe.Views.Shared;
using Microsoft.Extensions.Localization;
using student_management_fe.Resources;

namespace student_management_fe.Views.Pages.AcademicManagements;

public partial class RegistrationManagement
{
    private GetCourseClassResult courseClass = new();

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    private List<StudentInClass> studentsInClass = new();
    private List<Year> years = new();
    private string yearAndSemesterText = string.Empty;
    private StudentInClass? editingStudent;
    private float? editingGrade;
    private float? originalGrade;
    private bool isLoading = true;

    private readonly ICourseClassService _courseClassService;
    private readonly IDataService _dataService;
    private readonly IYearAndSemesterService _yearAndSemesterService;
    private readonly ICourseEnrollmentService _courseErollmentService;
    private readonly IStringLocalizer<Content> _localizer;

    public RegistrationManagement(
        ICourseClassService courseClassService, 
        IDataService dataService, 
        IYearAndSemesterService yearAndSemesterService, 
        ICourseEnrollmentService courseErollmentService,
        IStringLocalizer<Content> localizer)
    {
        _courseClassService = courseClassService;
        _dataService = dataService;
        _yearAndSemesterService = yearAndSemesterService;
        _courseErollmentService = courseErollmentService;
        _localizer = localizer;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            var courseClassTemp = _dataService.GetData<GetCourseClassResult>();
            if (courseClassTemp == null)
            {
                courseClassTemp = await LocalStorage.GetItemAsync<GetCourseClassResult>("cachedCourseClassSelected");
                if (courseClassTemp == null)
                {
                    Snackbar.Add(_localizer["registration_management_classes_info_not_found"], Severity.Warning);
                    return;
                }
            }
            courseClass = await _courseClassService.GetCourseClassByIdAndCourseAndSemester(courseClassTemp.Id, courseClassTemp.Course.Id, courseClassTemp.Semester.Id);
            
            years = await _yearAndSemesterService.GetAllYears();
            if (years != null && courseClass.Semester != null)
            {
                var yearSelected = years.FirstOrDefault(x => x.Id == courseClass.Semester.YearId);
                if (yearSelected != null)
                {
                    yearAndSemesterText = $"{yearSelected.Name}/{courseClass.Semester.SemesterNum}";
                }
            }

            studentsInClass = new List<StudentInClass>();
            if (courseClass != null)
            {
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass) ?? new List<StudentInClass>();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{_localizer["error"]} {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task AddStudentToClass()
    {
        var options = new Radzen.DialogOptions()
        {
            Resizable = false,
            Draggable = false,
            Width = "45%",
            Height = "90%",
            ContentCssClass = "custom-dialog"
        };

        var result = await DialogService.OpenAsync<AddStudentToClassForm>(_localizer["registration_management_enroll_student"], new Dictionary<string, object>
        {
            { "ButtonText", _localizer["registration_management_register_button"].Value },
            { "CourseClass", courseClass }
        }, options);

        if (result is not null) 
        {
            try
            {
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
                Snackbar.Add(result, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private async Task DeleteStudentFromClass(string id)
    {
        var parameters = new Dictionary<string, object>
        {
            { "ContentText", $"{_localizer["registration_management_delete_student_confirmation_content"].Value}: {id} !" },
            { "ButtonText", _localizer["registration_management_unenroll_button"].Value }
        };

        var resultVerify = await DialogService.OpenAsync<DeleteConfirmationDialog>(
            _localizer["registration_management_delete_confirmation_dialog_header"].Value, parameters
        );

        Console.WriteLine($"Dialog result: {resultVerify}");

        if (resultVerify is bool isConfirmed && isConfirmed)
        {
            try
            {
                var studentUnregistered = new CourseEnrollmentRequest
                {
                    StudentId = id,
                    ClassId = courseClass.Id,
                    CourseId = courseClass.Course.Id,
                    SemesterId = courseClass.Semester.Id
                };
                var result = await _courseErollmentService.RegisterAndUnregisterClass(ICourseEnrollmentService.EnrollmentActions.Unregister, studentUnregistered);
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
                Snackbar.Add(result, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
    }

    private void StartEditingGrade(StudentInClass student)
    {
        if (courseClass.Semester.StartDate > DateTime.Now)
        {
            Snackbar.Add(_localizer["registration_management_cannot_update_grade_before_semester"], Severity.Warning);
            return;
        }
        editingStudent = student;
        editingGrade = student.Grade;
        originalGrade = student.Grade;
    }

    private async Task SaveGrade()
    {
        if (editingStudent != null)
        {
            try
            {
                editingStudent.Grade = editingGrade;
                var updateStudentGradeRequest = new UpdateStudentGradeRequest
                {
                    StudentId = editingStudent.Id,
                    CourseId = courseClass.Course.Id,
                    Grade = editingStudent.Grade
                };

                var messgae = await _courseErollmentService.UpdateStudentGrade(updateStudentGradeRequest);
                studentsInClass = await _courseClassService.GetStudentsInClass(courseClass);
                Snackbar.Add(messgae, Severity.Success);
            }
            catch (Exception ex)
            {
                if (editingStudent != null && originalGrade.HasValue)
                    editingStudent.Grade = originalGrade;
              
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally
            {
                editingStudent = null;
                editingGrade = null;
                originalGrade = null;
            }
        }
    }

    private void CancelEdit()
    {
        editingStudent = null;
        editingGrade = null;
        originalGrade = null;
    }
}