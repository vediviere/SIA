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

        [Fact]
        public void Constructor_WithCodeContainingSpaces_ShouldNormalizeCode()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = "  periodo-2026-2  ";
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
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
                );

            // Assert
            Assert.Equal("PERIODO-2026-2", academicPeriod.Code);

        }

        [Fact]
        public void Constructor_WithNameContainingSpaces_ShouldNormalizeName()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = " Periodo Agosto-Diciembre 2026 ";
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
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
                    "periodo-2026-2",
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
                );

            // Assert 
            Assert.Equal("Periodo Agosto-Diciembre 2026", academicPeriod.Name);

        }

        [Fact]
        public void Deactivate_ShouldSetStatusToFalse()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            var academicPeriod = new AcademicPeriod(
                    tenantId,
                    "periodo-2026-2",
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

            // Act
            academicPeriod.Deactivate();

            //Assert
            Assert.False(academicPeriod.Status);
        }

        [Fact]
        public void Deactivate_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            var academicPeriod = new AcademicPeriod(
                    tenantId,
                    "periodo-2026-2",
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

            // Act
            academicPeriod.Deactivate();

            // Assert
            Assert.NotNull(academicPeriod.UpdatedAtUtc);
        }

        [Fact]
        public void Activate_ShouldSetStatusToTrue()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            var academicPeriod = new AcademicPeriod(
                    tenantId,
                    "periodo-2026-2",
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

            academicPeriod.Deactivate();

            // Act
            academicPeriod.Activate();

            //Assert
            Assert.True(academicPeriod.Status);
        }

        [Fact]
        public void Activate_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            var academicPeriod = new AcademicPeriod(
                    tenantId,
                    "periodo-2026-2",
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

            academicPeriod.Deactivate();

            // Act
            academicPeriod.Activate();

            //Assert
            Assert.NotNull(academicPeriod.UpdatedAtUtc);
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateAcademicPeriod()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 01);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);
            var academicLoadProcessStartDate = processDate;
            var academicLoadProcessEndDate = processDate;
            var enrollmentProcessStartDate = processDate;
            var enrollmentProcessEndDate = processDate;
            var planningSubmissionDate = processDate;
            var firstPartialGradeReportDate = processDate;
            var secondPartialGradeReportDate = processDate;
            var thirdPartialGradeReportDate = processDate;
            var finalMinutesSubmissionDate = processDate;

            var academicPeriod = new AcademicPeriod(
                    tenantId,
                    "periodo-2026-2",
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

            //Act
            academicPeriod.Update(
                "periodo-2026-1",
                "Periodo Enero-Julio 2026",
                new DateOnly(2026,1,15),
                new DateOnly(2026,1,16),
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


            //Assert
            Assert.Equal("PERIODO-2026-1",academicPeriod.Code);
            Assert.Equal("Periodo Enero-Julio 2026", academicPeriod.Name);
            Assert.Equal(new DateOnly(2026,1,15), academicPeriod.StartDate);
            Assert.Equal(new DateOnly(2026, 1, 16), academicPeriod.EndDate);
            Assert.NotNull(academicPeriod.UpdatedAtUtc);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidCode_ShouldThrowArgumentException(string code)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 1);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);

            var academicPeriod = new AcademicPeriod(
                tenantId,
                "PERIODO-2026-2",
                "Periodo Agosto-Diciembre 2026",
                startDate,
                endDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                academicPeriod.Update(
                    code,
                    "Periodo Enero-Julio 2026",
                    new DateOnly(2026, 1, 15),
                    new DateOnly(2026, 1, 16),
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 1);
            var startDate = new DateOnly(2026, 8, 15);
            var endDate = new DateOnly(2026, 8, 16);

            var academicPeriod = new AcademicPeriod(
                tenantId,
                "PERIODO-2026-2",
                "Periodo Agosto-Diciembre 2026",
                startDate,
                endDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                academicPeriod.Update(
                    "PERIODO-2026-1",
                    name,
                    new DateOnly(2026, 1, 15),
                    new DateOnly(2026, 1, 16),
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate
                )
            );
        }

        [Fact]
        public void Update_WithEndDateBeforeStartDate_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var processDate = new DateOnly(2026, 8, 1);

            var academicPeriod = new AcademicPeriod(
                tenantId,
                "PERIODO-2026-2",
                "Periodo Agosto-Diciembre 2026",
                new DateOnly(2026, 8, 15),
                new DateOnly(2026, 8, 16),
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate,
                processDate
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                academicPeriod.Update(
                    "PERIODO-2026-1",
                    "Periodo Enero-Julio 2026",
                    new DateOnly(2026, 8, 15),
                    new DateOnly(2026, 8, 1),
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate,
                    processDate
                )
            );
        }

    }
}
