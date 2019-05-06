using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinManager : MonoBehaviour
{
    int index = 0;
    float time;

    bool[] playerMove = new bool[4] { false, false, false, false };
    public bool[] PlayersMove {
        get { return playerMove; }
    }
    Vector3[] playerPos = new Vector3[4];
    public Vector3[] PlayerPos {
        get {
            return playerPos;
        }
        set {
            playerPos = value;
        }
    }

    public TestPlayerManager playerManager;
    //Player_Manager playerManager;

    List<NormalGoblin> freeNormalGoblins, usedNormalGoblins;
    List<ArcherGoblin> freeArcherGoblins, usedArcherGoblins;

    [System.Serializable]
    public struct GoblinInfo {
        public string typeName;
        public float sighDist;
        public float atkDist;
        public int hp;
        public int atkValue;
        public float speed;
        public float spawnHeight;
    }

    public GoblinInfo[] goblinInfo;


    // Start is called before the first frame update
    private void Awake()
    {
        Transform goblin;
        Transform goblins;

        goblins = transform.Find("NormalGoblins");
        freeNormalGoblins = new List<NormalGoblin>();
        usedNormalGoblins = new List<NormalGoblin>();
        for (int i = 0; i < goblins.childCount; i++) {
            goblin = goblins.GetChild(i);
            freeNormalGoblins.Add(new NormalGoblin());
            freeNormalGoblins[i].TestInit(goblin, goblinInfo[0], this);
            //freeNormalGoblins[i].Init(goblin, goblinInfo[0], playerManager, this);
            goblin.gameObject.SetActive(false);
        }

        goblins = transform.Find("ArcherGoblins");
        freeArcherGoblins = new List<ArcherGoblin>();
        usedArcherGoblins = new List<ArcherGoblin>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeArcherGoblins.Add(new ArcherGoblin());
            freeArcherGoblins[i].TestInit(goblin, goblinInfo[0], this);
            //freeArcherGoblins[i].Init(goblin, goblinInfo[0], playerManager, this);
            goblin.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            SpawnNormalGoblin(Vector3.zero, 1);
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            SpawnArcherGoblin(new Vector3(1,1,1),0);
        }

        for (index = 0; index < usedNormalGoblins.Count; index++) {
            usedNormalGoblins[index].Update();  
        }
        for (index = 0; index < usedArcherGoblins.Count; index++)
        {
            usedArcherGoblins[index].Update();
        }

    }

    void SpawnNormalGoblin(Vector3 pos, int col) {
        if (freeNormalGoblins.Count <= 0) return;
        NormalGoblin goblin = freeNormalGoblins[0];
        usedNormalGoblins.Add(goblin);
        goblin.Spawn(pos, col);
        freeNormalGoblins.RemoveAt(0);
        Debug.Log(freeNormalGoblins.Count);
    }
    void SpawnArcherGoblin(Vector3 pos, int col)
    {
        if (freeArcherGoblins.Count <= 0) return;
        ArcherGoblin goblin = freeArcherGoblins[0];
        usedArcherGoblins.Add(goblin);
        goblin.Spawn(pos, col);
        freeArcherGoblins.RemoveAt(0);
        Debug.Log(freeArcherGoblins.Count);
    }


    public void Recycle<T>(T goblin) where T : IEnemyUnit {
        if (goblin is NormalGoblin)
        {
            usedNormalGoblins.Remove(goblin as NormalGoblin);
            freeNormalGoblins.Add(goblin as NormalGoblin);
            Debug.Log(freeNormalGoblins.Count);
        }
        else if (goblin is ArcherGoblin) {
            usedArcherGoblins.Remove(goblin as ArcherGoblin);
            freeArcherGoblins.Add(goblin as ArcherGoblin);
            Debug.Log(freeArcherGoblins.Count);
        }
    }


    public void SetPlayersMove(int id, Vector3 pos) {
        playerMove[id] = true;
        playerPos[id] = pos;
    }

}
