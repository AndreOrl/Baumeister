using System;
using System.Globalization;

namespace Baumeister.Generators.Building;

internal sealed class WithMethodInfo : IEquatable<WithMethodInfo>
{
    public string ParameterTypeName { get; }
    public string ParameterName { get; }

    public WithMethodInfo(string parameterTypeName, string parameterName)
    {
        ParameterTypeName = parameterTypeName;
        ParameterName = parameterName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is WithMethodInfo other)
        {
            return ParameterTypeName == other.ParameterTypeName && ParameterName == other.ParameterName;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return ParameterTypeName.ToLower(CultureInfo.InvariantCulture).GetHashCode() + ParameterName.ToLower(CultureInfo.InvariantCulture).GetHashCode();
    }

    public bool Equals(WithMethodInfo other)
    {
        if (other == null) return false;
        return ParameterTypeName.Equals(other.ParameterTypeName, StringComparison.OrdinalIgnoreCase) && ParameterName.Equals(other.ParameterName, StringComparison.OrdinalIgnoreCase);
    }
}