namespace Shop.APIIdentity.Dto.Auth
{
    public class ResponseLogin
    {
        public required string Token { get; set; }
        public DateTime ExpirationAt { get; set; }
    }
}
