namespace Signals.Net;

public interface ISignal
{
    public void MarkAsSuspect();
    
    public int Version { get; set; }
    
    void RemoveChild(IComputeSignal child);
    void AddChild(IComputeSignal signal);
}