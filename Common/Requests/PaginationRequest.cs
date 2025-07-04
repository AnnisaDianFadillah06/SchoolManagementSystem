namespace SchoolManagementSystem.Common.Requests
{
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
    }
}
