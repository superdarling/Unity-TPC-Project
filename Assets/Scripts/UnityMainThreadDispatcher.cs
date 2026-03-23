using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> queue = new Queue<Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        if (!instance)
        {
            instance = new GameObject("Dispatcher")
                .AddComponent<UnityMainThreadDispatcher>();
        }
        return instance;
    }

    private static UnityMainThreadDispatcher instance;

    public void Enqueue(Action action)
    {
        lock (queue)
        {
            queue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (queue)
        {
            while (queue.Count > 0)
            {
                queue.Dequeue().Invoke();
            }
        }
    }
}