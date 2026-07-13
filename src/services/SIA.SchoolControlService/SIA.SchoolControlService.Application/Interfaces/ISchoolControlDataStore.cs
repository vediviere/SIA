using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Application.Interfaces;

public interface ISchoolControlDataStore
{
  Task<SubjectReference?> GetSubjectReferenceAsync(Guid subjectId, CancellationToken cancellationToken);
}
