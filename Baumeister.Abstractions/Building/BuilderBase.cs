namespace Baumeister.Abstractions.Building
{
    public abstract class BuilderBase<TBuilder, TEntity>
        where TBuilder : BuilderBase<TBuilder, TEntity>, new()
        where TEntity : class
    {
        public static TBuilder New()
        {
            return new TBuilder();
        }

        protected BuilderBase()
        {
        }

        public abstract TEntity Build();
    }
}
