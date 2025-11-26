namespace Shop.APIIdentity.Dto.Users
{
    public sealed record PasswordChageResponse
    {
        public bool Success { get; set; }
    }

    public sealed record PasswordChageRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }   
}
