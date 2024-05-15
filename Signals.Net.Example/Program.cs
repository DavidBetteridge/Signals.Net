using Signals.Net;

// Example 1 - Counter -> IsEven -> Parity
var counter = Signal.State(2);
var isEven = Signal.Computed(() => counter.Get() % 2 == 0 );
var parity = Signal.Computed(() => isEven.Get() ? "Even" : "Odd" );


// Example 2 - The dependencies of toPay mutate depending on the value of Quantity
var quantity = new ReadWriteSignal<int>(1);
var basePrice = new ReadWriteSignal<int>(60);
var discountedPrice = new ReadWriteSignal<int>(20);

var toPay = new ComputedSignal<int>(() => quantity.Get() < 10 ? 
                                        quantity.Get() * basePrice.Get() : 
                                        quantity.Get() * discountedPrice.Get());



// Example 3 - Data with custom equality
var data = Signal.State<List<People>>(
    [new People("David", 48), new People("Rebecca", 47), new People("Esther", 15)]  ); 
var adults = Signal.Computed(() => data.Get().Where(p => p.Age > 18).ToList(), CompareOrderedLists);
var numberOfAdults = Signal.Computed(() => adults.Get().Count );
numberOfAdults.AddEffect((oldCount, newCount) => 
    Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));



return;

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

record People(string Name, int Age);
