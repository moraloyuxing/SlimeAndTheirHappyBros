using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerManager : MonoBehaviour
{

    Transform[] players = new Transform[4];
    Vector3[] playerPos = new Vector3[4];
    Vector3[] oldPlayerPos = new Vector3[4];

    GoblinManager goblinManager;

    // Start is called before the first frame update
    private void Awake()
    {
        goblinManager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();
    }
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            players[i] = transform.GetChild(i);
            playerPos[i] = players[i].position;
            goblinManager.SetPlayersMove(i, playerPos[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++) {
            playerPos[i] = players[i].position;
            Vector3 diff = new Vector3(playerPos[i].x - oldPlayerPos[i].x, 0 , playerPos[i].z - oldPlayerPos[i].z);
            if (diff.sqrMagnitude > 0.25f) {
                goblinManager.SetPlayersMove(i, playerPos[i]);
                oldPlayerPos[i] = playerPos[i];
            }
        }
    }

    public Vector3[] GetPlayerPos() {
        return playerPos;
    }

}
