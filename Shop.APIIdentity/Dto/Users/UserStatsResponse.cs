namespace Shop.APIIdentity.Dto.Users
{
    public class UserStatsResponse
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalCustomers { get; set; }
        public int LockedUsers { get; set; }
    }
}
