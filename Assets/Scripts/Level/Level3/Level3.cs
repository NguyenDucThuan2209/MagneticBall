using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Level3 : Level
{
    [Header("Level Properties")]
    [SerializeField] protected GameObject UIhand;
    [SerializeField] protected Animator HandAnimator;

    protected Ease ease;

    protected bool canDrag;
    protected bool isWaiting;

    protected Vector3 delta;
    protected Vector3 prevPos;

    protected CarpetingBehavior carpetingBehavior;

    private void Awake()
    {
        if (UIhand != null)
        {
            HandAnimator = UIhand.GetComponent<Animator>();
        }
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
