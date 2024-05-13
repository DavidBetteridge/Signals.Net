namespace Signals.Net.Tests;

[Collection("All")]
public class ClassicExample
{
    [Fact]
    public void ParityFromCounter()
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
}