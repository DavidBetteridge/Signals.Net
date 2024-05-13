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

    public static ComputedSignal<T> Computed<T>(Func<T> expression)
    {
        return new ComputedSignal<T>(expression);
    }
    
    public static ComputedSignal<T> Computed<T>(Func<T> expression, Func<T, T, bool> equalityComparer)
    {
        var newSignal = new ComputedSignal<T>(expression);
        newSignal.UsingEquality(equalityComparer);
        return newSignal;
    }
}