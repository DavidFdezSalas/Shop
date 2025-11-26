namespace Shop.APIIdentity.Dto.Auth
{
    public class ResponseLogin
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpirationAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public sealed record RequestLogin
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
