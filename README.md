# Baumeister
Baumeister (german for "master builder") is a simple tool that helps by generating builder-pattern classes for your objects.

# Installation
For easy installation just use the dotnet cli
```cli
dotnet add package Baumeister.Generators
```

## Usage
To generate a builder class for a class `House` you first have to add a partial class `HouseBuilder`. 
In order to make it findable for Baumeister, you have to make it a generic specialization of `BuilderBase<T>`.
```cs
public partial class HouseBuilder : BuilderBase<House>
{
}
```
After compilation you are ready to use the newly generated builder.
## Examples

### Simple Example
```cs
public class House
{
	public int Floors { get; }

	public int Rooms { get; }

	public bool HasGarden { get; }

	public House(string name, int floors, int rooms, bool hasGarden)
	{
		Name = name;
		Floors = floors;
		Rooms = rooms;
		HasGarden = hasGarden;
	}
}

public partial class HouseBuilder : BuilderBase<House>
{
}

class Program
{
	static void Main(string[] args)
	{
		var house = new HouseBuilder()
			.WithFloors(2)
			.WithRooms(5)
			.WithHasGarden(true)
			.Build();
	}
}
```
## How it works
Baumeister uses the Roslyn compiler to analyze the source code of the project.
It searches for classes that are specializations of `BuilderBase` and generates a partial class with methods that enables the builder pattern.
The `Build()` method instantiates the object with the specified properties and returns it. For the instantiation, the builder uses the constructor with the most matching parameters.
It matches the given parameters with the constructor parameters and tries to find the best match. In order to do that it uses the type and names of the properties. 
In case not all properties that are set are used by the constructor, the builder will use public properties to set the values.
