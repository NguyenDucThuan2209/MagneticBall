using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class L2S2 : Level2
{
    [SerializeField] float cameraOffsetZ, cameraOffsetY;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    void Start()
    {
        UIhand.SetActive(true);
        canDrag = true;
        cameraOffsetY = cinemachineVirtualCamera.transform.position.y;
        cameraOffsetZ = cinemachineVirtualCamera.transform.position.z;
        subChains[chainCount].transform.DOLocalMoveX(-2.5f, 1f).SetEase(ease).OnComplete(() => OnNextChain());
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
            if (chainCount % 2 == 0)
            {
                subChains[chainCount].transform.DOLocalMoveX(-2.5f, 1f).SetEase(ease).OnComplete(() 
                                                                                                => { 
                                                                                                    OnNextChain(); 
                                                                                                    UIhand.SetActive(true);
                                                                                                    HandAnimator.Play("LeftToRight");
                                                                                                   }
                                                                                                );
                
            }
            else
            {
                subChains[chainCount].transform.DOLocalMoveX(2.5f, 1f).SetEase(ease).OnComplete(()
                                                                                => {
                                                                                    OnNextChain();
                                                                                    UIhand.SetActive(true);
                                                                                    HandAnimator.Play("RightToLeft");
                                                                                }
                                                                                );
            }
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

    void OnNextChain()
    {
        var onChain = subChains[chainCount];
        carpetingBehavior = onChain.GetComponent<CarpetingBehavior>();        
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
        if (carpetingBehavior.ballsLeft.Count == 0 && !isWaiting)
        {
            cameraOffsetZ -= 0.75f;
            cameraOffsetY += 0.75f;
            var cameraOffset = new Vector3(cinemachineVirtualCamera.transform.position.x, cameraOffsetY, cameraOffsetZ);
            cinemachineVirtualCamera.transform.DOMove(cameraOffset, 1f);
            StartCoroutine(WaitOnNextChain());
            isWaiting = true;
        }
    }
}
