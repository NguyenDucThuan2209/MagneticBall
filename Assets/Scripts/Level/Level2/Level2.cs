using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Level2 : Level
{
    [Header("Level Settings")]
    [SerializeField] protected GameObject UIhand;
    protected Animator HandAnimator;    
    protected bool canDrag;
    protected bool isWaiting;
    protected Vector3 delta;
    protected Vector3 prevPos;
    [SerializeField] protected Ease ease;
    [SerializeField] protected GameObject[] subChains;
    [SerializeField] protected Vector3[] chainsPosition;
    [SerializeField] protected int chainCount;
    [SerializeField] protected int chainLimit;
    [SerializeField] protected CarpetingBehavior carpetingBehavior;    

    private void Awake()
    {
        HandAnimator = UIhand.GetComponent<Animator>();
    }

    protected override void EndEvent()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDrag()
    {
        throw new System.NotImplementedException();
    }

    protected override void StartEvent()
    {
        throw new System.NotImplementedException();
    }
}
