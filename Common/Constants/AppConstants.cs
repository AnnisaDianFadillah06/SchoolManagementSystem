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