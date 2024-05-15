namespace Signals.Net;

public class ComputedSignal<T> : BaseSignal<T>, IComputeSignal
{
    private readonly Func<T> _expression;

    public ComputedSignal(Func<T> expression)
    {
        _expression = expression;
        Calculate();
    }
    
    // Who do we depend on?
    private readonly Dictionary<ISignal, uint> _parents = [];
    
    bool IComputeSignal.AddParent(ISignal signal)
    {
        if (!_parents.ContainsKey(signal))
        {
            _parents[signal] = signal.Version;
            return true;
        }

        return false;
    }

    private void RemoveAllDependencies()
    {
        foreach (var parent in _parents)
        {
            parent.Key.RemoveChild(this);
        }
        _parents.Clear();
    }
    
    private void Calculate()
    {
        SignalDependencies.StartTracking(this);
        Value = _expression();
        SignalDependencies.StopTracking();
    }

    private void Compute()
    {
        RemoveAllDependencies();        // Perf: In a lot of cases we will end up re-adding the same
        Calculate();                    // dependencies.  It might be better to add a dead/alive flag.
    }
    
    public override T Get()
    {
        SignalDependencies.RecordDependency(this);
        (this as IComputeSignal).EnsureNodeIsComputed();        // Perf: Cast
        return Value;
    }
    
    void IComputeSignal.FireEffects()
    {
        if (Effects is not null && Effects.Count > 0)
        {
            var oldValue = Value;
            (this as IComputeSignal).EnsureNodeIsComputed();        // Perf: Cast
            
            var changed = !Comparer(Value, oldValue);
            if (changed)
            {
                foreach (var effect in Effects)
                    effect.TheAction(oldValue, Value);
            }
            else
            {
                // We didn't change,  so don't need to examine our children
                return;
            }
        }
        // We have changed,  so maybe our children have as well
        if (Children is not null)
        {
            foreach (var child in Children.ToArray())           // Perf: Allocation
            {
                child.FireEffects();
            }
        }
    }
    
    void IComputeSignal.EnsureNodeIsComputed()
    {
        if (!IsSuspect) return;  // All good
        
        // Make sure our parents are good
        foreach (var parent in _parents)
        {
            if (parent.Key is IComputeSignal computeSignal)
            {
                computeSignal.EnsureNodeIsComputed();
            }
        }
        
        // If any of our parents have changed,  then we need to update
        var updateNeeded = false;
        foreach (var parent in _parents)
        {
            if (parent.Value != parent.Key.Version)
            {
                updateNeeded = true;
                _parents[parent.Key] = parent.Key.Version;
            }
        }

        if (updateNeeded)
        {
            var oldValue = Value;
            
            Compute();
            var changed = !Comparer(Value, oldValue);
            if (changed)
                IncrementVersion();
        }
        
        IsSuspect = false;
    }
    
    internal ComputedSignal<T> UsingEquality(Func<T, T, bool> comparer)
    {
        Comparer = comparer;
        return this;
    }

    public void Delete()
    {
        RemoveAllDependencies();
        if (Children is not null)
        {
            foreach (var child in Children.ToArray())
                child.Delete();
        }
    }
}