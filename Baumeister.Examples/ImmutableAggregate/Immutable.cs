namespace Baumeister.Examples.ImmutableAggregate;

public class Immutable
{
    private string _privateFieldUnderscore;
    private string privateField;

    public Immutable(string privateFieldUnderscore, string privateField)
    {
        _privateFieldUnderscore = privateFieldUnderscore;
        this.privateField = privateField;
    }

    public override string ToString()
    {
        return $"Immutable: _privateFieldUnderscore={_privateFieldUnderscore}, privateField={privateField}";
    }
}
