using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class TreeLogic : MonoBehaviour
{
    [SerializeField] private GameObject log;
    [SerializeField] private Transform tree;
    [SerializeField] private ParticleSystem particle;

    private bool isChoping;

    private void Start()
    {
        StartCoroutine(WaitToAdd());
        isChoping = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "People")
        {
            isChoping = true;
            StartCoroutine(FalseChoping());
        }
    }

    private void OnFelledTree()
    {
        if (isChoping)
        {
            EventAggregator.instance.OnDestroyTree(gameObject, isChoping);
            var position = new Vector3(tree.position.x, tree.position.y + 0.1f, tree.position.z);
            Instantiate(log, position, Quaternion.Euler(0,90,0));
            EventAggregator.instance.FelledTree -= OnFelledTree;
            EventAggregator.instance.Chop -= OnChop;
            Destroy(gameObject);
        }
    }
    private void OnChop()
    {
        if (isChoping)
        {
            particle.Play();
        }
    }

    private IEnumerator WaitToAdd()
    {
        yield return new WaitForSeconds(0.01f);
        EventAggregator.instance.OnNewTree(gameObject);
        EventAggregator.instance.FelledTree += OnFelledTree;
        EventAggregator.instance.Chop += OnChop;
    }
    private IEnumerator FalseChoping()
    {
        yield return new WaitForSeconds(5f);
        isChoping = false;
    }
    private void OnDestroy()
    {
        EventAggregator.instance.FelledTree -= OnFelledTree;
        EventAggregator.instance.Chop -= OnChop;
        EventAggregator.instance.OnDestroyTree(gameObject, isChoping);
    }
}
