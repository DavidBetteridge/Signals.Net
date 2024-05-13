namespace Signals.Net.Tests;

public class ReadWriteSignals
{
    [Fact]
    public void ASignalCanBeInitialisedAndRead()
    {
        var s = Signal.State(123);
        Assert.Equal(123, s.Get());
    }
    
    [Fact]
    public void ASignalCanBeWrittenToAndRead()
    {
        var s = Signal.State(123);
        s.Set(456);
        Assert.Equal(456, s.Get());
    }
    
    [Fact]
    public void EffectsFireWhenASignalIsChanged()
    {
        var s = Signal.State(123);
        
        int? previous = null;
        int? current = null;
        
        s.AddEffect((p, c) =>
        {
            previous = p;
            current = c;
        });
        s.Set(456);
        
        Assert.Equal(123, previous);
        Assert.Equal(456, current);
    }
    
    [Fact]
    public void EffectsDoNotFireWhenASignalIsNotChanged()
    {
        var s = Signal.State(123);
        
        int? previous = null;
        int? current = null;
        
        s.AddEffect((p, c) =>
        {
            previous = p;
            current = c;
        });
        s.Set(123);
        
        Assert.Null(previous);
        Assert.Null(current);
    }
    
    [Fact]
    public void EffectsFireWhenASignalWithCustomEqualityIsChanged()
    {
        var r = new R
        {
            S = "ABC", 
            Dt = new DateTime(2001, 1, 1)
        };
        
        var s = Signal.State(r).UsingEquality((a,b) => a.S == b.S);
        
        R? previous = null;
        R? current = null;
        
        s.AddEffect((p, c) =>
        {
            previous = p;
            current = c;
        });
        
        // Change just S
        var r2 = new R
        {
            S = "DEF", 
            Dt = new DateTime(2001, 1, 1)
        };
        s.Set(r2);
        
        Assert.Equal("ABC", previous!.S);
        Assert.Equal("DEF", current!.S);
    }
    
    [Fact]
    public void EffectsDoNotFireWhenASignalWithCustomEqualityIsUnchanged()
    {
        var r = new R
        {
            S = "ABC", 
            Dt = new DateTime(2001, 1, 1)
        };

        var s = Signal.State(initialValue: r, 
                             equalityComparer: (a, b) => a.S == b.S);
        
        R? previous = null;
        R? current = null;
        
        s.AddEffect((p, c) =>
        {
            previous = p;
            current = c;
        });
        
        // Change just Dt
        var r2 = new R
        {
            S = "ABC", 
            Dt = new DateTime(2002, 2, 2)
        };
        s.Set(r2);
        
        Assert.Null(previous);
        Assert.Null(current);
    }
    
    // Add multiple effects
    // Remove effect

    private record R
    {
        public required string S { get; init; }
        public required DateTime Dt { get; init; }
    }
}
