using System.Reflection;

namespace Baumeister.Abstractions.Building
{
    public abstract class BuilderBase<TEntity>
        where TEntity : class
    {
        private readonly List<object> valueStore = [];

        public BuilderBase<TEntity> With<T>(T value)
        {
            if(value != null)
            {
                valueStore.Add(value);
            }

            return this;
        }

        public TEntity Build()
        {
            var constructor = FindMatchingConstructor();

            return InvokeConstructor(constructor, valueStore);
        }

        private ConstructorInfo FindMatchingConstructor()
        {
            ConstructorInfo[] constructors = typeof(TEntity).GetConstructors();
            Type[] valueTypes = [.. valueStore.Select(v => v.GetType())];


            ConstructorInfo? foundConstructor = null;
            var constructorFound = TryFindExactMatchingConstructor(constructors, valueTypes, ref foundConstructor);
            constructorFound = constructorFound || TryFindImplicitMatchingConstructor(constructors, valueTypes, ref foundConstructor);

            return constructorFound ? foundConstructor! : throw new InvalidOperationException("No constructor found");
        }      

        private bool TryFindExactMatchingConstructor(ConstructorInfo[] constructors, Type[] valueTypes, ref ConstructorInfo? foundConstructor)
        {
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length == valueTypes.Length &&
                    parameters.All(p => valueTypes.Contains(p.ParameterType)))
                {
                    foundConstructor = constructor;
                    return true;
                }
            }

            return false;
        }

        private bool TryFindImplicitMatchingConstructor(ConstructorInfo[] constructors, Type[] valueTypes, ref ConstructorInfo? foundConstructor)
        {
            ConstructorInfo? bestMatch = null;
            int maxMatches = -1;

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                int matchCount = parameters.Count(p => valueTypes.Contains(p.ParameterType));

                if (matchCount > maxMatches)
                {
                    maxMatches = matchCount;
                    bestMatch = constructor;
                }
            }

            if (bestMatch != null)
            {
                foundConstructor = bestMatch;
                return true;
            }

            return false;
        }

        private TEntity InvokeConstructor(ConstructorInfo constructor, List<object> values)
        {
            var parameters = constructor.GetParameters();
            var orderedValues = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var matchingValue = values.FirstOrDefault(v => v.GetType() == parameterType);

                if (matchingValue != null)
                {
                    orderedValues[i] = matchingValue;
                }
                else
                {
                    orderedValues[i] = GetDefaultValue(parameterType);
                }
            }

            return (TEntity)constructor.Invoke(orderedValues);
        }

        private object? GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
