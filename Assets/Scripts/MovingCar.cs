using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MovingCar : MonoBehaviour
{
    [SerializeField] GameObject m_ListHolder;
    List<Transform> m_positionList = new List<Transform>();

    private void Awake()
    {
        for (int i = 0; i < m_ListHolder.transform.childCount; i++)
        {
            m_positionList.Add(m_ListHolder.transform.GetChild(i));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CarMovement());
    }

    private IEnumerator CarMovement()
    {
        foreach (Transform position in m_positionList)
        {
            this.transform.DOMove(position.position, 1f);
            this.transform.DOLocalRotate(position.localEulerAngles, 0.75f);
            yield return new WaitForSeconds(0.5f);
        }
        this.gameObject.SetActive(false);
    }
}
