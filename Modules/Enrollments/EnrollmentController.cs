using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Enrollments.Services;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Enrollments
{

    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Get all enrollments with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<EnrollmentDto>>>> GetAllEnrollments(
            [FromQuery] PaginationRequest request)
        {
            var response = await _enrollmentService.GetAllAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get enrollment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> GetEnrollmentById(int id)
        {
            var response = await _enrollmentService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new enrollment
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> CreateEnrollment(
            [FromBody] CreateEnrollmentDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<EnrollmentDto>.ErrorResponse(
                    "Validation failed", 400, errors));
            }

            var response = await _enrollmentService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing enrollment
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> UpdateEnrollment(
            int id, [FromBody] UpdateEnrollmentDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<EnrollmentDto>.ErrorResponse(
                    "Validation failed", 400, errors));
            }

            var response = await _enrollmentService.UpdateAsync(id, updateDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete an enrollment
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> DeleteEnrollment(int id)
        {
            var response = await _enrollmentService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

    }
}