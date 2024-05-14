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
        var changed = Comparer is null ? (Value is null || !Value.Equals(value)) : !Comparer(Value, value);
        
        if (changed)
        {
            // Top level signal has changed, so we need to mark all children as suspect
            (this as ISignal).MarkAsSuspect();

            // Update our value and bump the version
            var oldValue = Value;
            Value = value;
            IsSuspect = false;
            IncrementVersion();

            foreach (var effect in Effects)
            {
                effect.TheAction(oldValue, value);    
            }
            
            foreach (var child in Children.ToArray())
                child.FireEffects();
        }
    }
   
    public override T Get()
    {
        // Top level signal, so we know it's always correct
        SignalDependencies.Instance.RecordDependency(this);
        return Value;
    }
    
    public ReadWriteSignal<T> UsingEquality(Func<T, T, bool> comparer)
    {
        Comparer = comparer;
        return this;
    }
}