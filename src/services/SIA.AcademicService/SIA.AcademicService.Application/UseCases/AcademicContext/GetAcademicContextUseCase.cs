using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Contracts.Requests.AcademicContext;
using SIA.AcademicService.Contracts.Responses.AcademicContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.UseCases.AcademicContext;

public sealed class GetAcademicContextUseCase
{
    private readonly IAcademicPeriodQueries _academicPeriodQueries;
    private readonly IEducationalProgramQueries _educationalProgramQueries;
    private readonly IStudyPlanQueries _studyPlanQueries;
    private readonly TimeProvider _timeProvider;

    public GetAcademicContextUseCase(
        IAcademicPeriodQueries academicPeriodQueries,
        IEducationalProgramQueries educationalProgramQueries,
        IStudyPlanQueries studyPlanQueries,
        TimeProvider timeProvider)
    {
        _academicPeriodQueries = academicPeriodQueries;
        _educationalProgramQueries = educationalProgramQueries;
        _studyPlanQueries = studyPlanQueries;
        _timeProvider = timeProvider;
    }

    public async Task<GetAcademicContextResponse> ExecuteAsync(
        GetAcademicContextRequest request,
        CancellationToken cancellationToken)
    {
        // Consultamos el periodo escolar activo
        var activePeriod = await _academicPeriodQueries.GetActivePeriodAsync(request.TenantId, cancellationToken);
        if (activePeriod is null)
        {
            throw new AcademicPeriodNotFoundException(Guid.Empty);
        }

        // Determinamos si está dentro de la planeación
        var currentDate = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        bool isWithinWindow = currentDate >= activePeriod.AcademicLoadProcessStartDate &&
                              currentDate <= activePeriod.AcademicLoadProcessEndDate;

        // Consultamos el Programa Educativo
        var program = await _educationalProgramQueries.GetByIdAsync(
            request.TenantId,
            request.EducationalProgramId,
            cancellationToken);

        if (program is null)
        {
            throw new EducationalProgramNotFoundException(request.EducationalProgramId);
        }

        // Consultamos el Plan de Estudios activo del programa
        var studyPlan = await _studyPlanQueries.GetActiveByProgramIdAsync(
            request.TenantId,
            request.EducationalProgramId,
            cancellationToken);

        if (studyPlan is null)
        {
            throw new StudyPlanNotFoundException(Guid.Empty);
        }

        // Consultamos las materias de ese plan de estudios
        var subjects = await _studyPlanQueries.GetSubjectsByStudyPlanAsync(
            request.TenantId,
            studyPlan.Id,
            cancellationToken);

        return new GetAcademicContextResponse
        {
            AcademicPeriod = new AcademicPeriodContextDto
            {
                Id = activePeriod.Id,
                Code = activePeriod.Code,
                Name = activePeriod.Name,
                Status = activePeriod.Status,
                AcademicLoadProcessStartDate = activePeriod.AcademicLoadProcessStartDate,
                AcademicLoadProcessEndDate = activePeriod.AcademicLoadProcessEndDate
            },
            EducationalProgram = new EducationalProgramContextDto
            {
                Id = program.Id,
                Code = program.Code,
                Name = program.Name,
                Level = program.Level
            },
            StudyPlan = new StudyPlanContextDto
            {
                Id = studyPlan.Id,
                EducationalProgramId = studyPlan.EducationalProgramId,
                Code = studyPlan.Code,
                Name = studyPlan.Name,
                Version = studyPlan.Version,
                EffectiveFrom = studyPlan.EffectiveFrom,
                Status = studyPlan.Status
            },
            Subjects = subjects.Select(s => new SubjectContextDto
            {
                Id = s.SubjectId ?? Guid.Empty,
                Code = s.Code ?? string.Empty,
                Name = s.Name ?? string.Empty,
                Semester = s.Semester ?? 0,
                Credits = s.Credits ?? 0,
                IsRequired = s.IsRequired ?? false
            }).ToList(),
            IsWithinPlanningWindow = isWithinWindow
        };
    }
}