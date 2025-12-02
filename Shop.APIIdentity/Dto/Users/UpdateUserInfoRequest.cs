namespace Shop.APIIdentity.Dto.Users
{
    public class UpdateUserInfoRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}
