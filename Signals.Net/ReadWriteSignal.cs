namespace Signals.Net;

public class ReadWriteSignal<T> : BaseSignal<T>
{
    public ReadWriteSignal(T initialValue)
    {
        Value = initialValue;
        IsSuspect = false;
    }
    public void Set(T value)
    {
        var changed = !Comparer(Value, value);
        
        if (changed)
        {
            // Top level signal has changed, so we need to mark all children as suspect
            (this as ISignal).MarkAsSuspect();      // Perf: Cast

            // Update our value and bump the version
            var oldValue = Value;
            Value = value;
            IsSuspect = false;
            IncrementVersion();

            if (Effects is not null)
            {
                foreach (var effect in Effects)
                {
                    effect.TheAction(oldValue, value);
                }
            }

            if (Children is not null)
            {
                foreach (var child in Children.ToArray())  // Perf: Allocation
                    child.FireEffects();
            }
        }
    }
   
    public override T Get()
    {
        // Top level signal, so we know it's always correct
        SignalDependencies.RecordDependency(this);
        return Value;
    }
    
    internal ReadWriteSignal<T> UsingEquality(Func<T, T, bool> comparer)
    {
        Comparer = comparer;
        return this;
    }
}