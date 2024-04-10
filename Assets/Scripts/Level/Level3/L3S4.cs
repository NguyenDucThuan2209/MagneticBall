using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class L3S4 : Level3
{
    [Header("Individual Properties")]
    [SerializeField] CinemachineVirtualCamera vcam_2;
    [SerializeField] Transform LeftBackCollumn;
    [SerializeField] Transform RightBackCollumn;
    [SerializeField] AudioSource AssembleSound;

    [Header("Dragging Part")]
    [SerializeField] Transform DraggingDeck;
    [SerializeField] Transform DraggingEntrance;
    [SerializeField] Transform DraggingCollumn;

    [Header("DisplayPart")]
    [SerializeField] Transform DisplayDeck;
    [SerializeField] Transform DisplayLeftEntrance;
    [SerializeField] Transform DisplayRightEntrance;
    [SerializeField] Transform DisplayLeftCollumn;
    [SerializeField] Transform DisplayRightCollumn;
    [SerializeField] GameObject DisplayPlace;

    [Header("UI Properties")]
    [SerializeField] RectTransform DockUI;
    [SerializeField] Button[] ButtonList;
    [SerializeField] Text DeckAmountText;
    [SerializeField] Text EntranceAmountText;
    [SerializeField] Text CollumnAmountText;

    [Header("Target UI")]
    [SerializeField] Canvas TargetCanvas;
    [SerializeField] RectTransform FindPanel, TargetPanel;
    [SerializeField] Image TargetImage;
    [SerializeField] Sprite[] TargetList;

    [Header("Choosen Part")]
    [SerializeField] Transform PartHolder;
    [SerializeField] GameObject Car;
    enum ChoosenPart { Deck, Entrance, LeftEntrance, RightEntrance, Collumn, LeftCollumn, RightCollumn, Nothing }
    [SerializeField] ChoosenPart CurrentPart = ChoosenPart.Nothing;

    Vector3 originRotation;
    bool isLevelEnd;
    bool hasAttachedDeck;
    bool hasAttachedLeftEntrance;
    bool hasAttachedRightEntrance;
    bool hasAttachedLeftCollumn;
    bool hasAttachedRightCollumn;
    int collumnAmount = 2; int entranceAmount = 2; int deckAmount = 1;
    CustomLevelController m_CustomLevelController;

    private void Start()
    {
        SetEntranceAmount(entranceAmount);
        SetDeckAmount(deckAmount);
        SetCollumnAmount(collumnAmount);
        StartCoroutine(LateStart());

        m_CustomLevelController = FindObjectOfType<CustomLevelController>();
        isWaiting = false;

        TargetImage.sprite = TargetList[0];
        DisplayPlace.transform.GetChild(0).gameObject.SetActive(true);

        AssembleSound = GetComponent<AudioSource>();
    }
    private void LateUpdate()
    {
        if (collumnAmount + entranceAmount + deckAmount == 0 && !isWaiting && !Car.activeInHierarchy && !isLevelEnd)
        {
            EndEvent();
        }
    }
    IEnumerator LateStart()
    {
        FindPanel.DOAnchorPosY(-125, 1);       
        DockUI.DOAnchorPosY(0, 1);

        LeftBackCollumn.DOMoveX(-6, 1f);
        RightBackCollumn.DOMoveX(6, 1f);
        yield return new WaitForSeconds(0.25f);
        vcam_2.Priority = 11;
    }

    // Dragging Part    
    public void MoveDeck()
    {
        // Active Part
        DraggingDeck.gameObject.SetActive(true);

        // Attach Part to movement of mouse position
        var Vector3 = new Vector3(Input.mousePosition.x - 1, Input.mousePosition.y + 5, 20);
        DraggingDeck.transform.position = Camera.main.ScreenToWorldPoint(Vector3);

        // Capture Part's info
        CurrentPart = ChoosenPart.Deck;
        PartHolder = DraggingDeck;
        originRotation = PartHolder.localEulerAngles;
    }    
    public void MoveEntrance()
    {
        // Active Part
        DraggingEntrance.gameObject.SetActive(true);

        // Attach Part to movement of mouse position
        var Vector3 = new Vector3(Input.mousePosition.x - 1, Input.mousePosition.y + 5, 20);
        DraggingEntrance.transform.position = Camera.main.ScreenToWorldPoint(Vector3);

        // Capture Part's info
        CurrentPart = ChoosenPart.Entrance;
        PartHolder = DraggingEntrance;        
        originRotation = PartHolder.localEulerAngles;        
    }    
    public void MoveCollumn()
    {
        // Active Part to movement of mouse position
        DraggingCollumn.gameObject.SetActive(true);

        // Attach Part to moving
        var Vector3 = new Vector3(Input.mousePosition.x - 1, Input.mousePosition.y + 5, 18);
        DraggingCollumn.transform.position = Camera.main.ScreenToWorldPoint(Vector3);

        // Capture Part's info
        CurrentPart = ChoosenPart.Collumn;
        PartHolder = DraggingCollumn;
        originRotation = PartHolder.localEulerAngles;
    }    
    public void EndDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (!hasAttachedDeck) // Must attach the body first
                {
                    if (hit.transform.gameObject.CompareTag("Deck") && CurrentPart == ChoosenPart.Deck)
                    {
                        // Deactive Collider and Dragging Place
                        hit.collider.gameObject.SetActive(false);                        

                        // Decrease chain amount
                        deckAmount--;
                        if (deckAmount > 0) SetDeckAmount(deckAmount);
                        else HideButton(0);

                        // Moving Part  
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayDeck, CurrentPart));

                        // Confirm that the body has been attached
                        hasAttachedDeck = true;
                    }
                }
                else if (!hasAttachedLeftEntrance || !hasAttachedRightEntrance) // Must attach the entrance next
                {
                    if (hit.transform.gameObject.CompareTag("LeftEntrance"))
                    {
                        // Deactive Collider and Dragging Place
                        hit.collider.gameObject.SetActive(false);                        

                        // Specify the position of entrance
                        CurrentPart = ChoosenPart.LeftEntrance;

                        // Decrease chain amount
                        entranceAmount--;
                        if (entranceAmount > 0) SetEntranceAmount(entranceAmount);
                        else HideButton(1);

                        // Moving Part
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayLeftEntrance, CurrentPart));

                        // Confirm that the part has been attached
                        hasAttachedLeftEntrance = true;
                    }
                    else if (hit.transform.gameObject.CompareTag("RightEntrance"))
                    {
                        // Deactive Collider and Dragging Place
                        hit.collider.gameObject.SetActive(false);                        

                        // Specify the position of entrance
                        CurrentPart = ChoosenPart.RightEntrance;

                        // Decrease chain amount
                        entranceAmount--;
                        if (entranceAmount > 0) SetEntranceAmount(entranceAmount);
                        else HideButton(1);

                        // Moving Part
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayRightEntrance, CurrentPart));

                        // Confirm that the part has been attached
                        hasAttachedRightEntrance = true;
                    }
                }
                else
                {
                    if (hit.transform.gameObject.CompareTag("LeftCollumn"))
                    {
                        // Decactive Collider and Dragging Place
                        hit.collider.gameObject.SetActive(false);                        

                        // Specify the position of entrance
                        CurrentPart = ChoosenPart.LeftCollumn;

                        // Decrease chain amount
                        collumnAmount--;
                        if (collumnAmount > 0) SetCollumnAmount(collumnAmount);
                        else HideButton(2);

                        // Moving Part
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayLeftCollumn, CurrentPart));

                        hasAttachedLeftCollumn = true;
                    }
                    else if (hit.transform.gameObject.CompareTag("RightCollumn"))
                    {
                        // Decactive Collider and Dragging Place
                        hit.collider.gameObject.SetActive(false);                        

                        // Specify the position of entrance
                        CurrentPart = ChoosenPart.RightCollumn;

                        // Decrease chain amount
                        collumnAmount--;
                        if (collumnAmount > 0) SetCollumnAmount(collumnAmount);
                        else HideButton(2);

                        // Moving Part
                        StartCoroutine(IE_AssemblePart(PartHolder, DisplayRightCollumn, CurrentPart));

                        hasAttachedRightCollumn = true;
                    }
                }                
            }
            StartCoroutine(IE_TurnOffDraggingPart());
        }
    }   

    // IEnumertator   
    private IEnumerator IE_AssemblePart(Transform choosenPart, Transform targetPart, ChoosenPart part)
    {
        isWaiting = true;

        // Set the start & end position
        Vector3 startPos = choosenPart.position;
        Vector3 startRot = choosenPart.localEulerAngles;

        Vector3 endPos = targetPart.position;
        Vector3 endRot = targetPart.localEulerAngles;

        // Moving Part
        AssembleSound.PlayDelayed(0.45f);
        float t = 0;
        while (t < 1)
        {
            choosenPart.position = Vector3.Lerp(startPos, endPos, t * 2f);
            choosenPart.localEulerAngles = Vector3.Lerp(startRot, endRot, t * 2f);
            t += Time.deltaTime;
            yield return null;
        }        

        // Active DisplayPart
        switch (part)
        {
            case ChoosenPart.Deck:
                DisplayDeck.gameObject.SetActive(true);                
                break;

            case ChoosenPart.LeftEntrance:
                DisplayLeftEntrance.gameObject.SetActive(true);                                
                break;
            case ChoosenPart.RightEntrance:
                DisplayRightEntrance.gameObject.SetActive(true);                                
                break;

            case ChoosenPart.LeftCollumn:
                DisplayLeftCollumn.gameObject.SetActive(true);
                break;
            case ChoosenPart.RightCollumn:
                DisplayRightCollumn.gameObject.SetActive(true);
                break;
        }

        // Active Collider && TargetImage for next assemble
        if (hasAttachedDeck)
        {
            if (!hasAttachedRightEntrance || !hasAttachedLeftEntrance)
            {
                TargetImage.sprite = TargetList[1];                              
                if (!hasAttachedLeftEntrance)
                {
                    DisplayPlace.transform.GetChild(1).gameObject.SetActive(true);
                }
                if (!hasAttachedRightEntrance)
                {               
                    DisplayPlace.transform.GetChild(2).gameObject.SetActive(true);
                }
            }
            else
            {                
                TargetImage.sprite = TargetList[2];
                if (!hasAttachedLeftCollumn)
                {                    
                    DisplayPlace.transform.GetChild(3).gameObject.SetActive(true);
                }
                if (!hasAttachedRightCollumn)
                {                    
                    DisplayPlace.transform.GetChild(4).gameObject.SetActive(true);
                }
            }
        }

        isWaiting = false;
    }

    IEnumerator IE_TurnOffDraggingPart()
    {
        yield return new WaitUntil(() => !isWaiting);
        // Deactive Dragging Part                
        PartHolder.gameObject.SetActive(false);
        PartHolder.localEulerAngles = originRotation;        
        CurrentPart = ChoosenPart.Nothing;
        if (collumnAmount + entranceAmount + deckAmount == 0)
        {
            TargetCanvas.gameObject.SetActive(false);
            TargetPanel.gameObject.SetActive(false);
            DockUI.DOAnchorPosY(-500f, 1f);            
            vcam_2.gameObject.SetActive(false);
            Camera.main.transform.DOLocalMove(new Vector3(-7.5f, 18, -20), 1f);
            Camera.main.transform.DOLocalRotate(new Vector3(30, 20, 0), 1f);
            Car.SetActive(true);
        }
    }
    IEnumerator IE_EndLevel()
    {
        var effect = Instantiate(this.AssembleEffect, Camera.main.transform);
        effect.transform.localPosition = new Vector3(0, 0, 0);
        effect.Play();
        StateSound.PlayOneShot(EndLevel);
        yield return new WaitForSeconds(2.5f);
        this.gameObject.SetActive(false);
        m_CustomLevelController.LevelState = CustomLevelController.State.Win;
        m_CustomLevelController.EndLevel();
    }

    // Setting Chain Amount
    private void SetCollumnAmount(int amount)
    {
        CollumnAmountText.text = "x" + amount;
    }
    private void SetEntranceAmount(int amount)
    {
        EntranceAmountText.text = "x" + amount;
    }
    private void SetDeckAmount(int amount)
    {
        DeckAmountText.text = "x" + amount;
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
