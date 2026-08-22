using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Tests.Domain.Entities
{
    public class AcademicPeriodTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateAcademicPeriod()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 1);
            var endDate = new DateOnly(2026, 12, 15);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            // Act
            var academicPeriod = new AcademicPeriod(
                    tenantId,
                    "2026-2",
                    "Periodo Agosto-Diciembre 2026",
                    startDate,
                    endDate,
                    academicLoadProcessStartDate,
                    academicLoadProcessEndDate,
                    enrollmentProcessStartDate,
                    enrollmentProcessEndDate,
                    planningSubmissionDate,
                    firstPartialGradeReportDate,
                    secondPartialGradeReportDate,
                    thirdPartialGradeReportDate,
                    finalMinutesSubmissionDate
                );

            // Assert
            Assert.NotEqual(Guid.Empty, academicPeriod.Id);
            Assert.Equal(tenantId, academicPeriod.TenantId);
            Assert.Equal("2026-2", academicPeriod.Code);
            Assert.Equal("Periodo Agosto-Diciembre 2026", academicPeriod.Name);
            Assert.Equal(startDate, academicPeriod.StartDate);
            Assert.Equal(endDate, academicPeriod.EndDate);
            Assert.True(academicPeriod.Status);
            Assert.NotEqual(default, academicPeriod.CreatedAtUtc);
            Assert.Null(academicPeriod.UpdatedAtUtc);

        }

        [Fact]
        public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.Empty;
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 1);
            var endDate = new DateOnly(2026, 12, 15);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod(
                    tenantId,
                    "2026-2",
                    "Periodo Agosto-Diciembre 2026",
                    startDate,
                    endDate,
                    academicLoadProcessStartDate,
                    academicLoadProcessEndDate,
                    enrollmentProcessStartDate,
                    enrollmentProcessEndDate,
                    planningSubmissionDate,
                    firstPartialGradeReportDate,
                    secondPartialGradeReportDate,
                    thirdPartialGradeReportDate,
                    finalMinutesSubmissionDate
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void Constructor_WithInvalidCode_ShouldThrowArgumentException( string code)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 1);
            var endDate = new DateOnly(2026, 12, 15);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod(
                    tenantId,
                    code,
                    "Periodo Agosto-Diciembre 2026",
                    startDate,
                    endDate,
                    academicLoadProcessStartDate,
                    academicLoadProcessEndDate,
                    enrollmentProcessStartDate,
                    enrollmentProcessEndDate,
                    planningSubmissionDate,
                    firstPartialGradeReportDate,
                    secondPartialGradeReportDate,
                    thirdPartialGradeReportDate,
                    finalMinutesSubmissionDate
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void Constructor_WithInvalidName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 1);
            var endDate = new DateOnly(2026, 12, 15);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod(
                    tenantId,
                    "2026-2",
                    name,
                    startDate,
                    endDate,
                    academicLoadProcessStartDate,
                    academicLoadProcessEndDate,
                    enrollmentProcessStartDate,
                    enrollmentProcessEndDate,
                    planningSubmissionDate,
                    firstPartialGradeReportDate,
                    secondPartialGradeReportDate,
                    thirdPartialGradeReportDate,
                    finalMinutesSubmissionDate
                )
            );
        }

        [Fact]
        public void Constructor_WithEndDateBeforeStartDate_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 1);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod(
                    tenantId,
                    "2026-2",
                    "Periodo Agosto-Diciembre 2026",
                    startDate,
                    endDate,
                    academicLoadProcessStartDate,
                    academicLoadProcessEndDate,
                    enrollmentProcessStartDate,
                    enrollmentProcessEndDate,
                    planningSubmissionDate,
                    firstPartialGradeReportDate,
                    secondPartialGradeReportDate,
                    thirdPartialGradeReportDate,
                    finalMinutesSubmissionDate
                )
            );
        }
    }
}
