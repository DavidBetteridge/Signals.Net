namespace Signals.Net.Tests;

[Collection("All")]
public class ClassicExamples
{
    [Fact]
    public void ParityFromIsEvenFromCounter()
    {
        var counter = Signal.State(2);
        var isEven = Signal.Computed(() => counter.Get() % 2 == 0 );

        var parityComputed = false;
        var parity = Signal.Computed(() =>
        {
            parityComputed = true;
            return isEven.Get() ? "Even" : "Odd";
        });
        
        Assert.Equal("Even", parity.Get());
        parityComputed = false;
        
        counter.Set(104);
        Assert.Equal("Even", parity.Get());
        Assert.False(parityComputed);
        
        counter.Set(106);
        Assert.Equal("Even", parity.Get());
        Assert.False(parityComputed);
        
        counter.Set(107);
        Assert.Equal("Odd", parity.Get());
        Assert.True(parityComputed);
    }

    [Fact]
    public void Fullname()
    {
        var firstname = Signal.State("David");
        var lastname = Signal.State("Betteridge");
        var fullname = Signal.Computed(() => $"{firstname.Get()} {lastname.Get()}");

        Assert.Equal("David Betteridge", fullname.Get());
    }
    
    [Theory]
    [InlineData(5, 500)]
    [InlineData(11, 550)]
    public void Basket(int quantityInBasket, int expectedTotal)
    {
        var quantity = Signal.State(1);
        var fullPrice = Signal.State(100);
        var discount = Signal.State(50);
        var toPay = Signal.Computed(() =>  quantity.Get() < 10 ? 
                                                    (quantity.Get() * fullPrice.Get()) : 
                                                    (quantity.Get() * discount.Get()));

        quantity.Set(quantityInBasket);
        Assert.Equal(expectedTotal, toPay.Get());
    }

    [Fact]
    public void Adults()
    {
        var data = Signal.State<List<People>>([new People("David", 48)]); 

        var adults = Signal.Computed(() => data.Get().Where(p => p.Age > 18).ToList(), CompareOrderedLists);

        var computed = false;
        var numberOfAdults = Signal.Computed(() =>
        {
            computed = true;
            return adults.Get().Count;
        });

        numberOfAdults.AddEffect((oldCount, newCount) => 
            Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));

        // Doesn't compute
        computed = false;
        data.Set([new People("David", 48), new People("Mary", 12)]);
        Assert.False(computed);
        
        // Does compute
        data.Set([new People("David", 48), new People("Mary", 12), new People("Rebecca", 48)]);
        Assert.True(computed);
    }
    record People(string Name, int Age);
    
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
}