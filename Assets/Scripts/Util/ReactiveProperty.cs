using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReactiveProperty<T>
{
    private T value;
    private GenericEvent<T> stream;

    private GenericEvent<T> Stream
    {
        get
        {
            if (stream == null)
                stream = new GenericEvent<T>();
            return stream;
        }
    }

    public T Value
    {
        get { return value; }
        set
        {
            Stream.Invoke(value);
            this.value = value;
        }
    }

    public void AddListener(UnityAction<T> action)
    {
        Stream.AddListener(action);
    }

    public void RemoveListener(UnityAction<T> action)
    {
        Stream.RemoveListener(action);
    }

    public void RemoveAllListeners()
    {
        Stream.RemoveAllListeners();
    } 
}

public class GenericEvent<T> : UnityEvent<T> {}