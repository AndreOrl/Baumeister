#nullable enable
using System;

namespace Baumeister.Examples.DirtyAggregate
{
    public partial class DirtyBuilder
    {
        public static DirtyBuilder New()
        {
            return new DirtyBuilder();
        }

        public DirtyBuilder WithEqualityContract(System.Type equalitycontract)
        {
            this.With("EqualityContract", equalitycontract);
            return this;
        }

        public DirtyBuilder WithCtor1(string ctor1)
        {
            this.With("Ctor1", ctor1);
            return this;
        }

        public DirtyBuilder WithCtor2(string ctor2)
        {
            this.With("Ctor2", ctor2);
            return this;
        }

        public DirtyBuilder WithProperty1(string? property1)
        {
            this.With("Property1", property1);
            return this;
        }

        public DirtyBuilder WithProperty2(string? property2)
        {
            this.With("Property2", property2);
            return this;
        }
    }
}
#nullable restore
