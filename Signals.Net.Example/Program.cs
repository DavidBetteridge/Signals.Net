using Signals.Net;

// var quantity = new ReadWriteSignal<int>(1);
// var basePrice = new ReadWriteSignal<int>(60);
// var discountedPrice = new ReadWriteSignal<int>(20);
//
// var toPay = new ComputedSignal<int>(() => quantity.Get() < 10 ? 
//                                         quantity.Get() * basePrice.Get() : 
//                                         quantity.Get() * discountedPrice.Get());
//
// Console.WriteLine(toPay.Get());
// Console.ReadKey();
//
// discountedPrice.Set(10);
// Console.ReadKey();
//
// Console.WriteLine(toPay.Get());
// Console.ReadKey();
//
// quantity.Set(11);
// Console.ReadKey();
//
// Console.WriteLine(toPay.Get());
// Console.ReadKey();

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


var data = Signal.State<List<People>>([new People("David", 48)]); 

var adults = Signal.Computed(() => data.Get().Where(p => p.Age > 18).ToList(), CompareOrderedLists);

var numberOfAdults = Signal.Computed(() =>
{
    Console.WriteLine("Counting adults");
    return adults.Get().Count;
});

numberOfAdults.AddEffect((oldCount, newCount) => 
    Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));

data.Set([new People("David", 48), new People("Mary", 12)]);

Console.ReadKey();

data.Set([new People("David", 48), new People("Mary", 12), new People("Rebecca", 48)]);

record People(string Name, int Age);