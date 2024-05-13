namespace Signals.Net;

public abstract class ComputedSignal<T> : BaseSignal<T>, IComputeSignal
{
    protected class SignalWithVersion(ISignal signal, int version)
    {
        public int Version { get; set; } = version;
        public ISignal Signal { get; } = signal;
    }

    // Who do we depend on?
    protected readonly List<SignalWithVersion> Parents = [];
    
    public void AddParent(ISignal signal)
    {
        Parents.Add(new SignalWithVersion(signal, -1));
    }

    protected void RemoveAllDependencies()
    {
        foreach (var parent in Parents)
        {
            parent.Signal.RemoveChild(this);
        }
        Parents.Clear();
    }
    
    protected abstract void Compute();
    
    public override T Get()
    {
        SignalDependencies.Instance.RecordDependency(this);
        EnsureNodeIsComputed();
        return Value;
    }
    
    public void FireEffects()
    {
        if (Effect != null)
        {
            var oldValue = Value;
            EnsureNodeIsComputed();
            
            var changed = Comparer is null ? (Value is null || !Value.Equals(oldValue)) : !Comparer(Value, oldValue);
            if (changed)
            {
                Effect(oldValue, Value);
            }
            else
            {
                // We didn't change,  so don't need to examine our children
                return;
            }
        }
        // We have changed,  so maybe our children have as well
        foreach (var child in Children)
        {
            child.FireEffects();
        }
    }
    
    public void EnsureNodeIsComputed()
    {
        if (!IsSuspect) return;  // All good
        
        // Make sure our parents are good
        foreach (var parent in Parents)
        {
            if (parent.Signal is IComputeSignal computeSignal)
            {
                computeSignal.EnsureNodeIsComputed();
            }
        }

        //    SOLUTION - Track the version of each parent to determine if it has changed
        
        // If any of our parents have changed,  then we need to update
        var updateNeeded = false;
        foreach (var parent in Parents)
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
                Version++;
        }
        
        IsSuspect = false;
    }
    
    public BaseSignal<T> UsingEquality(Func<T, T, bool> comparer)
    {
        Comparer = comparer;
        return this;
    }
}