using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public interface ISubjectQueries
    {
        public Subject GetById(Guid id);

        public List<Subject> GetAll();
    }
}
