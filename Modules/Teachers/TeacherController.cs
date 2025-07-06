using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Teachers.Services;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Teachers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Get all teachers with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<TeacherDto>>>> GetAllTeachers(
            [FromQuery] PaginationRequest request)
        {
            var response = await _teacherService.GetAllAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get teacher by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> GetTeacherById(int id)
        {
            var response = await _teacherService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new teacher
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> CreateTeacher(
            [FromBody] CreateTeacherDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<TeacherDto>.ErrorResponse(
                    "Validation failed", 400, errors));
            }

            var response = await _teacherService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing teacher
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> UpdateTeacher(
            int id, [FromBody] UpdateTeacherDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<TeacherDto>.ErrorResponse(
                    "Validation failed", 400, errors));
            }

            var response = await _teacherService.UpdateAsync(id, updateDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete a teacher
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTeacher(int id)
        {
            var response = await _teacherService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

    }
}