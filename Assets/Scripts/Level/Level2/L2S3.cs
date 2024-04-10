using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
public class L2S3 : Level2
{
    [Header("State_3 Settings")]
    [SerializeField] Transform mainChain;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    void Start()
    {
        StartCoroutine(Flip());
    }

    // Addition
    void CheckAnimation()
    {
        switch(chainCount)
        {
            case 1:
                HandAnimator.Play("LeftToRight");
                break;
            case 2:
                HandAnimator.Play("RightToLeft");
                break;
            case 3:
                HandAnimator.Play("UpToDown");
                break;
            case 4:
                HandAnimator.Play("DownToUp");
                break;
            case 5:
                HandAnimator.Play("LeftToRight");
                break;
        }
    }
    void OnNextChain()
    {
        var onChain = subChains[chainCount++];
        carpetingBehavior = onChain.GetComponent<CarpetingBehavior>();

        UIhand.SetActive(true);

        canDrag = true;        
        isWaiting = false;
    }

    private void Update()
    {
        if (UIhand.activeInHierarchy)
        {
            CheckAnimation();
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
            delta.y = (Input.mousePosition - prevPos).x * Time.deltaTime * 0.01f;

            if (delta.x > 0 && chainCount == 1)
            {
                carpetingBehavior.Lerpvalue = Time.deltaTime;
            }
            if(delta.x < 0 && chainCount == 2)
            {
                carpetingBehavior.Lerpvalue = Time.deltaTime;
            }
            if (delta.y < 0 && chainCount == 3)
            {
                carpetingBehavior.Lerpvalue = Time.deltaTime;
            }
            if (delta.y > 0 && chainCount == 4)
            {
                carpetingBehavior.Lerpvalue = Time.deltaTime;
            }
            if (delta.x > 0 && chainCount == 5)
            {
                carpetingBehavior.Lerpvalue = Time.deltaTime;
            }
            carpetingBehavior.isWait = false;
            prevPos = Input.mousePosition;
            UIhand.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0))
        {
            delta.x = 0;
        }
        if (carpetingBehavior.ballsLeft.Count == 0 && !isWaiting)
        {
            carpetingBehavior.transform.parent = mainChain.transform;
            if(chainCount == 1)
            {
                StartCoroutine(FlipRight());
            }
            if (chainCount == 2)
            {
                StartCoroutine(FlipUp());
            }
            if (chainCount == 3)
            {
                StartCoroutine(FlipDown());
            }
            if(chainCount ==  4)
            {
                StartCoroutine(FlipBack());
            }
            if(chainCount == 5)
            {
                EndEvent();
            }
            isWaiting = true;
        }
    }

    // Flip Coroutine
    IEnumerator Flip()
    {
        // Push the mainChain up to localPosition.y = 3 in 0.5s
        mainChain.DOLocalMoveY(3, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);

        // Rotate the main Chain 
        mainChain.DOLocalRotate(new Vector3(90, 90, 0), 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);

        // Push the mainChain down to localPosition.y = 1.5 in 0.5s
        mainChain.DOLocalMoveY(1.5f, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);

        // Take the subChain into scene
        subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
    }
    IEnumerator FlipRight()
    {
        mainChain.DOLocalMoveY(4, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalRotate(new Vector3(-90, 90, 0), 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalMoveY(2.5f, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
    }
    IEnumerator FlipUp()
    {
        //StartCoroutine(CameraZoomOut());
        mainChain.DOLocalMoveY(3, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalRotate(new Vector3(-180, 0, 90), 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalMoveY(1.5f, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
    }
    IEnumerator FlipDown()
    {
        mainChain.DOLocalMoveY(4, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalRotate(new Vector3(-180, 90, -90), 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalMoveY(2.5f, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
    }
    IEnumerator FlipBack()
    {
        mainChain.DOLocalMoveY(4, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalRotate(new Vector3(0, 90, 0), 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        mainChain.DOLocalMoveY(2, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.25f);
        subChains[chainCount].transform.DOLocalMove(chainsPosition[chainCount], 1f).SetEase(ease).OnComplete(() => OnNextChain());
    }

    // No using Coroutine
    IEnumerator CameraZoomOut()
    {
        while (virtualCamera.m_Lens.FieldOfView < 80)
        {
            virtualCamera.m_Lens.FieldOfView += 0.1f;
            yield return null;
        }        
    }
    IEnumerator WaitOnNextChain()
    {
        yield return new WaitForSeconds(0.5f);
        bool limit = chainCount < chainLimit;
        if (limit)
        {
            subChains[chainCount].transform.DOLocalMoveX(-2.5f, 1f).SetEase(ease).OnComplete(() => OnNextChain());
        }
        else
        {
            UIhand.SetActive(false);
            StartCoroutine(MoveToNextState(currentState, nextState));
            chainLimit = 4;
            chainCount = 0;
            yield return new WaitForSeconds(1f);
        }
    }

    // End state
    protected override void EndEvent()
    {
        StartCoroutine(MoveToNextState(currentState, nextState));
    }

}
