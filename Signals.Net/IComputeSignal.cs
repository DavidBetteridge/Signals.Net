namespace Signals.Net;

public interface IComputeSignal : ISignal
{
    void EnsureNodeIsComputed();
    
    void FireEffects();
    void AddParent(ISignal gotSignal);
}
