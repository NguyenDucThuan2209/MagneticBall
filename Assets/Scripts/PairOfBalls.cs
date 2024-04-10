using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairOfBalls : MonoBehaviour
{
    public Transform pivot;

    private void Update()
    {
        pivot.rotation = Quaternion.identity;
    }
}
