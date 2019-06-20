using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking
{
    bool inShaking = false;
    float time = .0f, shakeTime = .0f, endTime = .0f;
    float shakeLength = .0f;
    Transform cameraTransform;
    // Start is called before the first frame update
    void Init(Transform camera)
    {
        cameraTransform = camera;
    }

    // Update is called once per frame
    void Update(float dt)
    {
        if (inShaking) {
            if (time < shakeTime)
            {
                time += dt;

            }
            else {

            }
            cameraTransform.position += new Vector3();
        }
    }

    public void StartShake()
    {

    }
    public void StartShake(float time) {

    }
    public void StartShakeEasyOut(float shakeT, float endT, float strength)
    {
        shakeTime = time;
        shakeLength = strength;
        inShaking = true;
    }
    public void EndShake() {

    }

}
