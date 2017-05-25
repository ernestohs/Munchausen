![Logo](https://images-cdn.fantasyflightgames.com/filer_public/1e/93/1e9389fe-d6af-47bb-9e7e-74924a333345/solid_barononaflyingcarpet.png)
# Munchausen

:cloud: [![Build Status](https://travis-ci.org/ernestohs/Munchausen.svg)](https://travis-ci.org/ernestohs/Munchausen)

## Data Generation Tool

The main goal is to have a fast, simple and fluent framework for data generation.
```csharp

public class MunchausenImplementation
{
   public static void Main()
   {
        List<Person> persons = Lie<Person>.Generate(100); // this will generate a collection with 100 objects of type Person.
        Person JustOne = Lie<Person>.Generate(); // this will generate only one person.
   }
}
```
