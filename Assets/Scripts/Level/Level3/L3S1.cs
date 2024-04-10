using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class L3S1 : Level3
{
    [Header("Parts")]
    [SerializeField] Transform Main;
    [SerializeField] Transform B1;
    [SerializeField] Transform B2;

    [Header("UI Properties")]
    [SerializeField] RectTransform TargetPanel;
    
    float target;
    float smoothTime = 0.3F;
    Vector3 velocity = Vector3.zero;
    bool isStateOver;

    void Start()
    {
        UIhand.SetActive(true);
        HandAnimator.Play("LeftToRight");
        TargetPanel.DOAnchorPosY(-125, 1);

        canDrag = true;
    }
    private void Update()
    {
        if (isStateOver) return;
        CheckStateOver();
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
            UIhand.SetActive(false);
            delta.x = (Input.mousePosition - prevPos).x * Time.deltaTime;
            if (delta.x > 0)
            {
                target += delta.x * 0.5f;
                Vector3 targetVector3 = new Vector3(target, 0f, 0f);
                Main.position = Vector3.SmoothDamp(Main.transform.position, Main.transform.position + targetVector3, ref velocity, smoothTime);
            }
            prevPos = Input.mousePosition;
            UIhand.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0))
        {
            delta.x = 0;
        }
    }

    private void CheckStateOver()
    {
        if (B2.parent == B1)
        {
            StartCoroutine(_MoveToNextState());
            isStateOver = true;
        }   
    }
    IEnumerator _MoveToNextState()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveToNextState(currentState, nextState));
    }
}
