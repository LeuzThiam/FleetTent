using FleetRent.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetRent.Domain.Tests.Common
{
    public class ValueObjectTests
    {
        private sealed class TestMoney : ValueObject
        {
            public decimal Amount { get; }
            public string Currency { get; }

            public TestMoney(decimal amount, string currency)
            {
                Amount = amount;
                Currency = currency;
            }
            protected override IEnumerable<object?> GetEqualityComponents()
            {
                yield return Amount;
                yield return Currency;
            }
        }
        [Fact]
        public void Equals_AvecMemesComposants_RetourneVrai()
        {
            var money1 = new TestMoney(100, "USD");
            var money2 = new TestMoney(100, "USD");
            Assert.Equal(money1, money2);
            Assert.True(money1 == money2);
        }
        [Fact]
        public void Equals_AvecComposantsDifferents_RetourneFaux()
        {
            var money1 = new TestMoney(100, "USD");
            var money2 = new TestMoney(200, "USD");
            Assert.NotEqual(money1, money2);
        }
        [Fact]
        public void Equals_AvecTypeDifferents_RetourneFaux()
        {
            var money = new TestMoney(100, "USD");
            Assert.False(money.Equals(new object()));
        }
    }
}
