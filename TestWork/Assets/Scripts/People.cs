using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal sealed class People : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform housePoint;
    [SerializeField] private List<GameObject> poolTrees;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] backpack;
    [SerializeField] private GameObject logs;

    private Transform target;
    private GameObject closestItem;
    private GameObject curentTree;
    private bool isBusy;
    private bool isGoingToTree;
    private int lastCountOfTree;

    private void Start()
    {
        foreach (var log in backpack)
        {
            log.SetActive(false);
        }
        SubscribeOnEvents();
        StartCoroutine(OnStartGame());
    }
    private void FixedUpdate()
    {
        if(isGoingToTree && (poolTrees.Count == 0))
        {
            isGoingToTree = false;
            agent.SetDestination(startPoint.position);
            isBusy = false;
        }
        if ((poolTrees.Count != 0) && (lastCountOfTree == 0) && !isBusy && !backpack[0].activeSelf)
        {
            isBusy = true;
            PrepareToWalk();
            GoToTheTree();
        }
        if(curentTree != FindClosestTree())
        {
            if((poolTrees.Count != 0) && (lastCountOfTree == 0) && (!backpack[0].activeSelf))
            {
                GoToTheTree();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.tag == "StartPosition") && (poolTrees.Count == 0) && (!backpack[0].activeSelf))//
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            animator.SetBool("isWalking", false);
            agent.isStopped = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tree" && isBusy)
        {
            isGoingToTree = false;
            curentTree = collision.gameObject;
            PrepareToAction(collision);
            animator.SetTrigger("Chop");
        }
        if (collision.gameObject.tag == "Log")
        {
            if (!backpack[0].activeSelf)
            {
                PrepareToAction(collision);
                animator.SetTrigger("PickLog");
            }
        }
        if(collision.gameObject.tag == "House")
        {
            PrepareToAction(collision);
            animator.SetTrigger("Unload");
        }
    }

    private void PrepareToAction(Collision coll)
    {
        animator.SetBool("isWalking", false);
        agent.isStopped = true;
        gameObject.transform.LookAt(coll.transform);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    private void PrepareToWalk()
    {
        rb.constraints = RigidbodyConstraints.None;
        agent.isStopped = false;
        animator.SetBool("isWalking", true);
    }
    private void GoToTheTree()
    {
        isGoingToTree = true;
        if (FindClosestTree() == null)
        {
            agent.SetDestination(startPoint.position);
        }
        else
        {
            target = FindClosestTree().transform;
            agent.SetDestination(target.position);
        }
    }
    private GameObject FindClosestTree()
    {
        try
        {
            float distance = Mathf.Infinity;
            foreach (var tree in poolTrees)
            {
                var curDistance = (tree.transform.position - transform.position).sqrMagnitude;
                if (curDistance < distance)
                {
                    closestItem = tree;
                    distance = curDistance;
                }
            }
            return closestItem;
        }
        catch(Exception)
        {
            return null;
        }
        // It`s can throw Exeption if you try to get out prefab "Tree" from Scene in Runtime
    }

    #region UsedInEvents
    private void OnNewTree(object sender, TreeEventArgs e)
    {
        poolTrees.Add(e.Tree);
    }
    private void OnNewLog(object sender, LogEventArgs e)
    {
        logs = e.Logs;
    }
    private void OnDestroyTree(object sender, TreeEventArgs e)
    {
        poolTrees.Remove(e.Tree);
        if (e.IsChoping)
        {
            StartCoroutine(TimeToFindLog(0f));
        }
    }
    #endregion

    #region UsedInAnimation
    private void Chop()
    {
        EventAggregator.instance.OnChop();
    }
    private void FelledTree()
    {
        lastCountOfTree = poolTrees.Count;
        EventAggregator.instance.OnFelledTree();
    }
    private void PickLog()
    {
        EventAggregator.instance.OnPickLog();
    }
    private void FillBackpack()
    {
        foreach (var log in backpack)
        {
            log.SetActive(true);
        }
    }
    private void EndPickingLog()
    {
        isBusy = false;
        lastCountOfTree = poolTrees.Count;
        PrepareToWalk();
        agent.SetDestination(housePoint.position);
    }
    private void UnloadLog()
    {
        foreach (var log in backpack)
        {
            log.SetActive(false);
        }
        EventAggregator.instance.OnUnloadLog();
    }
    private void EndUnload()
    {
        PrepareToWalk();
        if (poolTrees.Count == 0)
        {
            agent.SetDestination(startPoint.position);
        }
        else
        {
            isBusy = true;
            GoToTheTree();
        }
    }
    #endregion

    private IEnumerator OnStartGame()
    {
        yield return new WaitForSeconds(0.01f);
        isBusy = true;
        lastCountOfTree = poolTrees.Count;
        animator.SetBool("isWalking", true);
        GoToTheTree();
    }
    private IEnumerator TimeToFindLog(float time)
    {
        yield return new WaitForSeconds(time);
        target = logs.transform;
        PrepareToWalk();
        agent.SetDestination(target.position);
    }

    private void SubscribeOnEvents()
    {
        EventAggregator.instance.DestroyTree += OnDestroyTree; ;
        EventAggregator.instance.NewTree += OnNewTree;
        EventAggregator.instance.NewLog += OnNewLog;
    }
    private void UnsubscribeOnEvents()
    {
        EventAggregator.instance.DestroyTree -= OnDestroyTree; ;
        EventAggregator.instance.NewTree -= OnNewTree;
        EventAggregator.instance.NewLog -= OnNewLog;
    }

    private void OnDestroy()
    {
        UnsubscribeOnEvents();
    }
}
