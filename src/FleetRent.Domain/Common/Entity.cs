using System;
using System.Collections.Generic;
using System.Text;

namespace FleetRent.Domain.Common
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; protected set; }
        protected Entity( Guid id)
        {
            if (Id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de l'entité ne peut pas être vide.", nameof(Id));
            }
            Id = Id;
        }

        protected Entity()
        {
        }

        public bool Equals(Entity? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => Equals(obj as Entity);
        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
        public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);
        public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);

    }
}
