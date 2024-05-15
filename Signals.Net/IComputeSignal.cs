namespace Signals.Net;

public interface IComputeSignal : ISignal
{
    void EnsureNodeIsComputed();
    
    void FireEffects();
    bool AddParent(ISignal gotSignal);
    void Delete();
}
