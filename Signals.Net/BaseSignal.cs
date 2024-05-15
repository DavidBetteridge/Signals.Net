using System.Diagnostics.CodeAnalysis;

namespace Signals.Net;


[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
public abstract class BaseSignal<T> : ISignal
{
    // Who depends on this signal
    internal List<IComputeSignal>? Children;

    // Value the last time this signal was calculated
    internal T Value = default!;
    
    // How many times has the value of this signal changed
    uint ISignal.Version { get; set; }
    
    void ISignal.RemoveChild(IComputeSignal child)
    {
        Children?.Remove(child);
    }

    protected void IncrementVersion()
    {
        (this as ISignal).Version++;        // Perf: Cast
    }

    // We are suspect when a node somewhere above us in the graph has changed
    internal bool IsSuspect = true;
  
    // Optional methods to call when the value of this signal changes
    internal List<Effect<T>>? Effects;
    
    // Optional method to determine how the old and new values of T should be compared.
    internal Func<T, T, bool> Comparer = EqualityComparer<T>.Default.Equals;

    public abstract T Get();

    public Effect<T> AddEffect(Action<T, T> effect)
    {
        if (Effects is null) Effects = new List<Effect<T>>();
        var e = new Effect<T>(effect);
        Effects.Add(e);
        return e;
    }

    public bool RemoveEffect(Effect<T> effectToRemove)
    {
        return Effects?.Remove(effectToRemove) ?? false;
    }
    
    void ISignal.AddChild(IComputeSignal signal)
    {
        if (Children is null) Children = new List<IComputeSignal>();
        Children.Add(signal);
    }

    void ISignal.MarkAsSuspect()
    {
        if (!IsSuspect)
        {
            IsSuspect = true;
            if (Children is not null)
            {
                foreach (var child in Children)
                    child.MarkAsSuspect();
            }
        }
    }
}
