namespace FleetRent.Domain.Tests.Common;

using FleetRent.Domain.Common;

public class AggregateRootTests
{
    private sealed record TestDomainEvent : DomainEvent;

    private sealed class TestAggregate : AggregateRoot
    {
        public TestAggregate(Guid id) : base(id)
        {
        }

        public void FaireQuelqueChose() => Raise(new TestDomainEvent());
    }

    [Fact]
    public void Raise_AjouteLEvenementADomainEvents()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.FaireQuelqueChose();

        Assert.Single(aggregate.DomainEvents);
        Assert.IsType<TestDomainEvent>(aggregate.DomainEvents.First());
    }

    [Fact]
    public void ClearDomainEvents_ViveLesEvenements()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.FaireQuelqueChose();

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }
}