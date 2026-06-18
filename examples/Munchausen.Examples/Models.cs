namespace Munchausen.Examples;

// Self-contained domain models used across the example chapters. They are
// deliberately ordinary: plain properties, a record, an enum, a self-referential
// type, and a constructor-based aggregate. Munchausen infers values for all of
// them from member names and types with no configuration.
//
// Models are public because Munchausen lives in a separate assembly and
// constructs and populates these types from the outside, exactly as it would
// for a consumer's own public models.

/// <summary>A person. Member names drive name, email, age, address, and date inference.</summary>
public sealed class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>A postal address. Used both standalone and as nested members.</summary>
public sealed class Address
{
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

/// <summary>The type name carries the "car" hint, so vehicle members infer at high confidence.</summary>
public sealed class Car
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Vin { get; set; }
}

/// <summary>A record generated through its primary constructor; the "product" hint routes Name to a product name.</summary>
public sealed record Product(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string Category,
    string Currency);

/// <summary>Self-referential type used to demonstrate cycle handling.</summary>
public sealed class Employee
{
    public string Name { get; set; }
    public string Title { get; set; }
    public Employee? Manager { get; set; }
}

/// <summary>A line item inside an order.</summary>
public sealed class Item
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>Status of an order; demonstrates enum type inference.</summary>
public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Delivered,
    Cancelled,
}

/// <summary>
/// A constructor-based aggregate. The two <see cref="Address"/> members are siblings,
/// not a cycle, so both are generated.
/// </summary>
public sealed class Order
{
    public Order(Customer customer, IReadOnlyList<Item> items, OrderStatus status)
    {
        Customer = customer;
        Items = items;
        Status = status;
    }

    public Customer Customer { get; }
    public IReadOnlyList<Item> Items { get; }
    public OrderStatus Status { get; }
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
}

/// <summary>Has an initialized member, used to demonstrate <c>Preserve</c>.</summary>
public sealed class Coupon
{
    public string Code { get; set; }

    /// <summary>Initialized to true; Preserve keeps this instead of inferring a random bool.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Members chosen at different inference confidences, to contrast the semantic modes.</summary>
public sealed class Ticket
{
    public string Email { get; set; }   // High confidence
    public string Title { get; set; }   // Medium confidence
    public string Notes { get; set; }   // Medium confidence
    public string Code { get; set; }    // Low confidence
}

/// <summary>A wide model used to showcase every dataset in one place.</summary>
public sealed class Profile
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Website { get; set; }
    public string IpAddress { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public double Latitude { get; set; }
    public string Bio { get; set; }
    public DateTimeOffset MemberSince { get; set; }
    public int Roll { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public string Tier { get; set; }
    public IReadOnlyList<string> Tags { get; set; }
    public OrderStatus LastStatus { get; set; }
    public string CarMake { get; set; }
    public string Vin { get; set; }
    public string FeaturedProduct { get; set; }
    public string Sku { get; set; }
    public string Currency { get; set; }
}

/// <summary>Two constructors; <see cref="LieConstructorAttribute"/> selects which one Munchausen uses.</summary>
public sealed class Reservation
{
    public Reservation()
    {
    }

    [LieConstructor]
    public Reservation(string code, DateTimeOffset checkIn)
    {
        Code = code;
        CheckIn = checkIn;
    }

    public string Code { get; set; }
    public DateTimeOffset CheckIn { get; set; }
    public string GuestName { get; set; }
}

// A chain of distinct types to demonstrate the maximum-depth limit. Nested members
// are nullable so that termination at the depth limit is visible as null.

/// <summary>Top of a three-level nesting chain.</summary>
public sealed class Level1
{
    public string Label { get; set; }
    public Level2? Next { get; set; }
}

/// <summary>Middle of the nesting chain.</summary>
public sealed class Level2
{
    public string Label { get; set; }
    public Level3? Next { get; set; }
}

/// <summary>Bottom of the nesting chain.</summary>
public sealed class Level3
{
    public string Label { get; set; }
}

/// <summary>A line on an <see cref="Invoice"/>; LineTotal is derived from quantity and price.</summary>
public sealed class LineItem
{
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>An invoice whose money fields are derived, so the document is internally coherent.</summary>
public sealed class Invoice
{
    public string Number { get; set; }
    public Customer BillTo { get; set; }
    public IReadOnlyList<LineItem> Lines { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset IssuedOn { get; set; }
    public DateTimeOffset DueOn { get; set; }
}
