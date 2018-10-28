using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReactiveProperty<T>
{
    private T value;
    private GenericEvent<T> stream;

//    public ReactiveProperty(T value, GenericEvent<T> stream)
//    {
//        this.value = (T) new System.Object();
//        this.stream = stream;
//    }

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
            if (value.Equals(this.value))
                return;
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