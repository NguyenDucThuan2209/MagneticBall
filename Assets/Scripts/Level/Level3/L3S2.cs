using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class L3S2 : Level3
{
    [Header("State_3 Settings")]    
    [SerializeField] Transform SubChain;

    bool isStateDone;

    private void Start()
    {
        StartCoroutine(MoveSubChain());

        canDrag = true;
        carpetingBehavior = SubChain.GetComponent<CarpetingBehavior>();
    }
    private void Update()
    {
        if (isStateDone) return;
        if (carpetingBehavior.isDone)
        {
            StartCoroutine(_MoveToNextState());
            return;
        }
        if (canDrag)
        {
            OnDrag();
        }
    }
    protected override void OnDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            delta.x = (Input.mousePosition - prevPos).x * Time.deltaTime * 0.01f;
            if (delta.x != 0)
            {
                carpetingBehavior.Lerpvalue = Time.deltaTime;
                carpetingBehavior.isWait = false;
            }
            prevPos = Input.mousePosition;
            UIhand.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0))
        {
            delta.x = 0;
        }        
    }
    
    IEnumerator MoveSubChain()
    {        
        SubChain.DOLocalMoveX(-7f, 1f);
        yield return new WaitForSeconds(0.25f);
        UIhand.SetActive(true);
        HandAnimator.Play("LeftToRight");
    }

    IEnumerator _MoveToNextState()
    {
        isStateDone = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveToNextState(currentState, nextState));
    } 
}
