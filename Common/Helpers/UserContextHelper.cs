namespace SchoolManagementSystem.Common.Helpers
{
    public static class UserContextHelper
    {
        public static int GetUserId(HttpContext context)
        {
            return (int)(context.Items["UserId"] ?? 0);
        }

        public static string GetUserRole(HttpContext context)
        {
            return context.Items["Role"]?.ToString() ?? string.Empty;
        }

        public static int? GetStudentId(HttpContext context)
        {
            var studentId = context.Items["StudentId"]?.ToString();
            return string.IsNullOrEmpty(studentId) ? null : int.Parse(studentId);
        }

        public static int? GetTeacherId(HttpContext context)
        {
            var teacherId = context.Items["TeacherId"]?.ToString();
            return string.IsNullOrEmpty(teacherId) ? null : int.Parse(teacherId);
        }
    }
}