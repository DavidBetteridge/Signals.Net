namespace Signals.Net;

/// <summary>
/// Global class used to track dependencies
/// </summary>
internal class SignalDependencies
{
    private static SignalDependencies? _instance;
    public static SignalDependencies Instance => _instance ??= new SignalDependencies();

    private static readonly Stack<IComputeSignal> Tracking = new();
    
    public void StartTracking(IComputeSignal signal)
    {
        Tracking.Push(signal);
    }
    
    public void StopTracking()
    {
        Tracking.Pop();
    }

    public void RecordDependency(ISignal gotSignal)
    {
        if (Tracking.TryPeek(out var signalBeingCalculated))
        {
            if (signalBeingCalculated.AddParent(gotSignal));
                gotSignal.AddChild(signalBeingCalculated);
        }
    }
}
