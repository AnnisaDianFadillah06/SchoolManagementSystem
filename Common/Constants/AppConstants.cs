namespace SchoolManagementSystem.Common.Constants
{
    public static class AppConstants
    {
        public static class Database
        {
            public const string CurrentTimestamp = "CURRENT_TIMESTAMP";
        }
        public static class Messages
        {
            public const string UserCreated = "User created successfully";
            public const string Unauthorized = "Invalid credentials";
            public const string UserAlreadyExistsForTeacher = "User Already Exists For Teacher";
            public const string UserAlreadyExistsForStudent = "User Already Exists For Teacher";

            public const string StudentNotFound = "Student not found";
            public const string StudentCreated = "Student created successfully";
            public const string StudentUpdated = "Student updated successfully";
            public const string StudentDeleted = "Student deleted successfully";
            public const string ValidationError = "Validation error occurred";
            public const string InternalError = "Internal server error occurred";

            public const string TeacherNotFound = "Teacher not found";
            public const string TeacherCreated = "Teacher created successfully";
            public const string TeacherUpdated = "Teacher updated successfully";
            public const string TeacherDeleted = "Teacher deleted successfully";

            public const string ClassNotFound = "Class not found";
            public const string ClassCreated = "Class created successfully";
            public const string ClassUpdated = "Class updated successfully";
            public const string ClassDeleted = "Class deleted successfully";

            public const string EnrollmentNotFound = "Enrollment not found";
            public const string EnrollmentCreated = "Enrollment created successfully";
            public const string EnrollmentUpdated = "Enrollment updated successfully";
            public const string EnrollmentDeleted = "Enrollment deleted successfully";
            public const string EnrollmentRetrieved = "Enrollments retrieved successfully";
            public const string AccessDenied = "Access denied. You don't have permission to access this resource";
            public const string StudentAccessDenied = "Access denied. Students can only access their own data";
            public const string TeacherAccessDenied = "Access denied. Teachers can only access their own data or classes";
            public const string ClassAccessDenied = "Access denied. You don't have permission to access this class";
            public const string EnrollmentAccessDenied = "Access denied. You don't have permission to access this enrollment";
            public const string TeacherNotAssigned = "Access denied. Teacher is not assigned to this class";
        }

        public static class StatusCodes
        {
            public const int Success = 200;
            public const int Created = 201;
            public const int BadRequest = 400;
            public const int NotFound = 404;
            public const int InternalError = 500;
            public const int Unauthorized = 401;
            public const int Forbidden = 403;
        }
    }
}