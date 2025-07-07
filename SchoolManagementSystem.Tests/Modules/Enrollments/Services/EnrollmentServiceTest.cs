using AutoMapper;
using Moq;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Entities;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Enrollments.Services;
using Xunit;

namespace SchoolManagementSystem.Tests.Modules.Enrollments.Services
{
    public class EnrollmentServiceTests
    {
        private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentServiceTests()
        {
            _mockEnrollmentRepository = new Mock<IEnrollmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _enrollmentService = new EnrollmentService(_mockEnrollmentRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_Success_ReturnsCreatedEnrollment()
        {
            // Arrange
            var createDto = new CreateEnrollmentDto { StudentId = 1, ClassId = 1 };
            var enrollment = new Enrollment { Id = 1, StudentId = 1, ClassId = 1, Status = "Active" };
            var enrollmentDto = new EnrollmentDto { Id = 1, StudentId = 1, ClassId = 1, Status = "Active" };

            _mockEnrollmentRepository.Setup(r => r.StudentExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.ClassExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.DuplicateEnrollmentExistsAsync(1, 1)).ReturnsAsync(false);
            _mockEnrollmentRepository.Setup(r => r.CanEnrollAsync(1)).ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<Enrollment>(createDto)).Returns(enrollment);
            _mockEnrollmentRepository.Setup(r => r.CreateAsync(enrollment)).ReturnsAsync(enrollment);
            _mockMapper.Setup(m => m.Map<EnrollmentDto>(enrollment)).Returns(enrollmentDto);

            // Act
            var result = await _enrollmentService.CreateAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(AppConstants.Messages.EnrollmentCreated, result.Message);
            Assert.Equal(AppConstants.StatusCodes.Created, result.StatusCode);
            Assert.Equal(enrollmentDto, result.Data);
        }

        [Fact]
        public async Task CreateAsync_StudentNotFound_ReturnsError()
        {
            // Arrange
            var createDto = new CreateEnrollmentDto { StudentId = 1, ClassId = 1 };
            _mockEnrollmentRepository.Setup(r => r.StudentExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _enrollmentService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Student not found", result.Message);
            Assert.Equal(AppConstants.StatusCodes.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_ClassNotFound_ReturnsError()
        {
            // Arrange
            var createDto = new CreateEnrollmentDto { StudentId = 1, ClassId = 1 };
            _mockEnrollmentRepository.Setup(r => r.StudentExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.ClassExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _enrollmentService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Class not found", result.Message);
            Assert.Equal(AppConstants.StatusCodes.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_DuplicateEnrollment_ReturnsError()
        {
            // Arrange
            var createDto = new CreateEnrollmentDto { StudentId = 1, ClassId = 1 };
            _mockEnrollmentRepository.Setup(r => r.StudentExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.ClassExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.DuplicateEnrollmentExistsAsync(1, 1)).ReturnsAsync(true);

            // Act
            var result = await _enrollmentService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Student sudah terdaftar di kelas ini", result.Message);
            Assert.Equal(AppConstants.StatusCodes.BadRequest, result.StatusCode);
            Assert.Contains("Duplicate Enrollment", result.Errors);
        }

        [Fact]
        public async Task CreateAsync_ClassFull_ReturnsError()
        {
            // Arrange
            var createDto = new CreateEnrollmentDto { StudentId = 1, ClassId = 1 };
            _mockEnrollmentRepository.Setup(r => r.StudentExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.ClassExistsAsync(1)).ReturnsAsync(true);
            _mockEnrollmentRepository.Setup(r => r.DuplicateEnrollmentExistsAsync(1, 1)).ReturnsAsync(false);
            _mockEnrollmentRepository.Setup(r => r.CanEnrollAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _enrollmentService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Class has reached maximum capacity", result.Message);
            Assert.Equal(AppConstants.StatusCodes.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task UpdateAsync_Success_ReturnsUpdatedEnrollment()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateEnrollmentDto { Status = "Completed" };
            var existingEnrollment = new Enrollment { Id = 1, StudentId = 1, ClassId = 1, Status = "Active" };
            var updatedEnrollment = new Enrollment { Id = 1, StudentId = 1, ClassId = 1, Status = "Completed" };
            var enrollmentDto = new EnrollmentDto { Id = 1, StudentId = 1, ClassId = 1, Status = "Completed" };

            _mockEnrollmentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEnrollment);
            _mockMapper.Setup(m => m.Map(updateDto, existingEnrollment));
            _mockEnrollmentRepository.Setup(r => r.UpdateAsync(existingEnrollment)).ReturnsAsync(updatedEnrollment);
            _mockMapper.Setup(m => m.Map<EnrollmentDto>(updatedEnrollment)).Returns(enrollmentDto);

            // Act
            var result = await _enrollmentService.UpdateAsync(id, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(AppConstants.Messages.EnrollmentUpdated, result.Message);
            Assert.Equal(AppConstants.StatusCodes.Success, result.StatusCode);
            Assert.Equal(enrollmentDto, result.Data);
        }

        [Fact]
        public async Task UpdateAsync_EnrollmentNotFound_ReturnsError()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateEnrollmentDto { Status = "Completed" };
            _mockEnrollmentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Enrollment?)null);

            // Act
            var result = await _enrollmentService.UpdateAsync(id, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(AppConstants.Messages.EnrollmentNotFound, result.Message);
            Assert.Equal(AppConstants.StatusCodes.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateAsync_InvalidStatus_ReturnsError()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateEnrollmentDto { Status = "Invalid" };
            var existingEnrollment = new Enrollment { Id = 1, StudentId = 1, ClassId = 1, Status = "Active" };
            _mockEnrollmentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEnrollment);

            // Act
            var result = await _enrollmentService.UpdateAsync(id, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid status value", result.Message);
            Assert.Equal(AppConstants.StatusCodes.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task UpdateAsync_ClassFullWhenActivating_ReturnsError()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateEnrollmentDto { Status = "Active" };
            var existingEnrollment = new Enrollment { Id = 1, StudentId = 1, ClassId = 1, Status = "Inactive" };
            _mockEnrollmentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEnrollment);
            _mockEnrollmentRepository.Setup(r => r.CanEnrollAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _enrollmentService.UpdateAsync(id, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Class has reached maximum capacity", result.Message);
            Assert.Equal(AppConstants.StatusCodes.BadRequest, result.StatusCode);
        }
    }
}