using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Teachers.Services;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Attributes;
using SchoolManagementSystem.Common.Helpers;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Teachers
{
    [ApiController]
    [Route("api/teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Get all teachers with pagination - Admin only
        /// </summary>
        [HttpGet]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<TeacherDto>>>> GetAllTeachers(
            [FromQuery] PaginationRequest request)
        {
            var response = await _teacherService.GetAllAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get teacher by ID - Admin or own teacher data
        /// </summary>
        [HttpGet("{id}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher)]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> GetTeacherById(int id)
        {
            var userRole = UserContextHelper.GetUserRole(HttpContext);
            var teacherId = UserContextHelper.GetTeacherId(HttpContext);

            // Teacher can only access their own data
            if (userRole == UserRoles.Teacher && teacherId != id)
            {
                return StatusCode(AppConstants.StatusCodes.Forbidden, 
                    ApiResponse<TeacherDto>.ErrorResponse(
                        AppConstants.Messages.TeacherAccessDenied, 
                        AppConstants.StatusCodes.Forbidden));
            }

            var response = await _teacherService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new teacher - Admin only
        /// </summary>
        [HttpPost]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> CreateTeacher(
            [FromBody] CreateTeacherDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<TeacherDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var response = await _teacherService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing teacher - Admin only
        /// </summary>
        [HttpPut("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> UpdateTeacher(
            int id, [FromBody] UpdateTeacherDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<TeacherDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var response = await _teacherService.UpdateAsync(id, updateDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete a teacher - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> DeleteTeacher(int id)
        {
            var response = await _teacherService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}