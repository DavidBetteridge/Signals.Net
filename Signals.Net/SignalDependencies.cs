namespace Signals.Net;

/// <summary>
/// Global class used to track dependencies
/// </summary>
internal static class SignalDependencies
{
    private static readonly Stack<IComputeSignal> Tracking = new();
    
    public static void StartTracking(IComputeSignal signal)
    {
        Tracking.Push(signal);
    }
    
    public static void StopTracking()
    {
        Tracking.Pop();
    }

    public static void RecordDependency(ISignal gotSignal)
    {
        if (Tracking.TryPeek(out var signalBeingCalculated))
        {
            if (signalBeingCalculated.AddParent(gotSignal))
                gotSignal.AddChild(signalBeingCalculated);
        }
    }
}
