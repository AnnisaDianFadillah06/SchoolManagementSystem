namespace SchoolManagementSystem.Common.Constants
{
    public static class AppConstants
    {
        public static class Messages
        {
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
        }

        public static class StatusCodes
        {
            public const int Success = 200;
            public const int Created = 201;
            public const int BadRequest = 400;
            public const int NotFound = 404;
            public const int InternalError = 500;
        }
    }
}