namespace Signals.Net;

public class Computed<T> : ComputedSignal<T>
{
    private readonly Func<T> _expression;

    public Computed(Func<T> expression)
    {
        _expression = expression;
        Calculate();
    }

    private void Calculate()
    {
        SignalDependencies.Instance.StartTracking(this);
        Value = _expression();
        SignalDependencies.Instance.StopTracking();
    }
    protected override void Compute()
    {
        RemoveAllDependencies();
        Calculate();
    }
}