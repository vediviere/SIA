using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Tests.Domain.Entities
{
    public class EducationalProgramTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateEducationalProgram()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act
            var program = new EducationalProgram(
                tenantId,
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Assert
            Assert.NotEqual(Guid.Empty, program.Id);
            Assert.Equal(tenantId, program.TenantId);
            Assert.Equal("ISC", program.Code);
            Assert.Equal("Ingeniería en Sistemas Computacionales",program.Name);
            Assert.Equal("Licenciatura", program.Level);
            Assert.True(program.Status);
            Assert.NotEqual(default, program.CreatedAtUtc);
            Assert.Null(program.UpdatedAtUtc);
        }

        [Fact]
        public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
        {
            // Arrange
            var tenantId = Guid.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new EducationalProgram(
                    tenantId,
                    "ISC",
                    "Ingeniería en Sistemas Computacionales",
                    "Licenciatura"
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCode_ShouldThrowArgumentException(string code)
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new EducationalProgram(
                    tenantId,
                    code,
                    "Ingeniería en Sistemas Computacionales",
                    "Licenciatura"
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new EducationalProgram(
                    tenantId,
                    "ISC",
                    name,
                    "Licenciatura"
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidLevel_ShouldThrowArgumentException(string level)
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new EducationalProgram(
                    tenantId,
                    "ISC",
                    "Ingeniería en Sistemas Computacionales",
                    level
                )
            );
        }

        [Fact]
        public void Constructor_WithCodeContainingSpaces_ShouldNormalizeCode()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var code = "  isc  ";

            // Act
            var program = new EducationalProgram(
                tenantId,
                code,
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Assert
            Assert.Equal("ISC", program.Code);
        }

        [Fact]
        public void Constructor_WithNameContainingSpaces_ShouldNormalizeName()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "  Ingeniería en Sistemas Computacionales  ";

            // Act
            var program = new EducationalProgram(
                tenantId,
                "ISC",
                name,
                "Licenciatura"
            );

            // Assert
            Assert.Equal("Ingeniería en Sistemas Computacionales", program.Name);
        }

        [Fact]
        public void Constructor_WithLevelContainingSpaces_ShouldNormalizeLevel()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var level = "  Licenciatura  ";

            // Act
            var program = new EducationalProgram(
                tenantId,
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                level
            );

            // Assert
            Assert.Equal("Licenciatura", program.Level);
        }

        [Fact]
        public void Desactivate_ShouldSetStatusToFalse()
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Act
            program.Desactivate();

            // Assert
            Assert.False(program.Status);
        }

        [Fact]
        public void Desactivate_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Act
            program.Desactivate();

            // Assert
            Assert.NotNull(program.UpdatedAtUtc);
        }

        [Fact]
        public void Activate_ShouldSetStatusToTrue()
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            program.Desactivate();

            // Act
            program.Activate();

            // Assert
            Assert.True(program.Status);
        }

        [Fact]
        public void Activate_ShouldSetUpdatedAtUtc()
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            program.Desactivate();

            // Act
            program.Activate();

            // Assert
            Assert.NotNull(program.UpdatedAtUtc);
        }

        [Fact]
        public void UpdateDetails_WithValidData_ShouldUpdateEducationalProgram()
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Act
            program.UpdateDetails(
                "  ISE  ",
                "  Ingeniería en Software  ",
                "  Ingeniería  "
            );

            // Assert
            Assert.Equal("ISE", program.Code);
            Assert.Equal("Ingeniería en Software", program.Name);
            Assert.Equal("Ingeniería", program.Level);
            Assert.NotNull(program.UpdatedAtUtc);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateDetails_WithInvalidCode_ShouldThrowArgumentException(string code)
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                program.UpdateDetails(
                    code,
                    "Ingeniería en Software",
                    "Licenciatura"
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateDetails_WithInvalidName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                program.UpdateDetails(
                    "ISE",
                    name,
                    "Licenciatura"
                )
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateDetails_WithInvalidLevel_ShouldThrowArgumentException(string level)
        {
            // Arrange
            var program = new EducationalProgram(
                Guid.NewGuid(),
                "ISC",
                "Ingeniería en Sistemas Computacionales",
                "Licenciatura"
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                program.UpdateDetails(
                    "ISE",
                    "Ingeniería en Software",
                    level
                )
            );
        }
    }
}
