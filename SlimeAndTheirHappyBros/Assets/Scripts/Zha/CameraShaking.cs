using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking
{
    bool inShaking = false;
    float time = .0f, shakeTime = .0f;
    Transform cameraTransform;
    // Start is called before the first frame update
    void Init(Transform camera)
    {
        cameraTransform = camera;
    }

    // Update is called once per frame
    void Update(Vector3 curPos, float dt)
    {
        
    }

    public void StartShake()
    {

    }
    public void StartShake(float time) {

    }
    public void EndShake() {

    }

}
