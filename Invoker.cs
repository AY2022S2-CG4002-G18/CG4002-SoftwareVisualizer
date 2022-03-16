using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoker : MonoBehaviour
{
    static Invoker _instance;

    public List<System.Action> delegates = new List<System.Action>();

    public static void InvokeInMainThread(System.Action _delegate)
    {
        _instance.delegates.Add(_delegate);
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        Execute();
    }

    void Execute()
    {
        if (delegates.Count == 0) return;

        for (int i = 0; i < delegates.Count; i++)
        {
            delegates[i]();
        }

        delegates.Clear();
    }
}
