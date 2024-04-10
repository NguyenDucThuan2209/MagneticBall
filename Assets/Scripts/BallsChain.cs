using UnityEngine;
using System.Collections.Generic;

public class BallsChain : MonoBehaviour
{
    public Transform ballPrefab;
    public float distanceBetweenBalls;
    public int ballsCount;
    public Material material;
    public List<Transform> balls = new List<Transform>();

    [ContextMenu("Generate Balls")]
    public void GenerateBalls()
    {
        for (int i = 0; i < ballsCount; i++)
        {
            var parent = balls.Count > 0 ? balls[balls.Count - 1].transform : transform;
            var newBall = Instantiate(ballPrefab, transform.position + i * distanceBetweenBalls * Vector3.forward, Quaternion.identity, parent);
            var renderers = newBall.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.material = material;
            }
            balls.Add(newBall);
        }
    }
    
    public void ChangeMaterial(Material material)
    {
        foreach (var ball in balls)
        {
            foreach (var item in ball.GetComponent<BallsChild>().childBalls)
            {
                item.GetComponent<MeshRenderer>().material.color = material.color;
            }
        }
    }
}
