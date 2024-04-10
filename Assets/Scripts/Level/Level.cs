using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : MonoBehaviour
{
    protected abstract void StartEvent();
    protected abstract void EndEvent();
    protected abstract void OnDrag();
    [SerializeField] protected GameObject currentState;
    [SerializeField] protected GameObject nextState;
    public enum GameState
    {
        Playing,
        Win,
        Lose
    }
    public GameState gameState;
    public ParticleSystem AssembleEffect;    
    public AudioSource StateSound;
    public AudioClip EndState;
    public AudioClip EndLevel;
    protected Vector3 effectPosition;
    protected IEnumerator MoveToNextState(GameObject firstState, GameObject nextState)
    {
        // Remove Effect after state
        /*
        var effect = Instantiate(AssembleEffect, Camera.main.transform);
        effect.transform.localPosition = new Vector3(0, 0, 7);

        effect.Play();
        StateSound.PlayOneShot(EndState);
        yield return new WaitForSeconds(2);
        */
        StartCoroutine(FadingPanel.Ins.FadePanel());
        yield return new WaitForSeconds(0.75f);

        firstState.SetActive(false);
        nextState.SetActive(true);
    }
}
