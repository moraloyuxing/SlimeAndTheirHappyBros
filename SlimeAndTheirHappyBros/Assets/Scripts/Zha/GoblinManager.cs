using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinManager : MonoBehaviour
{
    float moneyFix = 2.0f;

    bool gameOver = false, bossTime = false, gamePause = false;
    int index = 0;
    float time;

    public Vector3[] spawnPos;

    bool[] playerMove = new bool[4] { false, false, false, false };
    bool[] playerDie = new bool[4] { false, false, false, false };
    public bool[] PlayersDie {
        get { return playerDie; }
    }
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

    bool calculatePath = false;
    public bool CalculatePath {
        set { calculatePath = value; }
    }

    List<NormalGoblin> freeNormalGoblins, usedNormalGoblins;
    List<ArcherGoblin> freeArcherGoblins, usedArcherGoblins;
    List<HobGoblin> freeHobGoblins, usedHobGoblins;

    Dictionary<string, GoblinBase> goblinDic = new Dictionary<string, GoblinBase>();

    KingGoblin kingGoblin;
    GoblinSpirit goblinSpirit;

    Dictionary<string, GoblinArrow> goblinArrowsDic;
    List<GoblinArrow> freeGoblinArrows, usedGoblinArrows;
    Dictionary<string, GoblinLeaf> goblinLeafDic;
    List<GoblinLeaf> freeGoblinLeaves, usedGoblinLeaves;
    List<GoblinWave> freeGoblinWaves, usedGoblinWaves;
    List<FallingGoblin> freeFallingGoblins, usedFallingGoblin;
    List<GoblinEnergyPunch> freeGoblinPunches, usedGoblinPunches;
    List<GoblinEnergyBall> freeGoblinBalls, usedGoblinBalls;
    List<Money> freeMoneys, usedMoneys;

    PathFindGrid pathGrid;

    public Player_Control[] Four_Player = new Player_Control[4];
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

        public int minMoney;
        public int maxMoney;

    }
    public GoblinInfo[] goblinInfo;

    [System.Serializable]
    public struct PoolUnitInfo {
        public string typeName;
        public float speed;
        public float length;
    }
    public PoolUnitInfo[] poolUnitInfo;

    System.Action KillGoblin;

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
            goblinDic.Add(goblin.name, freeHobGoblins[i]);
            goblin.gameObject.SetActive(false);
        }

        goblin = transform.Find("KingGoblin");
        kingGoblin = new KingGoblin();
        kingGoblin.Init(goblin, goblinInfo[2], this);
        goblin.gameObject.SetActive(false);

        goblin = transform.Find("GoblinSpirit");
        goblinSpirit = new GoblinSpirit();
        goblinSpirit.Init(goblin, this, kingGoblin);
        goblin.gameObject.SetActive(false);

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
            freeGoblinLeaves[i].Init(goblin, this, poolUnitInfo[1]);
            goblinLeafDic.Add(goblin.name, freeGoblinLeaves[i]);
            goblin.gameObject.SetActive(false);
        }
        goblins = transform.Find("GoblinWaves");
        freeGoblinWaves = new List<GoblinWave>();
        usedGoblinWaves = new List<GoblinWave>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeGoblinWaves.Add(new GoblinWave());
            freeGoblinWaves[i].Init(goblin, this, poolUnitInfo[1]);
            goblin.gameObject.SetActive(false);
        }
        goblins = transform.Find("FallingGoblins");
        freeFallingGoblins = new List<FallingGoblin>();
        usedFallingGoblin = new List<FallingGoblin>();
        for (int i = 0; i < goblins.childCount; i++) {
            goblin = goblins.GetChild(i);
            freeFallingGoblins.Add(new FallingGoblin());
            freeFallingGoblins[i].Init(goblin, this, poolUnitInfo[1]);
            goblin.gameObject.SetActive(false);
        }
        goblins = transform.Find("GoblinEnergyPunchs");
        freeGoblinPunches = new List<GoblinEnergyPunch>();
        usedGoblinPunches = new List<GoblinEnergyPunch>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeGoblinPunches.Add(new GoblinEnergyPunch());
            freeGoblinPunches[i].Init(goblin, this, poolUnitInfo[1]);
            goblin.gameObject.SetActive(false);
        }
        goblins = transform.Find("GoblinEnergyBalls");
        freeGoblinBalls = new List<GoblinEnergyBall>();
        usedGoblinBalls = new List<GoblinEnergyBall>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeGoblinBalls.Add(new GoblinEnergyBall());
            freeGoblinBalls[i].Init(goblin, this);
            goblin.gameObject.SetActive(false);
        }

        goblins = transform.Find("Moneys");
        freeMoneys = new List<Money>();
        usedMoneys = new List<Money>();
        for (int i = 0; i < goblins.childCount; i++)
        {
            goblin = goblins.GetChild(i);
            freeMoneys.Add(new Money());
            freeMoneys[i].Init(goblin, this, poolUnitInfo[2]);
            goblin.gameObject.SetActive(false);
        }

    }
    void Start()
    {
        pathGrid = GameObject.Find("PathFinding").GetComponent<PathFindGrid>();
        mapBorder = 0.5f * pathGrid.gridWorldSize;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q)) gamePause = !gamePause;
        if (gameOver || gamePause) return;

        float dt = Time.deltaTime;

        //if (Input.GetKeyDown(KeyCode.Z)) {
        //    SpawnNormalGoblinBaseColor(4);
        //}
        //if (Input.GetKeyDown(KeyCode.X)) {
        //    //SpawnNormalGoblinRandomPos(-1);
        //    SpawnArcherGoblinBaseColor(4);
        //}
        //if (Input.GetKeyDown(KeyCode.C)) {
        //    SpawnHobGoblinMutiColor(5);
        //}

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
        if (bossTime) {
            kingGoblin.Update(dt);
            goblinSpirit.Update(dt);
        }
        if (calculatePath) PathRequestManager.ClearExtendPenalty();

        //if (Input.GetKeyDown(KeyCode.X)) {
        //    if (freeGoblinArrows.Count <= 0) return;
        //    Debug.Log("spawn arrow");
        //    GoblinArrow arrow = freeGoblinArrows[0];
        //    usedGoblinArrows.Add(arrow);
        //    arrow.ToActive(new Vector3(-14.0f, 1.65f, -18.0f), new Vector3(0,0,-1).normalized);
        //    freeGoblinArrows.Remove(arrow);
        //}

        for (index = 0; index < usedGoblinArrows.Count; index++) {
            usedGoblinArrows[index].Update(dt);
        }
        for (index = 0; index < usedGoblinLeaves.Count; index++)
        {
            usedGoblinLeaves[index].Update(dt);
        }
        for (index = 0; index < usedGoblinPunches.Count; index++)
        {
            usedGoblinPunches[index].Update(dt);
        }
        for (index = 0; index < usedGoblinWaves.Count; index++) {
            usedGoblinWaves[index].Update(dt);
        }
        for (index = 0; index < usedFallingGoblin.Count; index++)
        {
            usedFallingGoblin[index].Update(dt);
        }
        for (index = 0; index < usedGoblinBalls.Count; index++)
        {
            usedGoblinBalls[index].Update(dt);
        }
        for (index = 0; index < usedMoneys.Count; index++)
        {
            usedMoneys[index].Update(dt);
        }

        playerMove[0] = false;
        playerMove[1] = false;
        playerMove[2] = false;
        playerMove[3] = false;
        calculatePath = false;
    }

    public void SubKillGoblinCBK(System.Action cbk) {
        KillGoblin = cbk;
    }

    public void SubBreakShopCBK(System.Action shop, System.Action bush) {
        kingGoblin.SubPunchCBK(shop, bush);
    }

    public void SpawnNormalGoblinBaseColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 90);
            if (o < 30) col = 1;
            else if (o < 60) col = 2;
            else col = 4;
        }
        if (freeNormalGoblins.Count <= 0) return;
        NormalGoblin goblin = freeNormalGoblins[0];
        usedNormalGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeNormalGoblins.RemoveAt(0);
    }
    public void SpawnNormalGoblinMutiColor(int col) {
        if (col == 0)
        {
            float o = Random.Range(0, 120);
            if (o < 30) col = 1;
            else if (o < 60) col = 2;
            else if (o < 90) col = 4;
            else if (o < 100) col = 3;
            else if (o < 110) col = 5;
            else col = 6;
        }
        if (freeNormalGoblins.Count <= 0) return;
        NormalGoblin goblin = freeNormalGoblins[0];
        usedNormalGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeNormalGoblins.RemoveAt(0);
    }
    public void SpawnArcherGoblinBaseColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 90);
            if (o < 30) col = 1;
            else if (o < 60) col = 2;
            else col = 4;
        }
        if (freeArcherGoblins.Count <= 0) return;
        ArcherGoblin goblin = freeArcherGoblins[0];
        usedArcherGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeArcherGoblins.RemoveAt(0);
    }
    public void SpawnArcherGoblinMutiColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 120);
            if (o < 30) col = 1;
            else if (o < 60) col = 2;
            else if (o < 90) col = 4;
            else if (o < 100) col = 3;
            else if (o < 110) col = 5;
            else col = 6;
        }
        if (freeArcherGoblins.Count <= 0) return;
        ArcherGoblin goblin = freeArcherGoblins[0];
        usedArcherGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeArcherGoblins.RemoveAt(0);
    }

    public void SpawnHobGoblinMutiColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 90);
            if (o < 30) col = 3;
            else if (o < 60) col = 5;
            else col = 6;
        }
        if (freeHobGoblins.Count <= 0) return;
        HobGoblin goblin = freeHobGoblins[0];
        usedHobGoblins.Add(goblin);
        Vector3 pos = spawnPos[Random.Range(0, 13)];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeHobGoblins.RemoveAt(0);
    }

    public void BossSpawnNormalGoblinMutiColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 120);
            if (o < 30) col = 1;
            else if (o < 60) col = 2;
            else if (o < 90) col = 4;
            else if (o < 100) col = 3;
            else if (o < 110) col = 5;
            else col = 6;
        }
        if (freeNormalGoblins.Count <= 0) return;
        NormalGoblin goblin = freeNormalGoblins[0];
        usedNormalGoblins.Add(goblin);
        int baseN = (Random.Range(0, 10) > 5) ? 2 : 10;
        baseN += Random.Range(0, 3);
        Vector3 pos = spawnPos[baseN];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeNormalGoblins.RemoveAt(0);
    }
    public void BossSpawnArcherGoblinMutiColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 120);
            if (o < 30) col = 1;
            else if (o < 60) col = 2;
            else if (o < 90) col = 4;
            else if (o < 100) col = 3;
            else if (o < 110) col = 5;
            else col = 6;
        }
        if (freeArcherGoblins.Count <= 0) return;
        ArcherGoblin goblin = freeArcherGoblins[0];
        usedArcherGoblins.Add(goblin);
        int baseN = (Random.Range(0, 10) > 5) ? 2 : 10;
        baseN += Random.Range(0, 3);
        Vector3 pos = spawnPos[baseN];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeArcherGoblins.RemoveAt(0);
    }
    public void BossSpawnHobGoblinMutiColor(int col)
    {
        if (col == 0)
        {
            float o = Random.Range(0, 90);
            if (o < 30) col = 3;
            else if (o < 60) col = 5;
            else col = 6;
        }
        if (freeHobGoblins.Count <= 0) return;
        HobGoblin goblin = freeHobGoblins[0];
        usedHobGoblins.Add(goblin);
        int baseN = (Random.Range(0, 10) > 5) ? 2 : 10;
        baseN += Random.Range(0, 3);
        Vector3 pos = spawnPos[baseN];
        goblin.Spawn(pos, col);
        goblin.UpdateAllPlayerPos();
        freeHobGoblins.RemoveAt(0);
    }
    public void RecycleGoblin<T>(T goblin) where T : IEnemyUnit {
        if (goblin is NormalGoblin)
        {
            usedNormalGoblins.Remove(goblin as NormalGoblin);
            freeNormalGoblins.Add(goblin as NormalGoblin);
            //Debug.Log(freeNormalGoblins.Count);
        }
        else if (goblin is ArcherGoblin) {
            usedArcherGoblins.Remove(goblin as ArcherGoblin);
            freeArcherGoblins.Add(goblin as ArcherGoblin);
            //Debug.Log(freeArcherGoblins.Count);
        }
        else if (goblin is HobGoblin)
        {
            usedHobGoblins.Remove(goblin as HobGoblin);
            freeHobGoblins.Add(goblin as HobGoblin);
            //Debug.Log(freeHobGoblins.Count);
        }
        KillGoblin();
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
    public void UsePunch(Vector3 pos, Vector3 d, Quaternion rot)
    {
        if (freeGoblinPunches.Count <= 0) return;
        usedGoblinPunches.Add(freeGoblinPunches[0]);
        freeGoblinPunches[0].ToActive(pos, d, rot);
        freeGoblinPunches.Remove(freeGoblinPunches[0]);
    }
    public void UseWave(Vector3 pos) {
        if (freeGoblinWaves.Count <= 0) return;
        GoblinWave wave = freeGoblinWaves[0];
        usedGoblinWaves.Add(wave);
        wave.ToActive(pos, new Vector3(0, 0, 0));
        freeGoblinWaves.Remove(wave);
    }
    public void UseFallingGoblin(int id)
    {
        if (freeFallingGoblins.Count <= 0) return;
        usedFallingGoblin.Add(freeFallingGoblins[0]);
        freeFallingGoblins[0].ToActive(id);
        freeFallingGoblins.Remove(freeFallingGoblins[0]);
    }
    public void UseEnergyBall(Vector3 pos, Vector3 dir, float spd)
    {
        if (freeGoblinBalls.Count <= 0) return;
        usedGoblinBalls.Add(freeGoblinBalls[0]);
        freeGoblinBalls[0].ToActive(pos, dir, spd);
        freeGoblinBalls.Remove(freeGoblinBalls[0]);
    }
    public void UseMoney(int num, Vector3 pos, int target)
    {
        Four_Player[target].MoneyUpdate(Mathf.FloorToInt(moneyFix * num));//UI更新num
        StartCoroutine(DropMoney(num, pos, target));

    }
    public void UseMoney(int num, Vector3 pos, int target1, int target2)
    {
        Four_Player[target1].MoneyUpdate(Mathf.FloorToInt(moneyFix * num));//UI更新num
        Four_Player[target2].MoneyUpdate(Mathf.FloorToInt(moneyFix * num));//UI更新num
        StartCoroutine(DropMoney(num, pos, target1));

    }
    IEnumerator DropMoney(int num, Vector3 pos, int target) {
        Four_Player[target].MoneyUI_GoBigger();
        int i = 0;
        while (i < num)
        {
            if (freeMoneys.Count <= 0) break;
            Money money = freeMoneys[0];
            usedMoneys.Add(money);
            money.ToActive(pos, target);
            freeMoneys.Remove(money);
            i++;
            yield return new WaitForSeconds(0.18f);
        }
    }
    IEnumerator DropMoney2(int num, Vector3 pos, int target)
    {
        Four_Player[target].MoneyUI_GoBigger();
        int i = 0;
        while (i < num)
        {
            if (freeMoneys.Count <= 0) break;
            Money money = freeMoneys[0];
            usedMoneys.Add(money);
            money.ToActive(pos, target);
            freeMoneys.Remove(money);
            i++;
            yield return new WaitForSeconds(0.3f);
        }
    }
    public void RecycleArrow(GoblinArrow arrow) {
        freeGoblinArrows.Add(arrow);
        usedGoblinArrows.Remove(arrow);

    }
    public void RecycleLeaf(GoblinLeaf leaf) {
        freeGoblinLeaves.Add(leaf);
        usedGoblinLeaves.Remove(leaf);
    }
    public void RecyclePunches(GoblinEnergyPunch punch)
    {
        freeGoblinPunches.Add(punch);
        usedGoblinPunches.Remove(punch);
    }
    public void RecyleFallingGoblin(FallingGoblin goblin) {
        freeFallingGoblins.Add(goblin);
        usedFallingGoblin.Remove(goblin);
    }
    public void RecycleWave(GoblinWave wave) {
        freeGoblinWaves.Add(wave);
        usedGoblinWaves.Remove(wave);
    }
    public void RecycleEnergyBalls(GoblinEnergyBall ball)
    {
        freeGoblinBalls.Add(ball);
        usedGoblinBalls.Remove(ball);
    }
    public void RecycleMoney(Money money)
    {
        freeMoneys.Add(money);
        usedMoneys.Remove(money);
    }

    public void SetPlayersMove(int id, Vector3 pos) {
        playerMove[id] = true;
        playerPos[id] = pos;
    }

    public void SetPlayerDie(int id) {
        playerDie[id] = true;
    }
    public void SetPlayerRevive(int id) {
        playerDie[id] = false;
    }

    public void SpawnBoss() {
        bossTime = true;
        kingGoblin.Spawn(Vector3.zero, 0);
        goblinSpirit.Spawn(Vector3.zero, 0);
    }

    public void GrowGoblinHP() {
        for (int i = 0; i < freeNormalGoblins.Count; i++) {
            freeNormalGoblins[i].GrowMaxHp();
        }
        for (int i = 0; i < freeArcherGoblins.Count; i++)
        {
            freeArcherGoblins[i].GrowMaxHp();
        }
        for (int i = 0; i < freeHobGoblins.Count; i++)
        {
            freeHobGoblins[i].GrowMaxHp();
        }
    }
    public void ChangeKingGoblinHP(int round) {
        if (round == 2) moneyFix = 1.5f;
        else if(round > 2) moneyFix = 1.0f;
        int hp = 150 + 100 * (round - 1);
        kingGoblin.ChangeHp(hp);
    }
    public void KillAllGoblin()
    {
        for (int i = 0; i < usedNormalGoblins.Count;)
        {
            usedNormalGoblins[i].ResetUnit();
        }
        for (int i = 0; i < usedArcherGoblins.Count;)
        {
            usedArcherGoblins[i].ResetUnit();
        }
        for (int i = 0; i < usedHobGoblins.Count;)
        {
            usedHobGoblins[i].ResetUnit();
        }
    }
    public void DisableBushCollider() {
        pathGrid.DisableCollider();
    }

    public void SubDecreaseBossHp(System.Action<float>cbk) {
        kingGoblin.SubDevreaseBossHp(cbk);
    }
    public void SubGameWin(System.Action cbk)
    {
        kingGoblin.SubGameWin(cbk);
    }
    public void HitKingGoblin(int color, int value) {
        kingGoblin.GetHurt(color, value);
    }

    public GoblinBase FindGoblin(string name) {
        if (goblinDic.ContainsKey(name))
        {
            return goblinDic[name];
        }
        else return null;
    }

    public void GameOver() {
        gameOver = true;

        for (index = 0; index < usedNormalGoblins.Count; index++)
        {
            usedNormalGoblins[index].SetGameOver();
        }
        for (index = 0; index < usedArcherGoblins.Count; index++)
        {
            usedArcherGoblins[index].SetGameOver();
        }
        for (index = 0; index < usedHobGoblins.Count; index++)
        {
            usedHobGoblins[index].SetGameOver();
        }
        if (bossTime) {
            kingGoblin.SetGameOver();
            goblinSpirit.SetGameOver();
        }
    }
    public void GameOver(bool win)
    {
        gameOver = true;

        for (index = 0; index < usedNormalGoblins.Count; index++)
        {
            usedNormalGoblins[index].SetGameOver();
        }
        for (index = 0; index < usedArcherGoblins.Count; index++)
        {
            usedArcherGoblins[index].SetGameOver();
        }
        for (index = 0; index < usedHobGoblins.Count; index++)
        {
            usedHobGoblins[index].SetGameOver();
        }
        StartCoroutine(KingDisappear());
        kingGoblin.SetGameOver();
        goblinSpirit.SetGameOver();
    }

    IEnumerator KingDisappear() {
        yield return new WaitForSeconds(1.35f);
        kingGoblin.ResetUnit();
        goblinSpirit.ResetUnit();
    }

}
