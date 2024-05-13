namespace Signals.Net;

public class Effect<T>(Action<T, T> effect)
{
    internal Action<T, T> TheAction { get; } = effect;
}
