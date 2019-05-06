using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerManager : MonoBehaviour
{

    Transform[] players = new Transform[4];
    Vector3[] playerPos = new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++) {
            playerPos[i] = players[i].position;
        }
    }

    public Vector3[] GetPlayerPos() {
        return playerPos;
    }

}
