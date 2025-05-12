namespace Baumeister.Examples.DirtyAggregate
{
    public record Dirty(string Ctor1, string Ctor2)
    {
        public string? Property1 { get; set; }

        public string? Property2 { get; set; }
    }
}
