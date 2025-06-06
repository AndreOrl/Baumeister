using System.Reflection;

namespace Baumeister.Abstractions.Building
{
    public abstract class BuilderBase<TEntity>
        where TEntity : class
    {
        private readonly List<ValueMapping> valueStore = new();

        public BuilderBase<TEntity> With<T>(string name, T value)
        {
            if(value != null)
            {
                var existingValueEntry = valueStore.FirstOrDefault(p => p.Name == name);
                if (existingValueEntry != null)
                {
                    valueStore.Remove(existingValueEntry);
                }

                valueStore.Add(new ValueMapping(name, value));
            }

            return this;
        }

        public TEntity Build()
        {
            var constructor = FindMatchingConstructor();
                        
            var createdObject = InvokeConstructor(constructor, valueStore, out var remainingValues);
            SetRemainingProperties(constructor, createdObject, remainingValues);

            return createdObject;
        }
        
        private ConstructorInfo FindMatchingConstructor()
        {
            ConstructorInfo[] constructors = typeof(TEntity).GetConstructors();
            Type[] valueTypes = valueStore.Select(v => v.Value.GetType()).ToArray();


            ConstructorInfo? foundConstructor = null;
            var isCtorFound = TryFindExactMatchingConstructor(constructors, valueTypes, ref foundConstructor);
            isCtorFound = isCtorFound || TryFindImplicitMatchingConstructor(constructors, valueTypes, ref foundConstructor);

            return isCtorFound ? foundConstructor! : throw new InvalidOperationException("No constructor found");
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

        private TEntity InvokeConstructor(ConstructorInfo constructor, List<ValueMapping> values, out List<ValueMapping> remainingValues)
        {
            remainingValues = new List<ValueMapping>();
            remainingValues.AddRange(values);

            var parameters = constructor.GetParameters();
            var orderedValues = new object?[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var valuesMatchedByType = values.Where(v => v.Value.GetType() == parameterType).ToList();

                if (valuesMatchedByType.Count == 1)
                {
                    orderedValues[i] = valuesMatchedByType[0].Value;
                    remainingValues.Remove(valuesMatchedByType[0]);
                }
                else if(valuesMatchedByType.Count > 1)
                {
                    var valuesMatchedByTypeAndName = valuesMatchedByType.Where(v => parameters[i].Name == v.Name).ToList();
                    if (valuesMatchedByTypeAndName.Count == 1)
                    {
                        orderedValues[i] = valuesMatchedByTypeAndName[0].Value;
                        remainingValues.Remove(valuesMatchedByTypeAndName[0]);
                    }
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

        private void SetRemainingProperties(ConstructorInfo usedConstructor, TEntity createdObject, List<ValueMapping> values)
        {
            var initializedConstructorParameters = usedConstructor.GetParameters().Select(p => p.Name.ToLowerInvariant());

            var createdObjectsType = createdObject.GetType();
            createdObjectsType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => 
                    p.CanWrite && 
                    initializedConstructorParameters.Contains(p.Name.ToLowerInvariant()) == false &&
                    values.Any(v => v.Value.GetType() == p.PropertyType))
                .ToList()
                .ForEach(p =>
                {
                    var valuesMatchedByType = values.Where(v => v.Value.GetType() == p.PropertyType).ToList();
                    if(valuesMatchedByType.Count == 1)
                    {
                        p.SetValue(createdObject, valuesMatchedByType[0].Value);
                        values.Remove(valuesMatchedByType[0]);
                    }
                    if(valuesMatchedByType.Count > 1)
                    {
                        var valuesMatchedByTypeAndName = valuesMatchedByType.Where(v => v.Name == p.Name).ToList();
                        if(valuesMatchedByTypeAndName.Count == 1)
                        {
                            p.SetValue(createdObject, valuesMatchedByTypeAndName[0].Value);
                            values.Remove(valuesMatchedByTypeAndName[0]);
                        }
                    }
                });
        }
    }
}
