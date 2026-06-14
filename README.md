# Munchausen

<img width="1024" height="434" alt="image" src="https://github.com/user-attachments/assets/c9f3b14f-0705-4b6a-9981-559c937c2744" />

**Inference-first mock-data generation for .NET.** Point Munchausen at one of your
types and it hands back believable instances — names that look like names, emails
that look like emails, prices that look like money — filling in nested objects and
collections for free, with no configuration required. When you do want control, a
small fluent API lets you pin individual members, and everything is reproducible
from a seed.

```csharp
using Munchausen;

// Zero configuration: infer everything from the shape of the type.
Customer customer  = Lie<Customer>.Generate();
IReadOnlyList<Customer> hundred = Lie<Customer>.Generate(100);
```

---

## Why Munchausen

- **Inference first.** It reads your property names and types and picks sensible,
  realistic values automatically. `FirstName` becomes a first name, `Email` becomes
  an email, `CreatedAt` becomes a recent past date, `Price` becomes positive money.
- **Reproducible by design.** The same type, definition, and seed produce the same
  data — on every machine, OS, and runtime. Commit a seed and your fixtures are
  stable forever.
- **Fast.** All reflection, parsing, and inference happen once when you build a
  definition (or first touch `Lie<T>`). Generating objects after that does no
  per-object reflection.
- **Explainable.** Inferred behavior is never a black box — ask a definition to
  explain exactly what it decided for each member, and why.
- **Fluent when you need it.** Override any member, derive values from siblings,
  ignore or preserve fields, supply your own constructor — without losing inference
  for everything you didn't touch.

## Requirements

- .NET 8.0 or later.

## Installation

```bash
dotnet add package Munchausen
```

---

## Quick start

Given a plain model:

```csharp
public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<Order> Orders { get; set; }
}
```

The fastest path is the zero-configuration generic entry point. It infers semantic
values, nested objects, and collections automatically:

```csharp
Customer c = Lie<Customer>.Generate();
// c.FirstName -> "Anthony"
// c.LastName  -> "Hall"
// c.Email     -> "margaretclark@example.net"
// c.CreatedAt -> a date in the recent past
// c.Orders    -> a small list of fully generated Orders, each with Items
```

Pass a count to generate a batch, and `GenerationOptions` to seed it:

```csharp
IReadOnlyList<Customer> batch =
    Lie<Customer>.Generate(100, new GenerationOptions { Seed = 42 });
```

## Definitions: customizing generation

When you want control, define a reusable, immutable definition with `Lie.Define<T>()`.
Anything you don't configure is still inferred:

```csharp
var customers = Lie.Define<Customer>()
    .WithName("Active customers")                       // optional label, shown in Explain()
    .With(x => x.Email, "support@example.com")          // a constant value
    .With(x => x.FirstName, data => data.Name.First())  // a value from a generator
    .Derive(x => x.LastName, (data, x) => $"{x.FirstName}sson")  // from the partial object
    .Ignore(x => x.Id)                                  // leave at its default
    .WithSeed(42)
    .Build();

Customer one             = customers.Generate();
IReadOnlyList<Customer> many = customers.Generate(1000);
```

A definition is mutable to *configure* and immutable once `Build()` is called.
Built definitions are thread-safe and reusable; call `Generate` on the same
definition from as many threads as you like.

### The builder

| Method | What it does |
|---|---|
| `WithName(name)` | Labels the definition (surfaced by `Explain()`). |
| `With(x => x.M, value)` | Assigns a constant. |
| `With(x => x.M, data => ...)` | Assigns from a generator delegate. |
| `Derive(x => x.M, (data, x) => ...)` | Computes a value from the partially built object, after population. |
| `Ignore(x => x.M)` | Leaves the member at its type default. |
| `Preserve(x => x.M)` | Keeps a value set by the constructor or initializer. |
| `ConstructWith(data => ...)` | Supplies a custom constructor. |
| `WithSeed(seed)` / `WithDefaults(...)` | Sets the default seed and other defaults. |
| `Build()` | Validates and compiles into an immutable definition. |

Member targets must be a single property access (`x => x.Email`). Nested paths,
method calls, and indexers are rejected at `Build()` with a clear diagnostic.

