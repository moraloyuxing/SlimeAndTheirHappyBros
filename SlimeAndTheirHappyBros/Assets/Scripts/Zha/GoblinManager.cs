using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinManager : MonoBehaviour
{
    int index = 0;
    float time;

    public Vector3[] spawnPos;

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

    List<NormalGoblin> freeNormalGoblins, usedNormalGoblins;
    List<ArcherGoblin> freeArcherGoblins, usedArcherGoblins;
    List<HobGoblin> freeHobGoblins, usedHobGoblins;

    Dictionary<string, GoblinBase> goblinDic = new Dictionary<string, GoblinBase>();

    Dictionary<string, GoblinArrow> goblinArrowsDic;
    List<GoblinArrow> freeGoblinArrows, usedGoblinArrows;
    Dictionary<string, GoblinLeaf> goblinLeafDic;
    List<GoblinLeaf> freeGoblinLeaves, usedGoblinLeaves;


    public TestPlayerManager playerManager;
    //Player_Manager playerManager;
    public Vector2 mapBorder;

    [System.Serializable]
    public struct GoblinInfo {
        public string typeName;
        public float sighDist;
        public float atkDist;
        public int hp;
        public int atkValue;
        public float speed;
        public float spawnHeight;

        public float turnDist;
        public float stopDist;
    }
    public GoblinInfo[] goblinInfo;

    [System.Serializable]
    public struct PoolUnitInfo {
        public string typeName;
        public float speed;
        public float length;
    }
    public PoolUnitInfo[] poolUnitInfo;

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
            goblinDic.Add(goblin.name, freeNormalGoblins[i]);
            goblin.gameObject.SetActive(false);
        }

        goblins = transform.Find("ArcherGoblins");
        freeArcherGoblins = new List<ArcherGoblin>();
        usedArcherGoblins = new List<ArcherGoblin>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeArcherGoblins.Add(new ArcherGoblin());
            freeArcherGoblins[i].TestInit(goblin, goblinInfo[1], this);
            //freeArcherGoblins[i].Init(goblin, goblinInfo[0], playerManager, this);
            goblinDic.Add(goblin.name, freeArcherGoblins[i]);
            goblin.gameObject.SetActive(false);
        }

        goblins = transform.Find("HobGoblins");
        freeHobGoblins = new List<HobGoblin>();
        usedHobGoblins = new List<HobGoblin>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeHobGoblins.Add(new HobGoblin());
            freeHobGoblins[i].TestInit(goblin, goblinInfo[2], this);
            //freeArcherGoblins[i].Init(goblin, goblinInfo[0], playerManager, this);
            goblinDic.Add(goblin.name, freeHobGoblins[i]);
            goblin.gameObject.SetActive(false);
        }


        Transform locs = transform.Find("SpawnLocs");
        spawnPos = new Vector3[locs.childCount];
        for (int i = 0; i < spawnPos.Length; i++) {
            spawnPos[i] = locs.GetChild(i).position;
        }

        goblins = transform.Find("GoblinArrows");
        freeGoblinArrows = new List<GoblinArrow>();
        usedGoblinArrows = new List<GoblinArrow>();
        goblinArrowsDic = new Dictionary<string, GoblinArrow>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeGoblinArrows.Add(new GoblinArrow());
            freeGoblinArrows[i].Init(goblin, this, poolUnitInfo[0]);
            goblinArrowsDic.Add(goblin.name, freeGoblinArrows[i]);
            goblin.gameObject.SetActive(false);
        }
        goblins = transform.Find("GoblinLeaves");
        freeGoblinLeaves = new List<GoblinLeaf>();
        usedGoblinLeaves = new List<GoblinLeaf>();
        goblinLeafDic = new Dictionary<string, GoblinLeaf>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeGoblinLeaves.Add(new GoblinLeaf());
            freeGoblinLeaves[i].Init(goblin, this, poolUnitInfo[0]);
            goblinLeafDic.Add(goblin.name, freeGoblinLeaves[i]);
            goblin.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        mapBorder =  0.5f * GameObject.Find("PathFinding").GetComponent<PathFindGrid>().gridWorldSize;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A)) {
            SpawnNormalGoblinRandomPos(-1);
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            //SpawnNormalGoblinRandomPos(-1);
            SpawnArcherGoblininRandomPos(-1);
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            SpawnHobGoblininRandomPos(-1);
        }

        for (index = 0; index < usedNormalGoblins.Count; index++) {
            usedNormalGoblins[index].Update(dt);
        }
        for (index = 0; index < usedArcherGoblins.Count; index++)
        {
            usedArcherGoblins[index].Update(dt);
        }
        for (index = 0; index < usedHobGoblins.Count; index++)
        {
            usedHobGoblins[index].Update(dt);
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            if (freeGoblinArrows.Count <= 0) return;
            Debug.Log("spawn arrow");
            GoblinArrow arrow = freeGoblinArrows[0];
            usedGoblinArrows.Add(arrow);
            arrow.ToActive(new Vector3(-14.0f, 1.65f, -18.0f), new Vector3(0,0,-1).normalized);
            freeGoblinArrows.Remove(arrow);
        }

        for (index = 0; index < usedGoblinArrows.Count; index++) {
            usedGoblinArrows[index].Update(dt);
        }
        for (index = 0; index < usedGoblinLeaves.Count; index++)
        {
            usedGoblinLeaves[index].Update(dt);
        }


        playerMove[0] = false;
        playerMove[1] = false;
        playerMove[2] = false;
        playerMove[3] = false;
    }

    void SpawnNormalGoblinRandomPos(int col)
    {
        if (freeNormalGoblins.Count <= 0) return;
        NormalGoblin goblin = freeNormalGoblins[0];
        usedNormalGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0,13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeNormalGoblins.RemoveAt(0);
    }
    void SpawnNormalGoblinSpecificPos(Vector3 pos, int col) {
        if (freeNormalGoblins.Count <= 0) return;
        NormalGoblin goblin = freeNormalGoblins[0];
        usedNormalGoblins.Add(goblin);
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeNormalGoblins.RemoveAt(0);
    }

    void SpawnArcherGoblininRandomPos(int col)
    {
        if (freeArcherGoblins.Count <= 0) return;
        ArcherGoblin goblin = freeArcherGoblins[0];
        usedArcherGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeArcherGoblins.RemoveAt(0);
    }
    void SpawnArcherSpecificPos(Vector3 pos, int col)
    {
        if (freeArcherGoblins.Count <= 0) return;
        ArcherGoblin goblin = freeArcherGoblins[0];
        usedArcherGoblins.Add(goblin);
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeArcherGoblins.RemoveAt(0);
    }
    void SpawnHobGoblininRandomPos(int col)
    {
        if (freeHobGoblins.Count <= 0) return;
        HobGoblin goblin = freeHobGoblins[0];
        usedHobGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeHobGoblins.RemoveAt(0);
    }
    void SpawnHobSpecificPos(Vector3 pos, int col)
    {
        if (freeHobGoblins.Count <= 0) return;
        HobGoblin goblin = freeHobGoblins[0];
        usedHobGoblins.Add(goblin);
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeHobGoblins.RemoveAt(0);
    }

    public void RecycleGoblin<T>(T goblin) where T : IEnemyUnit {
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

    public void UseArrow(Vector3 pos, Vector3 dir) {
        if (freeGoblinArrows.Count <= 0) return;
        GoblinArrow arrow = freeGoblinArrows[0];
        usedGoblinArrows.Add(arrow);
        arrow.ToActive(pos, dir);
        freeGoblinArrows.Remove(arrow);
    }
    public void UseLeaf(Vector3 pos, Vector3 dir)
    {
        if (freeGoblinLeaves.Count <= 0) return;
        GoblinLeaf leaf = freeGoblinLeaves[0];
        usedGoblinLeaves.Add(leaf);
        leaf.ToActive(pos, dir);
        freeGoblinLeaves.Remove(leaf);
    }

    public void RecycleArrow(GoblinArrow arrow) {
        freeGoblinArrows.Add(arrow);
        usedGoblinArrows.Remove(arrow);
        
    }

    public void RecycleLeaf(GoblinLeaf leaf) {
        freeGoblinLeaves.Add(leaf);
        usedGoblinLeaves.Remove(leaf);
    }


    public void SetPlayersMove(int id, Vector3 pos) {
        playerMove[id] = true;
        playerPos[id] = pos;
    }

    public GoblinBase FindGoblin(string name) {
        if (goblinDic.ContainsKey(name))
        {
            return goblinDic[name];
        }
        else return null;
    }

}
