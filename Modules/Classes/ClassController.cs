using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Classes.Services;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Attributes;
using SchoolManagementSystem.Common.Helpers;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Classes
{
    [ApiController]
    [Route("api/class")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        /// <summary>
        /// Get all classes with pagination - Admin and Teacher can access
        /// </summary>
        [HttpGet]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher)]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClassDto>>>> GetAllClasses(
            [FromQuery] PaginationRequest request)
        {
            var response = await _classService.GetAllAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get class by ID - Admin, Teacher (own class), Student (enrolled class)
        /// </summary>
        [HttpGet("{id}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        public async Task<ActionResult<ApiResponse<ClassDto>>> GetClassById(int id)
        {
            var userRole = UserContextHelper.GetUserRole(HttpContext);
            var teacherId = UserContextHelper.GetTeacherId(HttpContext);
            var studentId = UserContextHelper.GetStudentId(HttpContext);

            var response = await _classService.GetByIdAsync(id, teacherId, studentId, userRole);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new class - Admin only
        /// </summary>
        [HttpPost]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<ClassDto>>> CreateClass(
            [FromBody] CreateClassDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ClassDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var response = await _classService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing class - Admin and Teacher (own class)
        /// </summary>
        [HttpPut("{id}")]
        [RoleAuthorize(UserRoles.Admin, UserRoles.Teacher)]
        public async Task<ActionResult<ApiResponse<ClassDto>>> UpdateClass(
            int id, [FromBody] UpdateClassDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ClassDto>.ErrorResponse(
                    AppConstants.Messages.ValidationError, 
                    AppConstants.StatusCodes.BadRequest, 
                    errors));
            }

            var userRole = UserContextHelper.GetUserRole(HttpContext);
            var teacherId = UserContextHelper.GetTeacherId(HttpContext);

            var response = await _classService.UpdateAsync(id, updateDto, teacherId, userRole);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete a class - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [RoleAuthorize(UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<ClassDto>>> DeleteClass(int id)
        {
            var response = await _classService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}