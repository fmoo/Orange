using UnityEngine;
using UniRx;

public class Channel<T> : ScriptableObject {
    public Subject<T> events = new Subject<T>();
    public void emit(T @event) {
        events.OnNext(@event);
    }
}

public class ChannelWithValue<T> : ScriptableObject {
    public BehaviorSubject<T> events = new BehaviorSubject<T>(default(T));
    public T defaultValue;
    public T Value => events.Value;

    void Awake() {
        events.OnNext(defaultValue);
    }

    public void emit(T @event) {
        events.OnNext(@event);
    }
}