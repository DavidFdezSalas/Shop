
using MassTransit;

namespace Shop.Shared
{
    [ExcludeFromTopology]
    public interface IRabbitEvent
    {
        public DateTime CreatedAt { get; }
        public Guid EventId { get; }
    }
}