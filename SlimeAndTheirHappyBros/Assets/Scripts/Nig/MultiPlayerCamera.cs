using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiPlayerCamera : MonoBehaviour{

    public List<Transform> AllPlayers;
    public Vector3 offset;
    public float SmoothTime =1.0f;
    public float maxZoom = 30.0f;
    public float minZoom = 10.0f;
    public float zoomLimiter = 40.0f;
    Vector3 CenterPoint;
    Vector3 NewPosition;
    Vector3 Velocity;
    Camera cam;

    void Start(){
        cam = GetComponent<Camera>();
    }

    void LateUpdate(){
        if (AllPlayers.Count == 0) return;
        Move();
        Zoom();
    }


    void Move(){
        CenterPoint = GetCenterPoint();
        NewPosition = CenterPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, NewPosition, ref Velocity, SmoothTime);
    }

    void Zoom() {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, 1.0f - Mathf.Sqrt(GetGreatestDistance()) / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,newZoom, /*Time.deltaTime*/ SmoothTime);
    }

    Vector3 GetCenterPoint() {
        if (AllPlayers.Count == 1) {
            return AllPlayers[0].position;
        }

        var bounds = new Bounds(AllPlayers[0].position, Vector3.zero);
        for (int i = 0; i < AllPlayers.Count; i++) {
            bounds.Encapsulate(AllPlayers[i].position);
        }

        return bounds.center;
    }

    float GetGreatestDistance() {
        var bounds = new Bounds(AllPlayers[0].position, Vector3.zero);
        for (int i = 0; i < AllPlayers.Count; i++){
            bounds.Encapsulate(AllPlayers[i].position);
        }

        float greatest_total = bounds.size.x* bounds.size.x + bounds.size.z*bounds.size.z;
        return greatest_total;
    }




}
