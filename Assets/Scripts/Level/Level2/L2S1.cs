using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class L2S1 : Level2
{
    [Header("Parts")]
    [SerializeField] Transform Main;
    [SerializeField] Transform B1;
    [SerializeField] Transform B2;
    [SerializeField] Transform A1;

    [Header("UI Properties")]
    [SerializeField] RectTransform TargetPanel;

    float target;
    float smoothTime = 0.3F;
    Vector3 velocity = Vector3.zero;
    bool isStateDone;

    void Start()
    {
        UIhand.SetActive(true);        
        HandAnimator.Play("LeftToRight");
        TargetPanel.DOAnchorPosY(-180, 1);

        canDrag = true;
    }
    private void Update()
    {      
        if(canDrag)
        {
            OnDrag();
        }
    }
    protected override void OnDrag()
    {
        if (isStateDone) return;
        if (Input.GetMouseButtonDown(0))
        {
            prevPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            UIhand.SetActive(false);
            delta.x = (Input.mousePosition - prevPos).x * Time.deltaTime;
            if (delta.x > 0)
            {
                target += delta.x * 0.5f;                
                Vector3 targetVector3 = new Vector3(target, 0f, 0f) + Main.transform.position;
                targetVector3.x = Mathf.Clamp(targetVector3.x, 0, 8);
                CheckStateDone();
                if (isStateDone) return;
                Main.position = Vector3.SmoothDamp(Main.transform.position, targetVector3, ref velocity, smoothTime);
            }
            prevPos = Input.mousePosition;
            UIhand.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0))
        {
            delta.x = 0;
        }
    }
    private void CheckStateDone()
    {
        if (B2.parent == A1)
        {
            _MoveToNextState();
            isStateDone = true;
            canDrag = false;
        }
    }
    public void _MoveToNextState()
    {
        StartCoroutine(MoveToNextState(currentState, nextState));
    }
}