---

## How inference works

For every member you didn't configure, Munchausen runs two kinds of inference:

**Semantic inference** matches the member's name (and the containing model's name)
against a curated catalog. Names are normalized, so `FirstName`, `first_name`, and
`FIRST-NAME` are all treated the same. A representative sample of what's recognized:

| Member name | Generates |
|---|---|
| `FirstName`, `LastName`, `FullName` | personal names |
| `Email`, `UserName`, `Url`, `IpAddress` | internet identifiers |
| `Phone` | a fictional `555-01xx` number |
| `StreetAddress`, `City`, `State`, `PostalCode`, `Country` | addresses |
| `CreatedAt`, `UpdatedAt`, `BirthDate`, `ExpiresAt` | suitable past / recent / future dates |
| `Price`, `Cost`, `Total`, `Quantity` | money and counts |
| `Make`, `Model`, `Year`, `Vin` (on a vehicle-ish model) | vehicle data |
| `Description`, `Notes`, `Title` | lorem-style text |

**Type inference** is the fallback for anything not matched by name: useful,
human-plausible defaults per type (small integer ranges, recent dates, a random
`Guid`, a printable string, and so on), rather than full-range extremes.

### Inference modes

Each semantic match carries a confidence, and a mode decides which to accept:

```csharp
var defs = Lie.Define<Product>()
    .WithDefaults(new GenerationDefaults
    {
        SemanticInference = SemanticInferenceMode.Conservative
    })
    .Build();
```

- `Conservative` — high-confidence matches only.
- `Balanced` — high and medium confidence. **(default)**
- `Aggressive` — also accepts low-confidence matches.
- `Disabled` — skip semantic matching; use type inference only.

Rejected candidates fall back to type inference, so output is always *something*
reasonable.

---

## Determinism and seeding

Munchausen owns its random number generator, so seeded output is identical
everywhere. Seeds can come from the definition or per call; per-call options win:

```csharp
var def = Lie.Define<Customer>().WithSeed(42).Build();

var a = def.Generate(100);                                   // uses seed 42
var b = def.Generate(100, new GenerationOptions { Seed = 7 }); // overrides to 7
// a is reproducible; running it again yields the same 100 customers.
```

Date and time generation uses a single **reference time** resolved once per call
(from options, the definition default, an injected `TimeProvider`, or the current
UTC time) — it never reads the clock repeatedly mid-batch, so dated output is
reproducible too:

```csharp
var fixedNow = new GenerationOptions
{
    ReferenceTime = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
};
Customer c = def.Generate(fixedNow);
```

Generation without a seed produces fresh random data each run.

---

## Datasets

Inside any generator or derivation delegate you get a context exposing the
data sources Munchausen uses internally. Reach for these to build coherent values:

```csharp
var users = Lie.Define<User>()
    .With(u => u.FullName, data => data.Name.FullName())
    .With(u => u.City,     data => data.Address.City())
    .With(u => u.Bio,      data => data.Lorem.Sentence())
    .With(u => u.SignedUp, data => data.Date.Past(years: 3))
    .With(u => u.Discount, data => data.Random.Double(0, 0.5))
    .Build();
```

First-class datasets on the context:

- `data.Random` — primitives and choices: `Int`, `Long`, `Decimal`, `Double`,
  `Bool`, `Guid`, `Bytes`, `Pick`, `Weighted`, `Sample`, `Enum`, and more (all
  numeric bounds are inclusive).
- `data.Name` — `First`, `Last`, `FullName`.
- `data.Internet` — `Email`, `UserName`, `DomainName`, `Url`, `IpAddress`.
- `data.Address` — `StreetAddress`, `City`, `State`, `PostalCode`, `Country`,
  `CountryCode`, `Latitude`, `Longitude`.
- `data.Date` — `Past`, `Recent`, `Soon`, `Future`, `Between`, `BirthDate`.
- `data.Lorem` — `Word`, `Words`, `Sentence`, `Paragraph`, `Text`.

Two more ship in the box and are reached generically:

```csharp
string vin   = data.Dataset<VehicleData>().Vin();    // valid check digit
decimal price = data.Dataset<CommerceData>().Price(); // rounded money
```

