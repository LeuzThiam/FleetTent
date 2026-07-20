using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace FleetRent.Domain.Common
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public bool Equals(ValueObject? other)
        {
            if (other is null || GetType() != other.GetType())
            {
                return false;
            }
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override bool Equals(object? obj) => Equals(obj as ValueObject);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var component in GetEqualityComponents())
            {
                hash.Add(component);
            }
            return hash.ToHashCode();
        }

        public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);
        public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);


    }
}
