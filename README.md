# Signals.net

## Read/Write Signals
There are two types of signals.  Read/write signals can be assigned values.  Here we create one
called **counter** which we initially a assign a value of 4

```csharp 
var counter = Signal.State(4);
```

this can be changed to a five etc using `Set`.

```csharp 
counter.Set(5);
```

and read back using `Get`

```csharp
Console.WriteLine(counter.Get());   // prints 5
```

## Computed Signals

The second type of signals are computed ones,  these are based on other signals

```csharp
var isEven = Signal.Computed(() => counter.Get() % 2 == 0 );
```

```csharp
Console.WriteLine(isEven.Get());   // prints false as counter is 5
```

```csharp
counter.Set(6);
Console.WriteLine(isEven.Get());   // prints true as counter is 6
```

---

Computed signals can also be based on other computed signals

```csharp
var parity = Signal.Computed(() => isEven.Get() ? "Even" : "Odd";);
```
```csharp
Console.WriteLine(parity.Get());   // prints Even as isEven is true`
```

```csharp
counter.Set(9);
Console.WriteLine(parity.Get());   // prints odd as isEven is now false
```

---
or on multiple signals

```csharp
var firstname = Signal.State("David");
var lastname = Signal.State("Betteridge");
var fullname = Signal.Computed(() => $"{firstname.Get()} {lastname.Get()}");
```

## Effects

Effects can be added to signals which are automatically triggered when a value changes

```csharp
counter.AddEffect((previous, current) => Console.WriteLine($"Counter changed {previous} to {current}"));

isEven.AddEffect((previous, current) => Console.WriteLine($"IsEven changed {previous} to {current}"));

parity.AddEffect((previous, current) => Console.WriteLine($"Parity changed {previous} to {current}"));
```

``` csharp
counter.Set(10);
// Counter changed 9 to 10
// IsEven changed False to True
// Parity changed Odd to Even
```

``` csharp
counter.Set(12);
// Counter changed 10 to 12
```

Effects can also be removed from a signal.

``` csharp
var effect = isEven.AddEffect((p, c) => { ... }));
isEven.RemoveEffect(effect);
```

---

## Lazy

In the previous example not only aren't the other two effects triggered, but parity isn't even computed.

``` csharp
var parity = SignalBuilder.DependsOn(isEven).ComputedBy(v =>
{
    Console.WriteLine("Compute parity");
    return v.Get() ? "Even" : "Odd";
});

// This changes the value for isEven so parity is computed
counter.Set(13);
Console.WriteLine(parity.Get());

// This doesn't change the value for isEven so parity isn't computed
counter.Set(15);
Console.WriteLine(parity.Get());
```


## Equality

So far we have only looked at a signals based on a primitive type (integer).  In the real world we need to
cater for classes/records/arrays/lists etc.  For example:

``` csharp

record People(string Name, int Age);

var data = Signal.State<List<People>>([new People("David", 48)]); 

var adults = Signal.Computed(() => data.Get().Where(p => p.Age > 18).ToList());

var numberOfAdults = Signal.Computed(() => adults.Get().Count );

numberOfAdults.AddEffect((oldCount, newCount) => 
    Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));

```

The **numberOfAdults** signal only needs to be computed if **adults** changes.

In order to get this to work,  we have to supply your own Equality function. This is provided using a second (optional) 
argument to the **Signal.State** and **Signal.Computed** methods.

```csharp
var adults = Signal.Computed(() => data.Get().Where(p => p.Age > 18).ToList(), 
                            CompareOrderedLists);
```

```csharp
bool CompareOrderedLists<T>(IList<T> lhs, IList<T> rhs) where T : notnull
{
    // T must be a record type as they generate the EqualityContract property
    // There is no proper way to enforce this in C# however.
    if (lhs.Count != rhs.Count)
        return false;

    for (var i = 0; i < lhs.Count; i++)
    {
        if (!lhs[i].Equals(rhs[i]))
            return false;
    }

    return true;
}
```