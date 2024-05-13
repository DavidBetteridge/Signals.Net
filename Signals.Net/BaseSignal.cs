namespace Signals.Net;


public abstract class BaseSignal<T> : ISignal
{
    // Who depends on this signal
    protected readonly List<IComputeSignal> Children = [];

    // Value the last time this signal was calculated
    protected T Value = default!;
    
    // How many times has the value of this signal changed
    int ISignal.Version { get; set; }
    
    void ISignal.RemoveChild(IComputeSignal child)
    {
        Children.Remove(child);
    }

    protected void IncrementVersion()
    {
        (this as ISignal).Version++;
    }

    // We are suspect when a node somewhere above us in the graph has changed
    protected bool IsSuspect = true;
  
    // Optional method to call when the value of this signal changes
    protected readonly List<Effect<T>> Effects = [];
    protected Func<T, T, bool>? Comparer;

    public abstract T Get();

    public Effect<T> AddEffect(Action<T, T> effect)
    {
        var e = new Effect<T>(effect);
        Effects.Add(e);
        return e;
    }

    public bool RemoveEffect(Effect<T> effectToRemove)
    {
        return Effects.Remove(effectToRemove);
    }
    
    void ISignal.AddChild(IComputeSignal signal)
    {
        Children.Add(signal);
    }

    void ISignal.MarkAsSuspect()
    {
        if (!IsSuspect)
        {
            IsSuspect = true;
            foreach (var child in Children)
                child.MarkAsSuspect();
        }
    }
}
