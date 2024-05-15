namespace Signals.Net.Tests;

[Collection("All")]
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
        
        var s = Signal.State(r, (a,b) => a.S == b.S);
        
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
    
    [Fact]
    public void ASignalCanHaveMultipleEffects()
    {
        var s = Signal.State(123);
        
        int? previous1 = null;
        int? current1 = null;
        int? previous2 = null;
        int? current2 = null;
        
        s.AddEffect((p, c) =>
        {
            previous1 = p;
            current1 = c;
        });
        
        s.AddEffect((p, c) =>
        {
            previous2 = p;
            current2 = c;
        });
        
        s.Set(456);
        
        Assert.Equal(123, previous1);
        Assert.Equal(456, current1);
        
        Assert.Equal(123, previous2);
        Assert.Equal(456, current2);
    }
    
    [Fact]
    public void EffectsCanBeRemovedFromASignal()
    {
        var s = Signal.State(123);
        
        int? previous1 = null;
        int? current1 = null;
        int? previous2 = null;
        int? current2 = null;
        
        var e1 = s.AddEffect((p, c) =>
        {
            previous1 = p;
            current1 = c;
        });
        
        var e2 = s.AddEffect((p, c) =>
        {
            previous2 = p;
            current2 = c;
        });

        s.RemoveEffect(e2);
        
        s.Set(456);
        
        Assert.Equal(123, previous1);
        Assert.Equal(456, current1);
        
        Assert.Null(previous2);
        Assert.Null(current2);
    }
    
    [Fact]
    public void AReadWriteSignalCanBeDeleted()
    {
        var s1 = Signal.State(1);
        var s2 = Signal.State(2);
        var s3 = Signal.Computed(() => s1.Get() + s2.Get() );
        var triggered = false;
        s3.AddEffect((p, c) =>
        {
            triggered = true;
        });
        triggered = false;

        s1.Delete();
        s2.Set(3);
        Assert.False(triggered);
    }

    private record R
    {
        public required string S { get; init; }
        public required DateTime Dt { get; init; }
    }
}

