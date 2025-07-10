using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Enrollments.Services;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Attributes;
using SchoolManagementSystem.Common.Helpers;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Modules.Classes.Repositories;

namespace SchoolManagementSystem.Modules.Enrollments
{
    [ApiController]
    [Route("api/enrollment")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IClassRepository _classRepository;

        public EnrollmentController(
            IEnrollmentService enrollmentService,
            IClassRepository classRepository)
        {
            _enrollmentService = enrollmentService;
            _classRepository = classRepository;
        }

        /// <summary>
        /// Get all enrollments with pagination - Admin only
        /// </summary>
        [HttpGet]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<EnrollmentDto>>>> GetAllEnrollments(
            [FromQuery] PaginationRequest request)
        {
            var response = await _enrollmentService.GetAllAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get enrollment by ID - Admin, Teacher (their class), Student (their enrollment)
        /// </summary>
        [HttpGet("{id}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> GetEnrollmentById(int id)
        {
            var response = await _enrollmentService.GetByIdAsync(id);

            if (!response.Success || response.Data == null)
                return StatusCode(response.StatusCode, response);

            var enrollment = response.Data;
            var userRole = UserContextHelper.GetUserRole(HttpContext);

            if (userRole == UserRoles.Student)
            {
                var studentId = UserContextHelper.GetStudentId(HttpContext);
                if (studentId != enrollment.StudentId)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<EnrollmentDto>.ErrorResponse(
                            AppConstants.Messages.EnrollmentAccessDenied, 
                            AppConstants.StatusCodes.Forbidden));
                }
            }
            else if (userRole == UserRoles.Teacher)
            {
                var teacherId = UserContextHelper.GetTeacherId(HttpContext);
                if (teacherId == null)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<EnrollmentDto>.ErrorResponse(
                            AppConstants.Messages.TeacherAccessDenied, 
                            AppConstants.StatusCodes.Forbidden));
                }

                var isOwner = await _enrollmentService.IsClassOwnedByTeacherAsync(enrollment.ClassId, teacherId.Value);
                if (!isOwner)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<EnrollmentDto>.ErrorResponse(
                            AppConstants.Messages.TeacherNotAssigned, 
                            AppConstants.StatusCodes.Forbidden));
                }
            }

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get all enrollments for a specific student - Admin, Student (own data)
        /// </summary>
        [HttpGet("student/{studentId}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Student)]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<EnrollmentDto>>>> GetEnrollmentsByStudent(
            int studentId, [FromQuery] PaginationRequest request)
        {
            var userRole = UserContextHelper.GetUserRole(HttpContext);
            var currentStudentId = UserContextHelper.GetStudentId(HttpContext);

            // Student can only access their own enrollments
            if (userRole == UserRoles.Student && currentStudentId != studentId)
            {
                return StatusCode(AppConstants.StatusCodes.Forbidden, 
                    ApiResponse<PaginatedResponse<EnrollmentDto>>.ErrorResponse(
                        AppConstants.Messages.StudentAccessDenied, 
                        AppConstants.StatusCodes.Forbidden));
            }

            var response = await _enrollmentService.GetByStudentIdAsync(studentId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get all enrollments for a specific class - Admin, Teacher (own class)
        /// </summary>
        [HttpGet("class/{classId}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher)]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<EnrollmentDto>>>> GetEnrollmentsByClass(
            int classId, [FromQuery] PaginationRequest request)
        {
            var userRole = UserContextHelper.GetUserRole(HttpContext);

            // Teacher can only access enrollments for their own class
            if (userRole == UserRoles.Teacher)
            {
                var teacherId = UserContextHelper.GetTeacherId(HttpContext);
                if (teacherId == null)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<PaginatedResponse<EnrollmentDto>>.ErrorResponse(
                            AppConstants.Messages.TeacherAccessDenied, 
                            AppConstants.StatusCodes.Forbidden));
                }

                var isOwner = await _enrollmentService.IsClassOwnedByTeacherAsync(classId, teacherId.Value);
                if (!isOwner)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<PaginatedResponse<EnrollmentDto>>.ErrorResponse(
                            AppConstants.Messages.TeacherNotAssigned, 
                            AppConstants.StatusCodes.Forbidden));
                }
            }
            
            var response = await _enrollmentService.GetByClassIdAsync(classId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new enrollment - Admin only and Teacher their own class
        /// </summary>
        [HttpPost]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher)]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> CreateEnrollment(
            [FromBody] CreateEnrollmentDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<EnrollmentDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var userRole = UserContextHelper.GetUserRole(HttpContext);

            if (userRole == UserRoles.Teacher)
            {
                var teacherId = UserContextHelper.GetTeacherId(HttpContext);
                if (teacherId == null)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<EnrollmentDto>.ErrorResponse(
                            AppConstants.Messages.TeacherAccessDenied, 
                            AppConstants.StatusCodes.Forbidden));
                }

                var isTeaching = await _classRepository.IsClassOwnedByTeacherAsync(createDto.ClassId, teacherId.Value);
                if (!isTeaching)
                {
                    return StatusCode(AppConstants.StatusCodes.Forbidden, 
                        ApiResponse<EnrollmentDto>.ErrorResponse(
                            AppConstants.Messages.TeacherNotAssigned, 
                            AppConstants.StatusCodes.Forbidden));
                }
            }

            var response = await _enrollmentService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing enrollment - Admin only
        /// </summary>
        [HttpPut("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> UpdateEnrollment(
            int id, [FromBody] UpdateEnrollmentDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<EnrollmentDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var response = await _enrollmentService.UpdateAsync(id, updateDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete an enrollment - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> DeleteEnrollment(int id)
        {
            var response = await _enrollmentService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}