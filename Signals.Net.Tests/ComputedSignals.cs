namespace Signals.Net.Tests;

[Collection("All")]
public class ComputedSignals
{
    [Fact]
    public void ASignalCanBeDefinedAndRead()
    {
        var s1 = Signal.State(123);
        var s2 = Signal.Computed(() => s1.Get() + 1 );

        Assert.Equal(124, s2.Get());
    }
    
    [Fact]
    public void ASignalCanBeRecomputed()
    {
        var s1 = Signal.State(123);
        var s2 = Signal.Computed(() => s1.Get() + 1 );

        if (s2.Get() != 124) throw new Exception("s2 should be 124");
        
        s1.Set(100);
        
        Assert.Equal(101, s2.Get());
    }
    
    [Fact]
    public void ASignalIsOnlyComputedWhenPulled()
    {
        var s1 = Signal.State(123);

        var computed = false;
        var s2 = Signal.Computed(() =>
        {
            computed = true;
            return s1.Get() + 1;
        });
        computed = false;
        
        s1.Set(100);
        
        Assert.False(computed);
    }
    
    // TODO: Merge ComputedSignal and BaseComputedSignal
    
    
    // Effects
    // Equality
    // No Recompute etc
    // if statements
}