using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Level1 : Level
{
    [Header("Level Settings")]
    [SerializeField] protected Ease ease;
    [SerializeField] protected CarpetingBehavior carpetingBehavior;
    [SerializeField] protected GameObject UIhand;
    protected Animator HandAnimator;
    protected bool canDrag;
    protected bool isWaiting;
    protected Vector3 delta;
    protected Vector3 prevPos;
    [SerializeField] protected int chainCount;
    [SerializeField] protected int chainLimit;
    [SerializeField] protected GameObject[] subChains;
    [SerializeField] protected Vector3[] chainsPosition;
    [SerializeField] protected RectTransform dockUI;
    [SerializeField] protected RectTransform TargetPanel;

    protected override void EndEvent()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDrag()
    {
        throw new System.NotImplementedException();
    }
    private void Awake()
    {
        HandAnimator = UIhand.GetComponent<Animator>();
    }
    private void Start()
    {
        StartEvent();
    }
    protected override void StartEvent()
    {
       
    }
}
