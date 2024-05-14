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

        CheckSignal(s2, nameof(s2), 124);
        
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
    
    [Fact]
    public void ASignalCanBeComputedFromTwoSignals()
    {
        var s1 = Signal.State(123);
        var s2 = Signal.State(456);
        var s3 = Signal.Computed(() => s1.Get() + s2.Get() );
        
        s1.Set(100);
        
        Assert.Equal(556, s3.Get());
    }
    
    [Fact]
    public void AComputedSignalCanHaveMultipleEffects()
    {
        var s1 = Signal.State(123);
        var s2 = Signal.Computed(() => s1.Get() + 1 );
        
        int? previous1 = null;
        int? current1 = null;
        int? previous2 = null;
        int? current2 = null;
        
        s2.AddEffect((p, c) =>
        {
            previous1 = p;
            current1 = c;
        });
        
        s2.AddEffect((p, c) =>
        {
            previous2 = p;
            current2 = c;
        });
        
        s1.Set(456);
        
        Assert.Equal(124, previous1);
        Assert.Equal(457, current1);
        
        Assert.Equal(124, previous2);
        Assert.Equal(457, current2);
    }

    [Fact]
    public void AComputedSignalCanIgnoreDependenciesNotOnTheCodePath()
    {
        var s1 = Signal.State(1);
        var s2 = Signal.State(2);
        var s3 = Signal.State(3);

        var computed = false;
        var s4 = Signal.Computed(() =>
        {
            computed = true;
            return s1.Get() == 1 ? s2.Get() : s3.Get();
        });
        
        CheckSignal(s4, nameof(s4), 2);
        computed = false;
        
        // s4 is not dependent on s3 and so shouldn't be computed.
        s3.Set(30);
        CheckSignal(s4, nameof(s4), 2);
        
        Assert.False(computed);
    }
    
    [Fact]
    public void AComputedSignalCanChangeItsDependencies()
    {
        var s1 = Signal.State(1);
        var s2 = Signal.State(2);
        var s3 = Signal.State(3);

        var computed = false;
        var s4 = Signal.Computed(() =>
        {
            computed = true;
            return s1.Get() == 1 ? s2.Get() : s3.Get();
        });
        
        CheckSignal(s4, nameof(s4), 2);
        computed = false;
        
        // s4 is not dependent on s3 and so shouldn't be computed.
        s3.Set(30);
        CheckSignal(s4, nameof(s4), 2);
        if (computed) throw new Exception("computed should still be False");
        
        // s4 will now be dependent on s3
        s1.Set(2);
        CheckSignal(s4, nameof(s4), 30);
        computed = false;
        
        s3.Set(130);
        CheckSignal(s4, nameof(s4), 130);
        Assert.True(computed);
    }

    private static void CheckSignal(BaseSignal<int> s4, string signalName, int expectedValue)
    {
        if (s4.Get() != expectedValue)
            throw new Exception($"{signalName} should be {expectedValue} not {s4.Get()}");
    }

    [Fact]
    public void AComputedSignalCanUseCustomEqualityToPreventItTriggering()
    {
        var s1 = Signal.State(1);
        var s2 = Signal.Computed(expression: () => new R(s1.Get(), 10),
                                 equalityComparer: (l,r) => l.B == r.B);  // Only triggers when B changes.

        var triggered = false;
        s2.AddEffect((p, c) =>
        {
            triggered = true;
        });
        
        s1.Set(2);
        Assert.False(triggered);
    }

    [Fact]
    public void AComputedSignalCanUseCustomEqualityToTriggerWhenImportantValuesChange()
    {
        var s1 = Signal.State(1);
        var s2 = Signal.Computed(expression: () => new R(s1.Get(), 10),
            equalityComparer: (l,r) => l.A == r.A);  // Only triggers when A changes.

        var triggered = false;
        s2.AddEffect((p, c) =>
        {
            triggered = true;
        });
        
        s1.Set(2);
        Assert.True(triggered);
    }
    
    private record R(int A, int B);
}