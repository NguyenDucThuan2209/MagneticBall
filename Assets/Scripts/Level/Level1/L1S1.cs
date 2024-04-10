using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class L1S1 : Level1
{
    protected void Start()
    {
        effectPosition = new Vector3(0, 1, 1.5f);
        StartEvent();                        
    }
    protected override void StartEvent()
    {
        TargetPanel.DOAnchorPosY(-80, 1);
        subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
    }
    protected override void EndEvent()
    {

    }
    void Update()
    {
        if (canDrag && carpetingBehavior != null)
        {
            OnDrag();
        }
    }

    IEnumerator WaitOnNextChain()
    {
        yield return new WaitForSeconds(0.5f);
        bool limit = chainCount < chainLimit;
        if (limit)
        {
            subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
        }
        else
        {
            UIhand.SetActive(false);
            //StartCoroutine(MoveToNextState(currentState, nextState));

            Camera.main.transform.DOMove(new Vector3(0, 12.5f, -12.5f), 1f);
            Camera.main.transform.DOLocalRotate(new Vector3(45, 0, 0), 1f);
            chainLimit = 7;
            chainCount = 0;
            yield return new WaitForSeconds(1f);
            currentState.SetActive(false);
            nextState.SetActive(true);            
            //dockUI.DOAnchorPosY(0, 1);
        }
    }

    void OnNextChain()
    {
        var onChain = subChains[chainCount];
        carpetingBehavior = onChain.GetComponent<CarpetingBehavior>();        
        UIhand.SetActive(true);
        HandAnimator.Play("LeftToRight");
        canDrag = true;
        chainCount++;
        isWaiting = false;
    }
    protected override void OnDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            delta.x = (Input.mousePosition - prevPos).x * Time.deltaTime * 0.05f;            
            if (delta.x != 0)
            {
                //carpetingBehavior.Lerpvalue = Time.deltaTime;
                carpetingBehavior.Lerpvalue = delta.x;
                carpetingBehavior.isWait = false;
            }
            prevPos = Input.mousePosition;
            UIhand.SetActive(false);
            delta.x = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            delta.x = 0;
        }
        if (carpetingBehavior.ballsLeft.Count == 0 && !isWaiting)
        {
            StartCoroutine(WaitOnNextChain());
            isWaiting = true;
        }
    }
}
