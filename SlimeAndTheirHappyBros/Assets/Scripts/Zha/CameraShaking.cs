using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking
{
    bool inShaking = false;
    float time = .0f, shakeTime = .0f, endTime = .0f, perTime = 1.0f;
    float shakeLength = .0f, weakLevel = .0f, shakeWay = 1.0f;
    Vector3 shakeVec, oringingVec, shakeDir = new Vector3(.0f,.0f,.0f);
    Transform cameraTransform;
    // Start is called before the first frame update
    public void Init()
    {
        cameraTransform = GameObject.Find("Main Camera").transform;
    }
    public void Init(Transform camera)
    {
        cameraTransform = camera;
    }

    // Update is called once per frame
    public void Update(float dt, Vector3 pos)
    {
        if (inShaking) {
            time += dt;
            if (time > shakeTime)
            {
                if (time < (endTime))
                {
                    shakeLength -= weakLevel;
                    if (shakeLength < .0f) shakeLength = .0f;
                }
                else {
                    inShaking = false;
                    time = .0f;
                    perTime = .0f;
                }
            }
            perTime += dt;
            if (perTime > 0.05f) {
                shakeVec = (shakeWay * shakeLength) * (new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), .0f).normalized);
                cameraTransform.position = pos + (shakeVec);
                shakeWay *= -1.0f;
                perTime = .0f;
                Debug.Log(shakeLength);
            }
            
        }
    }

    public void StartShake()
    {

    }
    public void StartShake(float time) {

    }
    public void StartShakeEasyOut(float shakeT, float endT, float strength)
    {
        shakeTime = shakeT;
        endTime = shakeTime+endT;
        shakeLength = strength;
        inShaking = true;
        weakLevel = Time.deltaTime / endT;
    }
    public void EndShake() {

    }

}
