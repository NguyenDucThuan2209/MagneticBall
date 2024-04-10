using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class CarpetingBehavior : MonoBehaviour
{
    public AudioSource CarpetSound;
    public float baseAngle;
    public float factor;
    public float speed;
    public float snapMultiplier;
    public bool isDone;

    public List<Transform> ballsLeft;

    public float t = 0f;
    public float Lerpvalue;

    [HideInInspector] public bool isWait;
    Quaternion startRot;
    List<Quaternion> startRots = new List<Quaternion>();
    bool isFormingCarpet = true;
    bool isFlattening = false;
    private void Awake()
    {
        ballsLeft = GetComponent<BallsChain>().balls;
        if(ballsLeft.Count > 0)
            startRot = ballsLeft[0].localRotation;
        foreach (var ball in ballsLeft)
        {
            startRots.Add(ball.localRotation);
        }
        CarpetSound = GetComponent<AudioSource>();
    }
    private void Update()
    {

        LerpByDelta();
        //LerpByTime();
    }
    private Quaternion GetCarpetPointRotation(int index)
    {
        if (index < 0) return Quaternion.identity;

        var x = baseAngle + baseAngle * index * factor;
        return Quaternion.Euler(x, 0f, 0f);
    }

    [ContextMenu("Form Carpet")]
    private void FormCarpet()
    {
        var balls = GetComponent<BallsChain>().balls;
        for (int i = 0; i < balls.Count; i++)
        {
            var rot = balls[i].localRotation.eulerAngles;
            rot.x = baseAngle + baseAngle * i * factor;
            balls[i].localRotation = Quaternion.Euler(rot);
        }
    }

    [ContextMenu("Flatten Carpet")]
    private void FlattenCarpet()
    {
        var balls = GetComponent<BallsChain>().balls;
        foreach (var ball in balls)
        {
            ball.localRotation = Quaternion.identity;
        }
    }
    //Linh's Code
    void LerpByTime()
    {
        if (isFormingCarpet)
        {
            t += Time.deltaTime * speed / 2f;
            for (int i = 0; i < ballsLeft.Count; i++)
            {
                var targetRot = new Vector3(baseAngle + baseAngle * i * factor, 0f, 0f);
                ballsLeft[i].localRotation = Quaternion.Slerp(startRots[i], Quaternion.Euler(targetRot), t);
            }
            if (t >= 1f)
            {
                t = 0f;
                isFormingCarpet = false;

                startRot = ballsLeft[0].localRotation;
                isFlattening = true;
            }
        }

        if (isFlattening)
        {
            var deltaT = Time.deltaTime * speed * ((t > 0.35f) ? snapMultiplier : 1f);
            t += deltaT;
            for (int i = 0; i < ballsLeft.Count; i++)
            {
                ballsLeft[i].localRotation = Quaternion.Slerp(GetCarpetPointRotation(i), GetCarpetPointRotation(i - 1), t);
            }            
            //ballsLeft[0].localRotation = Quaternion.Slerp(startRot, Quaternion.identity, t);
            if (t >= 1f)
            {                
                t = 0f;
                ballsLeft.Remove(ballsLeft[0]);
                if (ballsLeft.Count == 0)
                {
                    isFlattening = false;
                    return;
                }
                startRot = ballsLeft[0].localRotation;
                //speed *= 1 + factor;
            }
        }
    }
    void LerpByDelta()
    {
        if (isWait) return;
        if (isFormingCarpet)
        {            
            t += Lerpvalue * speed / 0.5f;
            for (int i = 0; i < ballsLeft.Count; i++)
            {
                var targetRot = new Vector3(baseAngle + baseAngle * i * factor, 0f, 0f);
                ballsLeft[i].localRotation = Quaternion.Slerp(startRots[i], Quaternion.Euler(targetRot), t);
                isWait = true; Lerpvalue = 0;
            }
            if (t >= 1f)
            {
                t = 0f;
                isFormingCarpet = false;   
                startRot = ballsLeft[0].localRotation;
                isFlattening = true;
            }
        }

        if (isFlattening)
        {
            isWait = true;
            var deltaT = Lerpvalue * speed * ((t > 0.35f) ? snapMultiplier : 1f);
            t += deltaT;
            for (int i = 0; i < ballsLeft.Count; i++)
            {
                ballsLeft[i].localRotation = Quaternion.Slerp(GetCarpetPointRotation(i), GetCarpetPointRotation(i - 1), t);
            }
            //ballsLeft[0].localRotation = Quaternion.Slerp(startRot, Quaternion.identity, t);
            if (t >= 1f)
            {
                t = 0f;                
                ballsLeft.Remove(ballsLeft[0]);
                //if (!CarpetSound.isPlaying) CarpetSound.Play();
                CarpetSound.PlayOneShot(CarpetSound.clip);
                if (ballsLeft.Count == 0)
                {
                    isFlattening = false;
                    isDone = true;
                    return;
                }
                startRot = ballsLeft[0].localRotation;
                //speed *= 1 + factor;
            }
        }
    }
}
