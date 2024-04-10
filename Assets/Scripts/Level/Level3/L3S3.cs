using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Audio;

public class L3S3 : Level3
{
    [Header("Position for Cubes")]
    [SerializeField] float XWait;
    [SerializeField] float YWait;
    [SerializeField] AudioSource ClosingSound;

    [Header("Cubes Properties")]
    [SerializeField] GameObject[] BallCube;
    [SerializeField] Transform CubeHolder;

    [Header("UI Properties")]
    [SerializeField] Button[] ButtonList;

    [SerializeField] Text GreenText;    
    [SerializeField] Text YellowText;        
    [SerializeField] Text RedText;
    int GreenNumber = 4;
    int YellowNumber = 4;
    int RedNumber = 4;

    [Header("Target UI")]
    [SerializeField] RectTransform FindPanel;
    [SerializeField] Image TargetImage;
    [SerializeField] Sprite[] TargetList;

    public const int cubeLimitInLine = 4;
    public const int stackLimit = 3;
    const float cubeSize = 2;
    const float XWaitStart = -3.5f, YWaitStart = 1.5f;    

    int stack;
    int cubeNumber;
    bool isDone;

    private void Start()
    {
        cubeNumber = 0;
        stack = 0;

        XWait = XWaitStart;
        YWait = YWaitStart;

        isWaiting = false;
        isDone = false;

        FindPanel.DOAnchorPosY(-125, 1);
        TargetImage.sprite = TargetList[0];
        ClosingSound = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (stack == stackLimit && !isDone)
        {
            isDone = true;
            StartCoroutine(_MoveToNextState());
        }
    }

    public void OnButtonClick(int number)
    {
        // if not the right color than return
        if (number != stack) return;

        // if a cube is assembling or stack is full then doing nothing
        if (isDone || isWaiting) return;

        // Decrease amount of cube
        DecreaseCubeNumber(number);

        // Define the even or odd stack
        if (stack % 2 == 0)
        {
            // Instantiate a cube with the right color
            GameObject cube = Instantiate(BallCube[number], CubeHolder);            

            // Assemble the cube
            StartCoroutine(AssembleCube(cube, new Vector3(XWait, YWait, 0), 1));                                                       
            CheckStack();
        }
        else
        {
            // Instantiate a cube with right color
            GameObject cube = Instantiate(BallCube[number], CubeHolder);                      

            // Assemble the cube           
            StartCoroutine(AssembleCube(cube, new Vector3(XWait, YWait, 0), -1));
            CheckStack();
        }

        // Wait for the cube is assembled
        isWaiting = true;
    }

    private void DecreaseCubeNumber(int number)
    {
        switch(number)
        {
            case 0:
                GreenNumber--;
                if (GreenNumber > 0) GreenText.text = "x" + GreenNumber;
                else HideButton(0);
                break;
            case 1:
                YellowNumber--;
                if (YellowNumber > 0) YellowText.text = "x" + YellowNumber;
                else HideButton(1);
                break;
            case 2:
                RedNumber--;
                if (RedNumber > 0) RedText.text = "x" + RedNumber;
                else HideButton(2);
                break;
        }
    }

    private void CheckStack()
    {
        cubeNumber++;

        if (cubeNumber == cubeLimitInLine)
        {            
            stack++;
            cubeNumber = 0;

            YWait = YWaitStart + cubeSize * stack;             

            if (stack < stackLimit)
            {
                TargetImage.sprite = TargetList[stack];
            }
        }

        if (stack % 2 == 0)
        {            
            XWait = XWaitStart + cubeSize * cubeNumber;                                  
        }
        else
        {
            XWait = -(XWaitStart + cubeSize * cubeNumber);            
        }

        // Stop waiting for cube
        //isWaiting = false;
    }
    IEnumerator AssembleCube_OLD(GameObject cube, Vector3 waitPosition, Vector3 assemblePosition)
    {
        // Move the cube to wait position
        /*
        cube.transform.DOLocalMove(waitPosition, 0.5f);
        yield return new WaitForSeconds(0.5f);
        */
        cube.transform.position = waitPosition;
        yield return null;

        // Assemble the cube in the right direction & position
        cube.transform.DOLocalRotate(new Vector3(0, cube.transform.localEulerAngles.y, -90), 0.25f).SetEase(Ease.Flash);
        cube.transform.DOLocalMove(assemblePosition, 0.15f).SetEase(Ease.Flash).OnComplete(() =>
                                                                                            {
                                                                                                ClosingSound.PlayOneShot(ClosingSound.clip);
                                                                                                isWaiting = false;
                                                                                            });
    }
    IEnumerator AssembleCube(GameObject cube, Vector3 waitPosition, float direction)
    {
        cube.transform.position = waitPosition;
        yield return null;

        cube.transform.DOLocalRotate(new Vector3(0, cube.transform.localEulerAngles.y, -90 * direction), 0.25f).OnComplete(() =>        
                                                                                                    {
                                                                                                        ClosingSound.PlayOneShot(ClosingSound.clip);
                                                                                                        isWaiting = false;
                                                                                                    });
    }
    IEnumerator _MoveToNextState()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveToNextState(currentState, nextState));
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
}
