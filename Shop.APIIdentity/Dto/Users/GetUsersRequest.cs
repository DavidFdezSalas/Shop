namespace Shop.APIIdentity.Dto.Users
{
    public class GetUsersRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
