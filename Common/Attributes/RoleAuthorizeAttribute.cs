using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Common.Attributes
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public RoleAuthorizeAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }

    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";
    }
}