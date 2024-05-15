namespace Signals.Net;

public interface ISignal
{
    public void MarkAsSuspect();
    
    public uint Version { get; set; }
    
    void RemoveChild(IComputeSignal child);
    void AddChild(IComputeSignal signal);
}