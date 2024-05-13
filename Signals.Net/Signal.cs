namespace Signals.Net;

public static class Signal
{ 
    public static ReadWriteSignal<T> State<T>(T initialValue)
    {
        return new ReadWriteSignal<T>(initialValue);
    }
    
    public static ReadWriteSignal<T> State<T>(T initialValue, Func<T, T, bool> equalityComparer)
    {
        return new ReadWriteSignal<T>(initialValue).UsingEquality(equalityComparer);
    }
}