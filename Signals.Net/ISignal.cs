namespace Signals.Net;

public interface ISignal
{
    public void MarkAsSuspect();
    
    public int Version { get; }
    
    void RemoveChild(IComputeSignal child);
    void AddChild(IComputeSignal signal);
}