namespace Baumeister.Examples.DirtyAggregate
{
    public record Dirty(string Ctor1, string Ctor2)
    {
        public static string StaticProperty { get; set; } = "static";

#pragma warning disable CS0414
        private string _notSettable = "initial";
#pragma warning restore CS0414

        public string? Property1 { get; set; }

        public string? Property2 { get; set; }
    }
}
