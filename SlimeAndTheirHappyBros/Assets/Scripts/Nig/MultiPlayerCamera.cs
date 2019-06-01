using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiPlayerCamera : MonoBehaviour{

    public List<Transform> AllPlayers;
    public Vector3 offset;
    public float SmoothTime =2.0f;
    public float maxZoom = 30.0f;
    public float minZoom = 10.0f;
    public float zoomLimiter = 40.0f;
    Vector3 CenterPoint;
    Vector3 NewPosition;
    Vector3 Velocity;
    Camera cam;
    public Transform FirstAlivePlayer;

    void Start(){
        cam = GetComponent<Camera>();
        FirstAlivePlayer = AllPlayers[0];
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
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,newZoom, Time.deltaTime /*SmoothTime*/);
    }

    Vector3 GetCenterPoint() {
        for (int p = 0; p < AllPlayers.Count; p++) {
            if (p < 4 && AllPlayers[p].GetComponent<Player_Control>().DeathPriority == false){
                FirstAlivePlayer = AllPlayers[p];
                break;
            }
            else if (p >= 4 && AllPlayers[p].gameObject.activeSelf == true) {
                FirstAlivePlayer = AllPlayers[p];
                break;
            }
        }

        //理論上用不到
        //if (AllPlayers.Count == 1) {
        //    return AllPlayers[0].position;
        //}

        var bounds = new Bounds(FirstAlivePlayer.position, Vector3.zero);
        for (int p = 0; p < AllPlayers.Count; p++) {
            if(p<4 && AllPlayers[p].GetComponent<Player_Control>().DeathPriority == false) bounds.Encapsulate(AllPlayers[p].position);
            else if(p >= 4 && AllPlayers[p].gameObject.activeSelf == true)bounds.Encapsulate(AllPlayers[p].position);
        }

        return bounds.center;
    }

    float GetGreatestDistance() {
        var bounds = new Bounds(FirstAlivePlayer.position, Vector3.zero);
        for (int p = 0; p < AllPlayers.Count; p++){
            if (p < 4 && AllPlayers[p].GetComponent<Player_Control>().DeathPriority == false) bounds.Encapsulate(AllPlayers[p].position);
            else if (p >= 4 && AllPlayers[p].gameObject.activeSelf == true) bounds.Encapsulate(AllPlayers[p].position);
        }

        float greatest_total = bounds.size.x* bounds.size.x + bounds.size.z*bounds.size.z;
        return greatest_total;
    }




}
