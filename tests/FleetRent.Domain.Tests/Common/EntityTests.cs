using FleetRent.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetRent.Domain.Tests.Common
{
    public class EntityTests
    {
        private sealed class TestEntity : Entity
        {
            public TestEntity(Guid id) : base(id)
            {
            }
        }

        [Fact]
        public void Equals_AvecMemeId_RetourneVrai()
        {
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            Assert.Equal(entity1, entity2);
            Assert.True(entity1 == entity2);
        }

        [Fact]
        public void Equals_AvecIdDifferents_RetourneFaux()
        {
            var entity1 = new TestEntity(Guid.NewGuid());
            var entity2 = new TestEntity(Guid.NewGuid());
            Assert.NotEqual(entity1, entity2);
        }
        [Fact]
        public void Constructeur_AvecGuidVide_LeveArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new TestEntity(Guid.Empty));
        }
        [Fact]
        public void GetHashCode_AvecMemeId_SontEgaux()
        {
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);
            Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        }
    }
}
