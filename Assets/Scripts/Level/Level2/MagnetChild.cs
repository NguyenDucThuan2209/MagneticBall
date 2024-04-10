using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Audio;
public class MagnetChild : MonoBehaviour
{
    [SerializeField] AudioSource PushSound;
    [SerializeField] Ease ease;
    [SerializeField] float offset;
    [SerializeField] bool lastPart;
    [SerializeField] CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] int index;
    [SerializeField] L2S1 l2s1;

    private void Awake()
    {
        PushSound = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider col)
    {
        if (!this.transform.GetComponent<Collider>().isTrigger) return;
        else this.transform.GetComponent<Collider>().isTrigger = false; 
        
        if(col.CompareTag("MagnetParent"))
        {
            StartCoroutine(SetTargetWeight(1f));
            transform.parent = col.transform;            
            tag = "MagnetParent";
            transform.DOLocalMoveX(offset, 0.1f).SetEase(Ease.Flash).OnComplete(() => PushSound.PlayOneShot(PushSound.clip));            
        }
        if(lastPart)
        {
            StartCoroutine(EndEvent());
        }
    }
    IEnumerator EndEvent()
    {
        yield return new WaitForSeconds(1);
        l2s1._MoveToNextState();
        l2s1.enabled = false;
    }
    IEnumerator SetTargetWeight(float targetWeight)
    {
        float t = 0;
        while (t < 1)
        {
            cinemachineTargetGroup.m_Targets[index].weight = Mathf.Lerp(0.5f, targetWeight, t);
            t += Time.deltaTime * 3;
            yield return null;
        }
        cinemachineTargetGroup.m_Targets[index].weight = targetWeight;
    }
}
