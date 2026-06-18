# Munchausen Examples and Cookbook

A working tour of everything Munchausen v1.0 can do, from a one-line `Generate`
to fully derived, internally coherent object graphs. Every snippet here is
backed by a runnable chapter in the
[`Munchausen.Examples`](../examples/Munchausen.Examples) console project, so you
can read it and run it.

For the authoritative contract behind any behavior, see the
[Munchausen Contributor Handbook](MUNCHAUSEN_HANDBOOK.md).

## Contents

- [Run these yourself](#run-these-yourself)
- [At a glance](#at-a-glance)
- [Feature tour](#feature-tour)
  - [1. Basic generation](#1-basic-generation)
  - [2. The builder](#2-the-builder)
  - [3. Semantic inference modes](#3-semantic-inference-modes)
  - [4. Datasets](#4-datasets)
  - [5. Determinism and seeding](#5-determinism-and-seeding)
  - [6. Nested objects, collections, and cycles](#6-nested-objects-collections-and-cycles)
  - [7. Records and constructors](#7-records-and-constructors)
  - [8. Explain and introspection](#8-explain-and-introspection)
  - [9. Error handling](#9-error-handling)
  - [10. Cancellation and the generation context](#10-cancellation-and-the-generation-context)
- [Real-world recipes](#real-world-recipes)
- [Dataset reference](#dataset-reference)
- [Inference cheat sheet](#inference-cheat-sheet)
- [Diagnostics (LIE codes)](#diagnostics-lie-codes)
- [Not in v1.0](#not-in-v10)

## Run these yourself

```bash
export PATH="$HOME/.dotnet:$PATH"   # if the dotnet SDK is not already on PATH
dotnet run --project examples/Munchausen.Examples              # every chapter
dotnet run --project examples/Munchausen.Examples -- datasets  # one chapter (substring match)
dotnet run --project examples/Munchausen.Examples -- recipes   # the real-world recipes
```

The example models (`Customer`, `Car`, `Product`, `Order`, `Invoice`, and
friends) are defined in
[`Models.cs`](../examples/Munchausen.Examples/Models.cs).

## At a glance

**Entry points**

| Call | Result |
|---|---|
| `Lie<T>.Generate(options?, ct)` | one inferred `T` |
| `Lie<T>.Generate(count, options?, ct)` | `IReadOnlyList<T>` |
| `Lie.Define<T>()` | a `LieDefinitionBuilder<T>` to customize |
| `LieDefinition<T>.Generate(...)` / `.Generate(count, ...)` | run a compiled definition |
| `LieDefinition<T>.Explain()` | an `InferenceReport` (no values generated) |

**Builder methods** (chain on `Lie.Define<T>()`, then `Build()`)

| Method | Purpose |
|---|---|
| `WithName(name)` | label the definition (shows up in `Explain`) |
| `With(x => x.M, value)` | pin a constant |
| `With(x => x.M, ctx => ...)` | generate from a delegate |
| `Derive(x => x.M, (ctx, obj) => ...)` | compute from the partially built object |
| `Ignore(x => x.M)` | leave at the type default |
| `Preserve(x => x.M)` | keep a constructor or initializer value |
| `ConstructWith(ctx => new T(...))` | take over construction |
| `WithSeed(seed)` / `WithDefaults(defaults)` | set the seed and other defaults |

**Per-call vs definition defaults**

| `GenerationOptions` (per call) | `GenerationDefaults` (on the definition) |
|---|---|
| `Seed`, `ReferenceTime`, `TimeProvider` | `Seed`, `ReferenceTime`, `MaximumDepth`, `CycleBehavior`, `SemanticInference` |

## Feature tour

### 1. Basic generation

The automatic path infers every member from its name and type. Source:
[`BasicGeneration.cs`](../examples/Munchausen.Examples/BasicGeneration.cs).

```csharp
using Munchausen;

Customer one    = Lie<Customer>.Generate();
var hundred     = Lie<Customer>.Generate(100);
var reproducible = Lie<Customer>.Generate(100, new GenerationOptions { Seed = 7 });
```

### 2. The builder

`Lie.Define<T>()` builds an immutable, reusable, thread-safe definition. Pin with
`With`, compute with `Derive`, skip with `Ignore`, keep an existing value with
`Preserve`. Everything you do not touch is still inferred. Source:
[`BuilderCustomization.cs`](../examples/Munchausen.Examples/BuilderCustomization.cs).

```csharp
LieDefinition<Customer> vip = Lie.Define<Customer>()
    .WithName("VIP customers")
    .With(c => c.Country, "United States")                 // constant
    .With(c => c.FirstName, data => data.Name.First())     // generator delegate
    .Derive(c => c.Email, (data, c) =>                     // computed from the built object
        $"{c.FirstName}.{c.LastName}@vip.example.com".ToLowerInvariant())
    .Ignore(c => c.Age)                                    // stays 0
    .WithSeed(42)
    .Build();

Customer customer = vip.Generate();
```

`Preserve` keeps a value the constructor or an initializer set, instead of
overwriting it:

```csharp
// Coupon.IsActive is declared `= true`; Preserve keeps it instead of a random bool.
Coupon coupon = Lie.Define<Coupon>().Preserve(x => x.IsActive).Build().Generate();
```

### 3. Semantic inference modes

`SemanticInferenceMode` controls how much confidence a name match needs.
Source:
[`SemanticInference.cs`](../examples/Munchausen.Examples/SemanticInference.cs).

```csharp
Lie.Define<Ticket>()
    .WithDefaults(new GenerationDefaults { SemanticInference = SemanticInferenceMode.Conservative })
    .Build();
```

| Member (confidence) | Conservative | Balanced (default) | Aggressive | Disabled |
|---|---|---|---|---|
| `Email` (High) | semantic | semantic | semantic | type |
| `Title`, `Notes` (Medium) | type | semantic | semantic | type |
| `Code` (Low) | type | type | semantic | type |

### 4. Datasets

Inside a generator delegate the `GenerationContext` exposes `Name`, `Internet`,
`Address`, `Date`, `Lorem`, and `Random`; `Vehicle` and `Commerce` come from
`Dataset<T>()`. See the full [Dataset reference](#dataset-reference) below.
Source: [`Datasets.cs`](../examples/Munchausen.Examples/Datasets.cs).

```csharp
Lie.Define<Profile>()
    .With(x => x.FullName,        d => d.Name.FullName())
    .With(x => x.Email,           d => d.Internet.Email())
    .With(x => x.Street,          d => d.Address.StreetAddress())
    .With(x => x.Bio,             d => d.Lorem.Sentence())
    .With(x => x.MemberSince,     d => d.Date.Past(years: 3))
    .With(x => x.Tier,            d => d.Random.Pick("Bronze", "Silver", "Gold"))
    .With(x => x.Tags,            d => d.Random.Sample(["new", "vip", "trial", "partner"], 2))
    .With(x => x.CarMake,         d => d.Dataset<VehicleData>().Make())
    .With(x => x.FeaturedProduct, d => d.Dataset<CommerceData>().ProductName())
    .Build();
```

`Random.Weighted` draws in proportion to weights (about 70/25/5 over many draws):

```csharp
d.Random.Weighted(
[
    new WeightedValue<string>("Free", 0.70),
    new WeightedValue<string>("Pro", 0.25),
    new WeightedValue<string>("Enterprise", 0.05),
]);
```

### 5. Determinism and seeding

Same type, definition, and seed produce identical output. Values that depend on
time also depend on the reference time, so pin it too for byte-for-byte
reproducibility. Source:
[`DeterminismAndSeeding.cs`](../examples/Munchausen.Examples/DeterminismAndSeeding.cs).

```csharp
var anchor = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
LieDefinition<Customer> seeded = Lie.Define<Customer>()
    .WithDefaults(new GenerationDefaults { Seed = 42, ReferenceTime = anchor })
    .Build();

IReadOnlyList<Customer> run1 = seeded.Generate(3);
IReadOnlyList<Customer> run2 = seeded.Generate(3);   // member-for-member identical

seeded.Generate(new GenerationOptions { Seed = 7 }); // per-call seed overrides the definition

// A TimeProvider is the testable seam for "now" when no ReferenceTime is given.
Lie<Customer>.Generate(new GenerationOptions { TimeProvider = myClock });
```

### 6. Nested objects, collections, and cycles

Nested objects and collections fill in automatically to a bounded depth.
`CycleBehavior.Terminate` (default) yields null or empty at a cycle or the
`MaximumDepth` limit; `CycleBehavior.Throw` raises a `LieGenerationException`.
Source:
[`NestedCollectionsAndCycles.cs`](../examples/Munchausen.Examples/NestedCollectionsAndCycles.cs).

```csharp
Order order = Lie<Order>.Generate();   // nested Customer, a list of Items, two Addresses

Employee emp = Lie<Employee>.Generate();   // self-referential Manager terminates as null

Lie.Define<Level1>()
    .WithDefaults(new GenerationDefaults { MaximumDepth = 1, CycleBehavior = CycleBehavior.Throw })
    .Build();
```

Supported collection shapes: `T[]`, `List<T>`, `IList<T>`, `ICollection<T>`,
`IEnumerable<T>`, `IReadOnlyList<T>`, `IReadOnlyCollection<T>`. Dictionaries
materialize empty in v1.0.

### 7. Records and constructors

Records and constructor-based types work out of the box; parameters use the same
inference as properties. `[LieConstructor]` picks a constructor; `ConstructWith`
takes over construction and can generate nested values on demand. Source:
[`RecordsAndConstructors.cs`](../examples/Munchausen.Examples/RecordsAndConstructors.cs).

```csharp
Product product = Lie<Product>.Generate();   // built through the record's constructor

LieDefinition<Order> orders = Lie.Define<Order>()
    .ConstructWith(data => new Order(
        data.Generate<Customer>(),     // one nested Customer
        data.GenerateMany<Item>(3),    // exactly three Items
        OrderStatus.Paid))
    .Build();
```

### 8. Explain and introspection

`Explain()` inspects the compiled plan without generating values. Use the
structured `InferenceReport` for stable assertions; `ToText()` is for humans.
Source:
[`ExplainAndIntrospection.cs`](../examples/Munchausen.Examples/ExplainAndIntrospection.cs).

```csharp
InferenceReport report = Lie.Define<Car>().Build().Explain();

Console.WriteLine(report.ToText());
foreach (MemberInferenceReport m in report.Members)
{
    Console.WriteLine($"{m.MemberPath}: {m.Generator} [{m.Source}] {m.Confidence}");
    // m.OverriddenRules, m.DerivationOrder, and report.IsComplete are also available.
}
```

### 9. Error handling

`Build()` aggregates every detectable error into one `LieDefinitionException`,
each diagnostic carrying a stable LIE code. A failure inside a user delegate at
generation time is wrapped once as a `LieGenerationException` (with `ModelType`,
`MemberPath`, `GenerationIndex`, `Phase`, and the original as `InnerException`).
`OperationCanceledException` is never wrapped. Source:
[`ErrorHandling.cs`](../examples/Munchausen.Examples/ErrorHandling.cs).

```csharp
try
{
    definition.Generate();
}
catch (LieGenerationException ex)
{
    log.Error($"{ex.ModelType.Name}.{ex.MemberPath} failed in {ex.Phase}", ex.InnerException);
}
```

### 10. Cancellation and the generation context

Every `Generate` call accepts a `CancellationToken`. Inside a generator delegate
the context exposes the root `Index` and the current `MemberPath`. Source:
[`CancellationAndContext.cs`](../examples/Munchausen.Examples/CancellationAndContext.cs).

```csharp
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(5));
Lie<Customer>.Generate(1_000_000, cancellationToken: cts.Token);

Lie.Define<Customer>()
    .With(c => c.Email, data => $"user{data.Index}@example.com")   // row-dependent value
    .Build();
```

## Real-world recipes

The recipes that combine the pieces into things you actually build. All of these
run in [`Recipes.cs`](../examples/Munchausen.Examples/Recipes.cs)
(`dotnet run --project examples/Munchausen.Examples -- recipes`).

### Test fixture with pinned assertions

Arrange a believable subject and pin only the fields the test asserts on; let the
rest stay realistic.

```csharp
Customer subject = Lie.Define<Customer>()
    .With(c => c.Email, "known@example.com")
    .With(c => c.Age, 30)
    .WithSeed(1)
    .Build()
    .Generate();
```

### Coherent data: derive one field from another

`Derive` runs after members are populated, so it can read the generated values.
Here the email matches the generated name.

```csharp
Lie.Define<Customer>()
    .With(c => c.FirstName, d => d.Name.First())
    .With(c => c.LastName,  d => d.Name.Last())
    .Derive(c => c.Email, (d, c) => d.Internet.Email(c.FirstName, c.LastName))
    .Build();
// Jennifer White -> jennifer_white@example.net
```

### A composed object graph with coherent money

Build a reusable child definition, compose it into the parent with
`GenerateMany(childDefinition, count)`, then derive totals. Derivations run in
registration order, so later ones (Total) see earlier ones (Subtotal, Tax).

```csharp
LieDefinition<LineItem> line = Lie.Define<LineItem>()
    .With(l => l.Description, d => d.Dataset<CommerceData>().ProductName())
    .With(l => l.Quantity,    d => d.Random.Int(1, 5))
    .With(l => l.UnitPrice,   d => d.Dataset<CommerceData>().Price(5m, 200m))
    .Derive(l => l.LineTotal, (d, l) => l.Quantity * l.UnitPrice)
    .Build();

Invoice invoice = Lie.Define<Invoice>()
    .With(i => i.Number, d => $"INV-{d.Random.Int(1000, 9999)}")
    .With(i => i.Lines,  d => d.GenerateMany(line, 3))             // reuse the child definition
    .Derive(i => i.Subtotal, (d, i) => i.Lines.Sum(l => l.LineTotal))
    .Derive(i => i.Tax,      (d, i) => Math.Round(i.Subtotal * 0.10m, 2))
    .Derive(i => i.Total,    (d, i) => i.Subtotal + i.Tax)
    .Derive(i => i.DueOn,    (d, i) => i.IssuedOn.AddDays(30))
    .WithDefaults(new GenerationDefaults { Seed = 3, ReferenceTime = anchor })
    .Build()
    .Generate();

// invoice.Subtotal == invoice.Lines.Sum(l => l.LineTotal)  ->  true
```

### Reproduce an exact case by committing its seed

Capture the seed of an interesting (or failing) case and replay it later for a
byte-for-byte identical object.

```csharp
var options = new GenerationOptions { Seed = 1234, ReferenceTime = anchor };
Customer first  = Lie<Customer>.Generate(options);
Customer replay = Lie<Customer>.Generate(options);   // identical to first
```

### A deterministic, dated cohort

Pin the seed and reference time to get a stable, dated population for tests or
demos.

```csharp
IReadOnlyList<Customer> cohort = Lie.Define<Customer>()
    .With(c => c.CreatedAt, d => d.Date.Past(years: 1))
    .WithDefaults(new GenerationDefaults { Seed = 7, ReferenceTime = anchor })
    .Build()
    .Generate(5);
```

### Lock an inference decision with Explain

Treat inference as a contract: assert how a member resolves, with no values
generated.

```csharp
InferenceReport report = Lie.Define<Car>().Build().Explain();
MemberInferenceReport make = report.Members.Single(m => m.MemberPath == "Make");
Assert.Equal(InferenceSource.Semantic, make.Source);
Assert.Equal("Vehicle.Make", make.Generator);
```

### Bulk generation

All reflection happens at `Build`, so large batches are fast (100,000 customers
generate in a few hundred milliseconds).

```csharp
IReadOnlyList<Customer> rows = Lie<Customer>.Generate(100_000, new GenerationOptions { Seed = 1 });
```

## Dataset reference

All methods are seeded and reproducible. `Name`, `Internet`, `Address`, `Date`,
`Lorem`, and `Random` are properties on the context; `Vehicle` and `Commerce`
are resolved with `data.Dataset<T>()`.

**`data.Name`**

| Method | Returns |
|---|---|
| `First()`, `Last()`, `FullName()` | personal names |

**`data.Internet`**

| Method | Returns |
|---|---|
| `Email()`, `Email(first, last)` | fictional-safe email (coherent with a name) |
| `UserName()`, `DomainName()`, `Url()`, `IpAddress()` | handle, domain, URL, TEST-NET IPv4 |

**`data.Address`**

| Method | Returns |
|---|---|
| `StreetAddress()`, `City()`, `State()`, `PostalCode()` | postal parts |
| `Country()`, `CountryCode()` | country name, ISO alpha-2 |
| `Latitude()`, `Longitude()` | geo coordinates |

**`data.Date`** (relative to the operation's reference time, all UTC)

| Method | Returns |
|---|---|
| `Past(years = 10)`, `Recent(days = 30)` | a past instant |
| `Soon(days = 30)`, `Future(years = 10)` | a future instant |
| `Between(min, max)` | an instant in a range |
| `BirthDate(minAge = 18, maxAge = 80)` | a birth date for an age range |
| `ReferenceTime` | the fixed reference time itself |

**`data.Lorem`**

| Method | Returns |
|---|---|
| `Word()`, `Words(count)` | filler word(s) |
| `Sentence()`, `Paragraph()`, `Text(maxLength)` | filler prose |

**`data.Random`**

| Method | Returns |
|---|---|
| `Bool(probability = 0.5)` | a weighted coin flip |
| `Int(min, max)`, `Long(min, max)` | integers (inclusive bounds) |
| `Double(min = 0, max = 1)`, `Decimal(min, max, decimals = 2)` | reals |
| `String(length)`, `AlphaNumeric(length)`, `Bytes(length)`, `Guid()` | primitives |
| `Pick(values)`, `Sample(values, count, replacement = false)` | choose one / many |
| `Weighted(weightedValues)` | probability-weighted choice |
| `Enum<TEnum>()` | a defined enum value |

**`data.Dataset<VehicleData>()`**

| Method | Returns |
|---|---|
| `Make()`, `Model()`, `Model(make)` | manufacturer and model (coherent with a make) |
| `Year()`, `Vin()` | model year, 17-char VIN with valid check digit |

**`data.Dataset<CommerceData>()`**

| Method | Returns |
|---|---|
| `Price(min = 1, max = 1000, decimals = 2)` | a money value |
| `ProductName()`, `Category()`, `Sku()`, `CurrencyCode()` | product fields, ISO 4217 code |

## Inference cheat sheet

**Resolution order per member:** explicit rule (`With`/`Derive`/`Ignore`/`Preserve`)
&rarr; semantic inference (name + model hint + type) &rarr; type inference &rarr;
nested object / collection &rarr; left untouched (reported as `Unsupported`).

**Semantic highlights** (member name &rarr; generator; many are higher confidence
with a matching model-name hint such as `customer`, `product`, or `car`):

| Names | Generator |
|---|---|
| `firstName`, `lastName`, `fullName`, `name` | `Name.*` |
| `email`, `username`, `url`, `domain`, `ipAddress` | `Internet.*` |
| `street`, `city`, `state`, `postalCode`, `country`, `latitude`, `longitude` | `Address.*` |
| `createdAt`, `updatedAt`, `birthDate`, `expiresAt`, `dueDate` | `Date.*` |
| `price`, `cost`, `sku`, `category`, `currency`, `quantity` | `Commerce.*` / ranges |
| `make`, `model`, `year`, `vin` (with a vehicle hint) | `Vehicle.*` |
| `description`, `notes`, `title`, `bio` | `Lorem.*` |

**Type inference defaults** (when no semantic candidate survives):

| Type | Value |
|---|---|
| `bool` | 50/50 |
| integers | bounded non-negative ranges (e.g. `int` 0-10,000) |
| `float`/`double`/`decimal` | bounded reals |
| `string` | `Lorem.Words(2)` |
| `Guid` | random GUID |
| `DateTime`/`DateTimeOffset`/`DateOnly` | a recent instant |
| `TimeOnly`/`TimeSpan` | uniform over a day |
| enum | a defined value (one flag for `[Flags]`) |
| `Uri` | an absolute HTTPS URI |
| `byte[]` | 16 random bytes |

## Diagnostics (LIE codes)

| Code | Meaning |
|---|---|
| LIE001 | Invalid member expression (not `x => x.Member`) |
| LIE002 | Conflicting member rules (e.g. `With` + `Ignore`) |
| LIE003 | Unresolved required member |
| LIE004 | Ambiguous constructor |
| LIE005 | Invalid option (e.g. a blank definition name) |
| LIE011 | A `Preserve` target may never be assigned (warning) |

## Not in v1.0

Composition beyond passing a child definition, custom inference providers and
datasets, uniqueness, validation, lifecycle hooks, locales beyond `en`,
dictionary key/value synthesis, and async/streaming are not part of the v1.0
surface. See the Roadmap Appendix in the
[handbook](MUNCHAUSEN_HANDBOOK.md).
