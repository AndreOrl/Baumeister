namespace Baumeister.Abstractions.Building
{
    public class BuilderAttribute : Attribute
    {
        public Type TypeToBuild { get; }

        public BuilderAttribute(Type typeToBuild)
        {
            TypeToBuild = typeToBuild;
        }
    }
}
