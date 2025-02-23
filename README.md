# Baumeister
Baumeister (german for "master builder") is a simple tool that helps you by generating builder-pattern classes for your objects.

## Usage
To generate a builder class for a class `House` you first have to add a class `HouseBuilder`. 
In order to make it findable for Baumeister, you have to attribute it with the `BuilderAttribute`.
To specify that the builder should generate code for building the `House` class, you have to specify the `BuilderAttribute` with the `House` class as parameter.
```cs
[Builder(typeof(House))]
public class HouseBuilder
{
}
```
The next step is to implement the `BuilderBase` class with the type of the builder class itself, and the type of the object to be built, as parameters.
```cs
[Builder(typeof(House))]
public class HouseBuilder : BuilderBase<HouseBuilder, House>
{
}
```
After comiplation you are ready to use the newly generated builder.
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

[Builder(typeof(House))]
public class HouseBuilder : BuilderBase<HouseBuilder, House>
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
It searches for classes that are attributed with the `BuilderAttribute` and generates extensions methods for that class that enables the builder pattern.
The `Build()` method instantiates the object with the specified properties and returns it. For the instantiation, the builder uses the constructor with the most matching parameters.
