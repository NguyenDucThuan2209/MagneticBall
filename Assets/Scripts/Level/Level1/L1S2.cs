using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.UI;

public class L1S2 : Level1
{
    [SerializeField] protected Material[] materials;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] float cameraOffsetZ, cameraOffsetY;
    [Header("UI Properties")]
    [SerializeField] Button[] ButtonList;
    [SerializeField] Sprite[] TargetList;
    [SerializeField] Canvas TargetCanvas;
    [SerializeField] RectTransform FindPanel;    
    [SerializeField] RectTransform ChainDock;
    [SerializeField] Image TargetImage;    

    private CustomLevelController m_CustomLevelController;    
    bool isLevelEnded = false;
    bool canGenerate = true;
    bool canHide = false;

    private void Start()
    {
        m_CustomLevelController = FindObjectOfType<CustomLevelController>();
        TargetCanvas.gameObject.SetActive(true);                
        TargetImage.sprite = TargetList[chainCount + 1];

        FindPanel.DOAnchorPosY(-80, 1);        
        ChainDock.DOAnchorPosY(0, 1);

        var color = TargetImage.color;
        color.a = 1;
        TargetImage.color = color;
    }

    protected override void StartEvent()
    {
        cameraOffsetY = cinemachineVirtualCamera.transform.position.y;
        cameraOffsetZ = cinemachineVirtualCamera.transform.position.z;
    }
    public void GenerateChain(int colorNum)
    {        
        if (chainCount < chainLimit && chainCount + 1 == colorNum && canGenerate)
        {
            var color = materials[colorNum];
            var ballChain = subChains[chainCount].gameObject;
            //ballChain.GetComponent<BallsChain>().ChangeMaterial(color);
            if (chainCount % 2 == 0)
            {
                ballChain.transform.DOLocalMoveX(-3.5f, 1f).OnComplete(() => StartCoroutine(OnNextChain()));
            }
            else
            {
                ballChain.transform.DOLocalMoveX(3.5f, 1f).OnComplete(() => StartCoroutine(OnNextChain()));
            }
            canGenerate = false;
            canHide = true;
        }
    }
    void Update()
    {
        if (canDrag && carpetingBehavior != null && !isLevelEnded)
        {            
            OnDrag();
        }
    }
    IEnumerator OnNextChain()
    {
        var onChain = subChains[chainCount];
        carpetingBehavior = onChain.GetComponent<CarpetingBehavior>();

        UIhand.SetActive(true);        
        if (chainCount % 2 == 0)
        {
            HandAnimator.Play("LeftToRight");
        }
        else
        {
            HandAnimator.Play("RightToLeft");
        }

        chainCount++;
        canDrag = true;
        isWaiting = false;
        yield return new WaitUntil(() => carpetingBehavior.isDone);
        canGenerate = true;
        if (chainCount < chainLimit)
        {
            TargetImage.sprite = TargetList[chainCount + 1];
            //dockUI.DOAnchorPosY(0f, 1);
        }
        else
        {
            dockUI.GetComponentInParent<Image>().GetComponent<RectTransform>().DOAnchorPosY(-500f, 1);
            TargetPanel.DOAnchorPosY(180, 1);

            cinemachineVirtualCamera.gameObject.SetActive(false);
            Camera.main.transform.DOLocalMove(new Vector3(0, 20, -15), 1f).OnComplete(() => 
                                                                                  { 
                                                                                    
                                                                                    EndEvent(); 
                                                                                  });            
        }
    }
    IEnumerator IE_EndLevel()
    {
        canDrag = false;
        isLevelEnded = true;
        
        TargetCanvas.gameObject.SetActive(false);

        var effect = Instantiate(AssembleEffect, Camera.main.transform);
        effect.transform.localPosition = new Vector3(0, 0, 0);
        effect.Play();

        StateSound.PlayOneShot(EndLevel);

        yield return new WaitForSeconds(2.5f);

        this.gameObject.SetActive(false);
        m_CustomLevelController.LevelState = CustomLevelController.State.Win;
        m_CustomLevelController.EndLevel();        
        yield return null;
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
            if (delta.x < 0 && chainCount % 2 != 0) return; // if left2right but player drag right2left then cancel.
            if (delta.x > 0 && chainCount % 2 == 0) return; // oppositie case.
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
            if (chainCount == chainLimit)
            {
                TargetCanvas.gameObject.SetActive(false);
            }
            else
            {
                TargetCanvas.gameObject.SetActive(true);
                
            }
        }
        if (carpetingBehavior.ballsLeft.Count == 0 && !isWaiting)
        {
            cameraOffsetZ -= 0.5f;
            cameraOffsetY += 0.5f;
            var cameraOffset = new Vector3(cinemachineVirtualCamera.transform.position.x, cameraOffsetY, cameraOffsetZ);
            cinemachineVirtualCamera.transform.DOMove(cinemachineVirtualCamera.transform.position + cameraOffset * 0.5f, 1f);
            isWaiting = true;
        }        
    }
    private void HideButton(Button button)
    {
        /*
        //button.GetComponent<Image>().enabled = false;
        //button.interactable = false;
        RectTransform buttonHolder = button.GetComponentInParent<LayoutElement>().GetComponent<RectTransform>();
        RectTransform buttonRect = buttonHolder.GetComponentInParent<HorizontalLayoutGroup>().GetComponent<RectTransform>();

        buttonHolder.DOMoveX(buttonHolder.position.x - 333, 1);
        buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x - 150, 250);
        */
        button.gameObject.SetActive(false);
    }
    public void HideButton(int buttonNum)
    {
        // To hide a button after change its position:
        // 1. You need to edit the ButtonList's member follow the new order
        // 2. You need to change the buttonNum in every button so it stay correct order in the ButtonList
        if (!canHide) return;
                
        Vector3 tmp = ButtonList[buttonNum].transform.position;
        Vector3 buttonPos;
        ButtonList[buttonNum].gameObject.SetActive(false);

        for (int i = buttonNum + 1; i < ButtonList.Length; i++)
        {
            if (!ButtonList[i].IsActive()) continue;            
            buttonPos = tmp;
            tmp = ButtonList[i].transform.position;            

            ButtonList[i].transform.DOMove(buttonPos, 1f);
            dockUI.sizeDelta = new Vector2(dockUI.sizeDelta.x - 45, 250);            
        }
        canHide = false;
    }

    protected override void EndEvent()
    {
        StartCoroutine(IE_EndLevel());
    }
}
