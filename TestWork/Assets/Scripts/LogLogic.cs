using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class LogLogic : MonoBehaviour
{
    [SerializeField] private GameObject[] logs;
    private int numberOfLog;
    private void Start()
    {
        numberOfLog = 0;
        EventAggregator.instance.OnNewLog(gameObject);
        EventAggregator.instance.PickLog += OnPickLog;
    }

    private void OnPickLog()
    {
        logs[numberOfLog].SetActive(false);
        numberOfLog++;
        if(numberOfLog > 2)
        {
            EventAggregator.instance.PickLog -= OnPickLog;
            Destroy(gameObject);
        }
    }
}
