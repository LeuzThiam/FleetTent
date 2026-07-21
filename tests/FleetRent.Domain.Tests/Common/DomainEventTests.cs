namespace FleetRent.Domain.Tests.Common;

using FleetRent.Domain.Common;

public class DomainEventTests
{
    private sealed record TestDomainEvent : DomainEvent;

    [Fact]
    public void OccurredOn_EstDefiniALaCreation()
    {
        var avant = DateTimeOffset.UtcNow;
        var domainEvent = new TestDomainEvent();
        var apres = DateTimeOffset.UtcNow;

        Assert.InRange(domainEvent.OccurredOn, avant, apres);
    }
}