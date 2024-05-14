namespace Signals.Net;

public class ComputedSignal<T> : BaseSignal<T>, IComputeSignal
{
    private readonly Func<T> _expression;

    public ComputedSignal(Func<T> expression)
    {
        _expression = expression;
        Calculate();
    }
    
    private sealed class SignalWithVersion(ISignal signal, int version)
    {
        public int Version { get; set; } = version;
        public ISignal Signal { get; } = signal;
    }

    // Who do we depend on?
    private readonly List<SignalWithVersion> _parents = [];
    
    bool IComputeSignal.AddParent(ISignal signal)
    {
        if (!_parents.Exists(s => s.Signal == signal))
        {
            _parents.Add(new SignalWithVersion(signal, signal.Version));
            return true;
        }

        return false;
    }

    private void RemoveAllDependencies()
    {
        foreach (var parent in _parents)
        {
            parent.Signal.RemoveChild(this);
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
        RemoveAllDependencies();
        Calculate();
    }
    
    public override T Get()
    {
        SignalDependencies.RecordDependency(this);
        (this as IComputeSignal).EnsureNodeIsComputed();
        return Value;
    }
    
    void IComputeSignal.FireEffects()
    {
        if (Effects.Count > 0)
        {
            var oldValue = Value;
            (this as IComputeSignal).EnsureNodeIsComputed();
            
            var changed = Comparer is null ? (Value is null || !Value.Equals(oldValue)) : !Comparer(Value, oldValue);
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
            foreach (var child in Children.ToArray())
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
            if (parent.Signal is IComputeSignal computeSignal)
            {
                computeSignal.EnsureNodeIsComputed();
            }
        }
        
        // If any of our parents have changed,  then we need to update
        var updateNeeded = false;
        foreach (var parent in _parents)
        {
            if (parent.Version != parent.Signal.Version)
            {
                updateNeeded = true;
                parent.Version = parent.Signal.Version;
            }
        }

        if (updateNeeded)
        {
            var oldValue = Value;
            
            Compute();
            var changed = Comparer is null ? (Value is null || !Value.Equals(oldValue)) : !Comparer(Value, oldValue);
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
}