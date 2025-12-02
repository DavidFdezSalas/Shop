namespace Shop.APIIdentity.Dto.Users
{
    public class GetUsersResponse
    {
        public required List<UserInfoResponse> Users { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
