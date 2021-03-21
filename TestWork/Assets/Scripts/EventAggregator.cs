using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


internal sealed class EventAggregator : MonoBehaviour
{
    public event EventHandler<TreeEventArgs> NewTree;
    public event EventHandler<TreeEventArgs> DestroyTree;
    public event EventHandler<LogEventArgs> NewLog;
    public event Action FelledTree;
    public event Action PickLog;
    public event Action UnloadLog;
    public event Action Chop;

    public static EventAggregator instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }
    public void OnChop()
    {
        Chop?.Invoke();
    }
    public void OnUnloadLog()
    {
        UnloadLog?.Invoke();
    }
    public void OnPickLog()
    {
        PickLog?.Invoke();
    }
    public void OnFelledTree()
    {
        FelledTree?.Invoke();
    }
    public void OnNewLog(GameObject log)
    {
        var newLog = new LogEventArgs { Logs = log };
        NewLog?.Invoke(this, newLog);
    }
    public void OnNewTree(GameObject tree)
    {
        var newTree = new TreeEventArgs { Tree = tree };
        NewTree?.Invoke(this, newTree);
    }
    public void OnDestroyTree(GameObject tree, bool isChoping)
    {
        var destroyedTree = new TreeEventArgs { Tree = tree, IsChoping = isChoping};
        DestroyTree?.Invoke(this, destroyedTree);
    }
}

internal sealed class TreeEventArgs : EventArgs
{
    public GameObject Tree { get; set; }
    public bool IsChoping { get; set; }
}
internal sealed class LogEventArgs : EventArgs
{
    public GameObject Logs { get; set; }
}
