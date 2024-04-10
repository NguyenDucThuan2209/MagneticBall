using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class L2S4 : Level2
{
    [Header("Individual Properties")]
    [SerializeField] CinemachineVirtualCamera vcam_2;
    [SerializeField] Transform Leg1;
    [SerializeField] Transform Leg2;
    [SerializeField] AudioSource AssembleSound;

    [Header("Dragging Part")]
    [SerializeField] Transform DraggingHand;
    [SerializeField] Transform DraggingBody;
    [SerializeField] Transform DraggingHead;

    [Header("DisplayPart")]
    [SerializeField] Transform DisplayLeftHand;
    [SerializeField] Transform DisplayRightHand;
    [SerializeField] Transform DisplayBody;
    [SerializeField] Transform DisplayHead;
    [SerializeField] GameObject DisplayPlace;

    [Header("UI Properties")]
    [SerializeField] RectTransform ChainDock;
    [SerializeField] Button[] ButtonList;
    [SerializeField] Text HandAmountText;
    [SerializeField] Text HeadAmountText;
    [SerializeField] Text BodyAmountText;

    [Header("Target UI")]
    [SerializeField] Canvas TargetCanvas;
    [SerializeField] RectTransform FindPanel;
    [SerializeField] RectTransform TargetPanel;
    [SerializeField] Image TargetImage;
    [SerializeField] Sprite[] TargetList;
    
    [Header("Choosen Part")]
    [SerializeField] Transform PartHolder;
    enum ChoosenPart { Hand, LeftHand, RightHand, Body, Head, Nothing }
    [SerializeField] ChoosenPart CurrentPart = ChoosenPart.Nothing;

    bool isLevelEnd;
    bool hasAttachedBody, hasAttachedLeftHand, hasAttachedRightHand;        
    int handAmount = 2; int headAmount = 1; int bodyAmount = 1;
    private CustomLevelController m_CustomLevelController;    

    private void Start()
    {
        SetHeadAmount(headAmount);
        SetBodyAmount(bodyAmount);
        SetHandAmount(handAmount);
        StartCoroutine(LateStart());

        m_CustomLevelController = FindObjectOfType<CustomLevelController>();
        isWaiting = false;

        TargetImage.sprite = TargetList[0];
        DisplayPlace.transform.GetChild(0).gameObject.SetActive(true);

        AssembleSound = GetComponent<AudioSource>();
    }
    private void LateUpdate()
    {        
        if (handAmount + headAmount + bodyAmount == 0 && !isWaiting && !isLevelEnd)
        {            
            EndEvent();
        }
    }
    IEnumerator LateStart()
    {
        FindPanel.DOAnchorPosY(-180, 1);
        TargetPanel.DOAnchorPosY(-180, 1);
        ChainDock.DOAnchorPosY(0, 1);

        Leg1.DOMoveX(-0.5f, 1f);
        Leg2.DOMoveX(2.5f, 1f);
        yield return new WaitForSeconds(0.25f);
        AssembleSound.PlayDelayed(0.5f);
        vcam_2.Priority = 11;
    }

    // Dragging Part
    public void MoveHand()
    {        
        // Active Part
        DraggingHand.gameObject.SetActive(true);

        // Attach Part to movement of mouse position
        var Vector3 = new Vector3(Input.mousePosition.x - 1, Input.mousePosition.y + 5, 15);
        DraggingHand.transform.position = Camera.main.ScreenToWorldPoint(Vector3);

        // Capture Part's info
        CurrentPart = ChoosenPart.Hand;
        PartHolder = DraggingHand;
    }
    public void MoveBody()
    {        
        // Active Part
        DraggingBody.gameObject.SetActive(true);

        // Attach Part to movement of mouse position
        var Vector3 = new Vector3(Input.mousePosition.x - 1, Input.mousePosition.y + 5, 20);
        DraggingBody.transform.position = Camera.main.ScreenToWorldPoint(Vector3);

        // Capture Part's info
        CurrentPart = ChoosenPart.Body;
        PartHolder = DraggingBody;
    }
    public void MoveHead()
    {        
        // Active Part to movement of mouse position
        DraggingHead.gameObject.SetActive(true);

        // Attach Part to moving
        var Vector3 = new Vector3(Input.mousePosition.x - 1, Input.mousePosition.y + 5, 12);
        DraggingHead.transform.position = Camera.main.ScreenToWorldPoint(Vector3);

        // Capture Part's info
        CurrentPart = ChoosenPart.Head;
        PartHolder = DraggingHead;
    }    
    public void EndDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (!hasAttachedBody) // Must attach the body first
                {
                    if (hit.transform.gameObject.CompareTag("Body") && CurrentPart == ChoosenPart.Body)
                    {
                        // Deactive Collider
                        hit.collider.gameObject.SetActive(false);

                        // Decrease chain amount
                        bodyAmount--;
                        if (bodyAmount > 0) SetBodyAmount(bodyAmount);
                        else HideButton(0);

                        // Moving Part  
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayBody, CurrentPart));

                        // Confirm that the body has been attached
                        hasAttachedBody = true;
                    }
                }
                else if (!hasAttachedLeftHand || !hasAttachedRightHand)
                {
                    if (hit.transform.gameObject.CompareTag("LeftHand") && CurrentPart == ChoosenPart.Hand)
                    {
                        // Deactive Collider
                        hit.collider.gameObject.SetActive(false);

                        // Decrease chain amount
                        handAmount--;
                        if (handAmount > 0) SetHandAmount(handAmount);
                        else HideButton(1);

                        // Set hand side
                        CurrentPart = ChoosenPart.LeftHand;

                        // Moving Part
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayLeftHand, CurrentPart));

                        // Confirm that the part has been attached
                        hasAttachedLeftHand = true;
                    }
                    else if (hit.transform.gameObject.CompareTag("RightHand") && CurrentPart == ChoosenPart.Hand)
                    {
                        // Deactive Collider
                        hit.collider.gameObject.SetActive(false);

                        // Decrease chain amount
                        handAmount--;
                        if (handAmount > 0) SetHandAmount(handAmount);
                        else HideButton(1);

                        // Set hand side
                        CurrentPart = ChoosenPart.RightHand;

                        // Moving Part
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayRightHand, CurrentPart));

                        // Confirm that the part has been attached
                        hasAttachedRightHand = true;
                    }
                }
                else if (hit.transform.gameObject.CompareTag("Head") && CurrentPart == ChoosenPart.Head)
                {
                    // Deactive Collider
                    hit.collider.gameObject.SetActive(false);

                    // Decrease chain amount
                    headAmount--;
                    if (headAmount > 0) SetHeadAmount(headAmount);
                    else HideButton(2);

                    // Moving Part
                    StartCoroutine(IE_AssemblePart(PartHolder, DisplayHead, CurrentPart));

                    // Deactive the targetPanel
                    TargetCanvas.gameObject.SetActive(false);
                    ChainDock.gameObject.SetActive(false);
                    TargetPanel.gameObject.SetActive(false);
                }               
            }            
            StartCoroutine(IE_TurnOffDraggingPart());
        }
    }
    private IEnumerator IE_TurnOffDraggingPart()
    {
        yield return new WaitUntil(() => !isWaiting);        
        // Deactive the dragging part
        PartHolder.gameObject.SetActive(false);
        // Nothing is being choosen
        CurrentPart = ChoosenPart.Nothing;
    }
    private IEnumerator IE_EndLevel()
    {
        DisplayPlace.SetActive(false);
        var effect = Instantiate(this.AssembleEffect, Camera.main.transform);
        effect.transform.localPosition = new Vector3(0, 0, 0);
        effect.Play();
        StateSound.PlayOneShot(EndLevel);
        yield return new WaitForSeconds(2.5f);
        this.gameObject.SetActive(false);
        m_CustomLevelController.LevelState = CustomLevelController.State.Win;
        m_CustomLevelController.EndLevel();
    }

    // Assemble Part
    private IEnumerator IE_AssemblePart(Transform choosenPart, Transform targetPart, ChoosenPart part)
    {
        // Start moving
        isWaiting = true;

        // Set the start & end position
        Vector3 start = choosenPart.position;
        Vector3 end = targetPart.position;

        // Moving Part
        AssembleSound.PlayDelayed(0.45f);
        float t = 0;
        while (t < 1)
        {            
            choosenPart.position = Vector3.Lerp(start, end, t * 2f);
            t += Time.deltaTime;
            yield return null;
        }  
        
        // Active DisplayPart
        switch (part)
        {
            case ChoosenPart.Body:
                DisplayBody.gameObject.SetActive(true);
                break;

            case ChoosenPart.LeftHand:
                DisplayLeftHand.gameObject.SetActive(true);
                break;

            case ChoosenPart.RightHand:
                DisplayRightHand.gameObject.SetActive(true);
                break;

            case ChoosenPart.Head:                
                DisplayHead.gameObject.SetActive(true);                               
                break;
        }

        // Active Collider && TargetImage for next assemble
        if (hasAttachedBody)
        {
            if (!hasAttachedLeftHand || !hasAttachedRightHand)
            {
                TargetImage.sprite = TargetList[1];
                if (!hasAttachedLeftHand)
                {
                    DisplayPlace.transform.GetChild(1).gameObject.SetActive(true);
                }
                if (!hasAttachedRightHand)
                {
                    DisplayPlace.transform.GetChild(2).gameObject.SetActive(true);
                }
            }
            else
            {
                TargetImage.sprite = TargetList[2];
                DisplayPlace.transform.GetChild(3).gameObject.SetActive(true);
            }
        }
        isWaiting = false;
    }

    // Setting Chain Amount
    private void SetHandAmount(int amount)
    {
        HandAmountText.text = "x" + amount;
    }
    private void SetHeadAmount(int amount)
    {
        HeadAmountText.text = "x" + amount;
    }
    private void SetBodyAmount(int amount)
    {
        BodyAmountText.text = "x" + amount;
    }

    private void HideButton(int buttonNum)
    {
        Vector3 tmp = ButtonList[buttonNum].transform.position;
        Vector3 buttonPos;
        ButtonList[buttonNum].gameObject.SetActive(false);

        for (int i = buttonNum + 1; i < ButtonList.Length; i++)
        {
            if (!ButtonList[i].IsActive()) continue;            
            buttonPos = tmp;
            tmp = ButtonList[i].transform.position;            

            ButtonList[i].transform.DOMove(buttonPos, 1f);            
        }
    }

    protected override void EndEvent()
    {
        isLevelEnd = true;
        StartCoroutine(IE_EndLevel());
    }
}
