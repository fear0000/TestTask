using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class LogDecor : MonoBehaviour
{
    [SerializeField] private GameObject[] logs;
    private int FilledLog;
    private void Start()
    {
        FilledLog = 0;
        EventAggregator.instance.UnloadLog += OnUnloadLog;
    }

    private void OnUnloadLog()
    {
        for (int i = 0; i < 3; i++)
        {
            if (FilledLog < logs.Length)
            {
                logs[FilledLog].SetActive(true);
                FilledLog++;
            }
        }
    }
    private void OnDestroy()
    {
        EventAggregator.instance.UnloadLog -= OnUnloadLog;
    }
}
