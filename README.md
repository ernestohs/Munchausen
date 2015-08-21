[![Build Status](https://travis-ci.org/ernestohs/Munchausen.svg)](https://travis-ci.org/ernestohs/Munchausen)

# Munchausen
Data Generation Tool

The main goal is to have a fast, simple and fluent framework for data generation.

    List<Person> persons = Lie<Person>.Generate(100); // this will generate a collection with 100 objects of type Person.
    Person JustOne = Lie<Person>.Generate(); // this will generate only one person.

