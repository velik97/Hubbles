using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Gives opportunity to call methods parallel with calling callbacks after completion
/// </summary>
public class ParallelTaskManager : MonoSingleton<ParallelTaskManager>
{
    private Queue<Action> parallelRequestQueue;
    private Queue<Exception> parallelExceptionQueue;
    private static readonly object parallelTaskLock = new object();

    private void Awake ()
    {
        parallelRequestQueue = new Queue<Action>();
        parallelExceptionQueue = new Queue<Exception>();
    }

    private void Update()
    {
        lock (parallelTaskLock)
        {
            while (parallelRequestQueue.Count > 0) 
            {
                parallelRequestQueue.Dequeue().Invoke();
            }
            while (parallelExceptionQueue.Count > 0) 
            {
                Debug.LogError("[Parallel Thread] " + parallelExceptionQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// Calls <paramref name="paralellTask"/> in a parallel thread, than calls <paramref name="callback"></paramref>
    /// in the main thread with result of <paramref name="paralellTask"/> as an argument
    /// </summary>
    /// <param name="paralellTask">to be called in a parallel thread</param>
    /// <param name="callback">to be called after <paramref name="paralellTask"/> is complete</param>
    /// <typeparam name="T">type, that <paramref name="paralellTask"/> returns</typeparam>
    /// <exception cref="ThreadStartException">В <paramref name="paralellTask"/>it's not possible to call UnityEngine methods inside of a <paramref name="paralellTask"/></exception>
    public void CallFuncParallel<T>(Func<T> paralellTask, Action<T> callback)
    {
        ThreadStart threadStart = new ThreadStart(delegate
        {
            try
            {
                T receiveData = paralellTask();
                lock (parallelTaskLock)
                {
                    parallelRequestQueue.Enqueue(delegate
                    {
                        callback.Invoke(receiveData);
                    });
                }
            }
            catch (Exception e)
            {
                lock (parallelTaskLock)
                {
                    parallelExceptionQueue.Enqueue(e);
                }
            }
        });
        
        new Thread(threadStart).Start();
    }
    
    /// <summary>
    /// Calls <paramref name="parallelTask"/> in a parallel thread, than calls <paramref name="callback"></paramref>
    /// in the main thread
    /// </summary>
    /// <param name="parallelTask">to be called in a parallel thread</param>
    /// <param name="callback">to be called after <paramref name="parallelTask"/> is complete</param>
    /// <exception cref="ThreadStartException">It's not possible to call UnityEngine methods inside of a <paramref name="parallelTask"/></exception>
    public void CallFuncParallel(Action parallelTask, Action callback)
    {
        ThreadStart threadStart = new ThreadStart(delegate
        {
            try
            {
                parallelTask.Invoke();
                lock (parallelTaskLock)
                {
                    parallelRequestQueue.Enqueue(callback.Invoke);
                }
            }
            catch (Exception e)
            {
                lock (parallelTaskLock)
                {
                    parallelExceptionQueue.Enqueue(e);
                }
            }
        });
        
        new Thread(threadStart).Start();
    }
}

