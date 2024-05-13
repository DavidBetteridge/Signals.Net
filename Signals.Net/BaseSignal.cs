namespace Signals.Net;


public abstract class BaseSignal<T> : ISignal
{
    // Who depends on this signal
    protected readonly List<IComputeSignal> Children = [];

    // Value the last time this signal was calculated
    protected T Value = default!;
    
    // How many times has the value of this signal changed
    public int Version { get; set; }
    public void RemoveChild(IComputeSignal child)
    {
        Children.Remove(child);
    }

    // We are suspect when a node somewhere above us in the graph has changed
    protected bool IsSuspect = true;
  
    // Optional method to call when the value of this signal changes
    protected Action<T,T>? Effect;
    protected Func<T, T, bool>? Comparer;

    public abstract T Get();

    public void AddEffect(Action<T, T> effect)
    {
        Effect = effect;
    }
    
    public void AddChild(IComputeSignal signal)
    {
        Children.Add(signal);
    }

    public void MarkAsSuspect()
    {
        if (!IsSuspect)
        {
            IsSuspect = true;
            foreach (var child in Children)
                child.MarkAsSuspect();
        }
    }
}
