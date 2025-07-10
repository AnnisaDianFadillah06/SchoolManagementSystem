using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Students.Services;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Examples;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Attributes;
using SchoolManagementSystem.Common.Helpers;
using SchoolManagementSystem.Common.Constants;
using Swashbuckle.AspNetCore.Filters;

namespace SchoolManagementSystem.Modules.Students
{
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students with pagination - Admin only
        /// </summary>
        [HttpGet]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<StudentDto>>>> GetAllStudents(
            [FromQuery] PaginationRequest request)
        {
            var response = await _studentService.GetAllAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get student by ID - Admin or own student data
        /// </summary>
        [HttpGet("{id}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Student)]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetStudentById(int id)
        {
            var userRole = UserContextHelper.GetUserRole(HttpContext);
            var studentId = UserContextHelper.GetStudentId(HttpContext);

            // Student can only access their own data
            if (userRole == UserRoles.Student && studentId != id)
            {
                return StatusCode(AppConstants.StatusCodes.Forbidden, 
                    ApiResponse<StudentDto>.ErrorResponse(
                        AppConstants.Messages.StudentAccessDenied, 
                        AppConstants.StatusCodes.Forbidden));
            }

            var response = await _studentService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new student - Admin only
        /// </summary>
        [HttpPost]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<StudentDto>>> CreateStudent(
            [FromBody] CreateStudentDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<StudentDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var response = await _studentService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing student - Admin only
        /// </summary>
        [HttpPatch("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        [SwaggerRequestExample(typeof(PatchStudentDto), typeof(PatchStudentDtoExample))]
        public async Task<ActionResult<ApiResponse<StudentDto>>> PatchStudent(
            int id, [FromBody] PatchStudentDto patchDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<StudentDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError,
                    AppConstants.StatusCodes.BadRequest,
                    errors));
            }

            var response = await _studentService.PatchAsync(id, patchDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete a student - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<StudentDto>>> DeleteStudent(int id)
        {
            var response = await _studentService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}