using System;
using System.Collections.Generic;
using System.Text;

namespace FleetRent.Domain.Common
{
    public interface IDomainEvent
    {
        DateTimeOffset OccurredOn { get; }
    }

    public abstract record DomainEvent : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