A coherent pair (the email derived from the chosen name) is just a matter of
drawing once and reusing:

```csharp
.With(u => u.Email, data =>
{
    var first = data.Name.First();
    var last  = data.Name.Last();
    return data.Internet.Email(first, last);
})
```

Generated contact data is deliberately fictional-safe: email domains are the
reserved `example.com` / `example.org` / `example.net`, IPs come from the
documentation TEST-NET ranges, and phone numbers use the fictional `555-01xx` band.

---

## Nested objects, collections, and cycles

Nested objects and collections are generated automatically, to a bounded depth.
Collections default to a small number of elements; you can tune depth and cycle
behavior:

```csharp
var employees = Lie.Define<Employee>()
    .WithDefaults(new GenerationDefaults
    {
        MaximumDepth = 3,
        CycleBehavior = CycleBehavior.Terminate  // or Throw
    })
    .Build();
```

Reference cycles are detected along the active path, so a self-referential
`Employee.Manager` terminates safely (as `null` under `Terminate`) while two
siblings of the same type — say a billing and a shipping address — are both
generated. Under `Terminate`, depth limits resolve nullable references to `null`
and collections to empty.

---

## Records and constructors

Constructor-based and immutable models work out of the box. A constructor's
parameters use the same inference as properties:

```csharp
public record Car(Guid Id, string Make, string Model, int Year);

Car car = Lie<Car>.Generate();
```

Selection is automatic (the most resolvable public constructor wins), or you can
mark one with `[LieConstructor]`, or take over entirely with `ConstructWith`:

```csharp
var orders = Lie.Define<Order>()
    .ConstructWith(data => new Order(
        data.Generate<Customer>(),       // generate a nested model on demand
        data.GenerateMany<Item>(3)))
    .Build();
```

---

## Explain what was inferred

Inferred behavior is inspectable. Ask a built definition to explain itself:

```csharp
InferenceReport report = Lie.Define<Car>().Build().Explain();

Console.WriteLine(report.ToText());
// Car (complete)
//   Car.Id    -> Random.Guid     [type]
//   Car.Make  -> Vehicle.Make    [property name + model] High
//   Car.Model -> Vehicle.Model   [property name + model] High
//   Car.Year  -> Vehicle.Year    [property name + model] High
//   Car.Owner -> (nested object) [nested object]
```

The report is structured data too — `report.Members` gives the source, generator,
and confidence per member — so you can build tooling on top of it. The exact text
of `ToText()` is for humans and may change between versions; the structured model
is the stable contract.

---

## Error handling

`Build()` validates the whole definition and reports **every** problem at once,
each with a stable diagnostic code, rather than failing one at a time:

```csharp
try
{
    var bad = Lie.Define<Car>()
        .With(c => c.Owner.Name, "x")  // not a single-member target
        .WithName("   ")               // empty name
        .Build();
}
catch (LieDefinitionException ex)
{
    Console.WriteLine(ex.Message);          // summary of the first error + count
    foreach (var d in ex.Diagnostics)        // every error, with codes
        Console.WriteLine($"{d.Code}: {d.Message}");
}
```

Failures during generation (for example, an exception thrown by one of your own
generator delegates) surface as a `LieGenerationException` that reports the model
type, member path, index, and lifecycle phase, with your original exception
preserved as the inner exception. Cancellation passes through untouched as the
standard `OperationCanceledException`.

---

## At a glance

- Automatic, zero-config generation via `Lie<T>.Generate(...)`.
- Reusable, immutable, thread-safe definitions via `Lie.Define<T>().Build()`.
- Semantic + type inference, with selectable confidence modes.
- Reproducible, owned PRNG; fixed per-call reference time; `TimeProvider` support.
- Built-in datasets for names, internet, addresses, dates, lorem, vehicles, and
  commerce — plus a full primitive `Random` surface.
- Automatic nested objects and collections with depth and cycle safety.
- Records, immutable models, and custom constructors.
- Structured, explainable inference.
- Aggregated, coded build diagnostics and wrapped generation failures.

## Status

Munchausen v1.0 (Foundation). The public API is intentionally small and stable.
