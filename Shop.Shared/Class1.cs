namespace Shop.Shared
{

    public sealed record UserCreatedEvents(string userId, string email) : EventBase { }

    public abstract record EventBase()
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
