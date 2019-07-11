using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobGoblin : GoblinBase, IEnemyUnit
{
    bool startAttack = false, endure = false, hasShoot = false;
    int quackStep = 0;
    int attackType = 0;
    float imgOffset, hurtAreaOffset;
    Vector3 shootOffset;
    Vector3 launchPos, shootFace;
    float[] atkColOffset = new float[3];
    float[] hintOffset = new float[2];
    Transform[] atkCol = new Transform[3];
    Transform[] atkHint = new Transform[2];
    Transform hurtArea;
    Collider hurtAreaCol;
    Collider[] atkCollider = new Collider[3];
    float[] sightDists = new float[2] { 15.0f, 20.0f };
    float[] atkDists = new float[2] {7.0f,  16.0f};

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        atkCol[0] = t.Find("AtkCollider");
        atkCol[1] = t.Find("AtkCollider (1)");
        atkCol[2] = t.Find("AtkCollider (2)");
        atkCollider[0] = atkCol[0].GetComponent<Collider>();
        atkCollider[1] = atkCol[1].GetComponent<Collider>();
        atkCollider[2] = atkCol[2].GetComponent<Collider>();
        atkHint[0] = t.Find("AttackHint");
        atkHint[1] = t.Find("AttackHint (1)");
        imgScale = image.localScale.x;
        imgOffset = image.localPosition.x;
        shootOffset = t.Find("ShootPos").localPosition;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
        atkColOffset[2] = atkCol[2].localPosition.x;
        hintOffset[0] = atkHint[0].localPosition.x;
        hintOffset[1] = atkHint[1].localPosition.x;
        hurtArea = t.Find("HurtArea");
        hurtAreaOffset = hurtArea.localPosition.x;
        hurtAreaCol = hurtArea.GetComponent<Collider>();
        maxHp = info.hp;
        hp = maxHp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
        minMoney = info.minMoney;
        maxMoney = info.maxMoney;

        attackType = Random.Range(0, 2);
        sightDist = sightDists[attackType];
        atkDist = atkDists[attackType];
    }

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        atkCol[0] = t.Find("AtkCollider");
        atkCol[1] = t.Find("AtkCollider (1)");
        atkCol[2] = t.Find("AtkCollider (2)");
        atkCollider[0] = atkCol[0].GetComponent<Collider>();
        atkCollider[1] = atkCol[1].GetComponent<Collider>();
        atkCollider[2] = atkCol[2].GetComponent<Collider>();
        atkHint[0] = t.Find("AttackHint");
        atkHint[1] = t.Find("AttackHint (1)");
        imgScale = image.localScale.x;
        imgOffset = image.localPosition.x;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
        atkColOffset[2] = atkCol[2].localPosition.x;
        hintOffset[0] = atkHint[0].localPosition.x;
        hintOffset[1] = atkHint[1].localPosition.x;
        hurtArea = t.Find("HurtArea");
        hurtAreaOffset = hurtArea.localPosition.x;
        hurtAreaCol = hurtArea.GetComponent<Collider>();
        maxHp = info.hp;
        hp = maxHp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
        minMoney = info.minMoney;
        maxMoney = info.maxMoney;

        attackType = Random.Range(0, 2);
        sightDist = sightDists[attackType];
        atkDist = atkDists[attackType];
        //playerManager = pManager;
    }


    public void Spawn(Vector3 pos, int col)
    {
        if (col <= 0)
        {
            col = Random.Range(1, 6);
        }
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, spawnHeight, pos.z);
        selfPos = transform.position;
        renderer.material.SetInt("_colorID", col);
        color = col;
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        if (Input.GetKeyDown(KeyCode.S)) ResetUnit();

        deltaTime = dt;
        selfPos = transform.position;

        if (hp > 0) {
            nearstPlayerDist = 500.0f;
            for (int i = 0; i < 4; i++)
            {
                if (goblinManager.PlayersDie[i]) continue;
                playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
                if (playerDist[i] < nearstPlayerDist)
                {
                    nearstPlayerDist = playerDist[i];
                    targetPlayer = i;
                }
                //if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
            }
        }
        
        //if(hp > 0)DetectGethurt();  //傷害判定
        StateMachine();
    }

    //public override void DetectGethurt()
    //{
    //    float offset = Mathf.Sign(image.localScale.x);
    //    Collider[] colliders = Physics.OverlapBox((selfPos + new Vector3(offset*0.7f,-1.55f,.0f)), new Vector3(1.5f, 3.1f, 0.1f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("DamageToGoblin"));
    //    int i = 0;
    //    while (i < colliders.Length)
    //    {
    //        hp--;
    //        if (hp <= 0) {
    //            SetState(GoblinState.die);
    //            break;
    //        }
    //        if (i == 0 && curState != GoblinState.hurt && !endure)
    //        {
    //            SetState(GoblinState.hurt);
    //            moveFwdDir = -Vector3.forward;
    //        }
    //        //Debug.Log(colliders[i].name);
    //        i++;
    //    }
    //}


    public override void MoveIn()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            firstInState = false;
            moveFwdDir = new Vector3(-selfPos.x, 0, -selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
            hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset,0,0);
        }
        else
        {
            transform.position += deltaTime * speed * moveFwdDir;
            if (inStateTime > 6.5f) SetState(GoblinState.ramble);
        }
    }

    public override void Ramble()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            firstInState = false;
            totalTime = Random.Range(1.0f, 3.0f);
            Vector3 playerPos = goblinManager.PlayerPos[Random.Range(0, 3)] + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
            if (Mathf.Abs(playerPos.x) > 70) playerPos.x = Mathf.Sign(playerPos.x) * 70.0f;
            if (Mathf.Abs(playerPos.y) > 70) playerPos.y = Mathf.Sign(playerPos.y) * 70.0f;
            moveFwdDir = new Vector3(playerPos.x - selfPos.x, 0, playerPos.z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
            hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
        }
        else
        {
            if (inStateTime < totalTime)
            {
                if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, 1 << LayerMask.NameToLayer("Barrier")))
                {
                    float degree = Random.Range(135.0f, 225.0f);
                    moveFwdDir = Quaternion.AngleAxis(degree, Vector3.up) * moveFwdDir;
                    float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                    image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                    hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                }
                transform.position += deltaTime * speed * moveFwdDir;
            }
            else if(!followingPath) SetState();
        }
    }
    public override void Chase()
    {
        if (firstInState)
        {
            startFindPath = false;
            animator.SetInteger("state", 1);
            animator.speed = 1.2f;
            firstInState = false;
            blankTime = .0f;
            //followingPath = false;
            //PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
        }
        else
        {
            if (inStateTime < 0.3f)
            {
                inStateTime += deltaTime;
            }
            else
            {
                if (goblinManager.PlayersMove[targetPlayer])
                {
                    //PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);

                    CalculatePath();
                    inStateTime = 0.0f;
                }
            }


            Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
            if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                {
                    followingPath = false;
                    SetState(GoblinState.attackBreak);
                    //OverAttackDetectDist();
                }
                else
                {
                    pathIndex++;
                    moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                    float scaleX = 1.0f;
                    if (moveFwdDir.x > 0.3f) scaleX = -1.0f;
                    else if (moveFwdDir.x < -0.3f) scaleX = 1.0f;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                    image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                    hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                }
            }
            transform.position += deltaTime * speed * 1.2f * moveFwdDir;

            //if (followingPath){ }
        }
    }

    public override void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            if (curState == GoblinState.idle || curState == GoblinState.ramble || curState == GoblinState.chase || curState == GoblinState.attackBreak) {
                path = new PathFinder.Path(waypoints, selfPos, turnDist);
                followingPath = true;
                pathIndex = 0;
                moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                SetState(GoblinState.chase);
            }
        }
        else
        {
            SetState(GoblinState.erroeCatch);
        }
    }

    void RandomATKType() {
        attackType = (Random.Range(0, 100) < 50)?0:1;
        sightDist = sightDists[attackType];
        atkDist = atkDists[attackType];
    }

    public override void Attack()
    {
        if (firstInState || !startAttack)
        {
            float scaleX = .0f;
            if (firstInState) {
                if (curPathRequest != null)
                {
                    PathRequestManager.CancleRequest(curPathRequest);
                    curPathRequest = null;
                }

                blankTime = .0f;

                moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z);
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);

                firstInState = false;
                if (attackType == 0)//Random.Range(0, 100) < 70
                { //槌擊
                    if (moveFwdDir.z > -0.3f || moveFwdDir.z < -1.0f)
                    {
                        animator.speed = 1.2f;
                        animator.SetInteger("state", 1);
                    }
                    else {
                        image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                        image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                        hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                        atkCol[0].localPosition = new Vector3(scaleX * atkColOffset[0], atkCol[0].localPosition.y, atkCol[0].localPosition.z);
                        atkCol[1].localPosition = new Vector3(scaleX * atkColOffset[1], atkCol[1].localPosition.y, atkCol[1].localPosition.z);
                        atkHint[0].localPosition = new Vector3(scaleX * hintOffset[0], atkHint[0].localPosition.y, atkHint[0].localPosition.z);
                        startAttack = true;
                        animator.speed = 1.0f;
                        animator.SetInteger("attackType", attackType);
                        animator.SetInteger("state", 2);
                        //animator.SetTrigger("attackOver");
                        endure = true;
                    }
                }
                else {//葉子
                    //attackType = 1;
                    startAttack = true;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                    image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                    hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                    atkCol[2].localPosition = new Vector3(scaleX * atkColOffset[2], atkCol[2].localPosition.y, atkCol[2].localPosition.z);
                    atkHint[1].localScale = new Vector3(scaleX * 1.54f, atkHint[1].localScale.y, atkHint[1].localScale.z);
                    atkHint[1].localPosition = new Vector3(scaleX * hintOffset[1], atkHint[1].localPosition.y, atkHint[1].localPosition.z);
                    launchPos = selfPos + new Vector3(scaleX * shootOffset.x, shootOffset.y, shootOffset.z);
                    shootFace = new Vector3(-scaleX, 0, 0);

                    animator.speed = 1.0f;
                    animator.SetInteger("attackType", attackType);
                    animator.SetInteger("state", 2);
                    //animator.SetTrigger("attackOver");
                    endure = true;
                }
            }

            if (!startAttack)
            {
                moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z);
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                if (moveFwdDir.sqrMagnitude > atkDist * atkDist) //Mathf.Abs(moveFwdDir.z) > 1.2f && 
                {
                    SetState(GoblinState.attackBreak);
                    //image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                    //image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                    //moveFwdDir = (moveFwdDir + new Vector3(-scaleX * 3.5f, 0, 0)).normalized;
                    //transform.position += speed * 1.2f * deltaTime * moveFwdDir;
                }
                else
                {
                    if (moveFwdDir.z > -0.3f)
                    {
                        moveFwdDir = new Vector3(0, 0, 1.0f);
                        transform.position += speed * 1.2f * deltaTime * moveFwdDir;
                    }
                    else if (moveFwdDir.z < -1.0f) {
                        moveFwdDir = new Vector3(0, 0,-1.0f);
                        transform.position += speed * 1.2f * deltaTime * moveFwdDir;
                    }
                    else
                    {
                        image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                        image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                        hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                        atkCol[0].localPosition = new Vector3(scaleX * atkColOffset[0], atkCol[0].localPosition.y, atkCol[0].localPosition.z);
                        atkCol[1].localPosition = new Vector3(scaleX * atkColOffset[1], atkCol[1].localPosition.y, atkCol[1].localPosition.z);
                        atkHint[0].localPosition = new Vector3(scaleX * hintOffset[0], atkHint[0].localPosition.y, atkHint[0].localPosition.z);
                        startAttack = true;
                        endure = true;
                        animator.speed = 1.0f;
                        animator.SetInteger("attackType", attackType);
                        animator.SetInteger("state", 2);
                        //animator.SetTrigger("attackOver");
                    }
                }

                //    moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z);
                //    scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                //    if (moveFwdDir.sqrMagnitude > 16.0f) //Mathf.Abs(moveFwdDir.z) > 1.2f && 
                //    {
                //        image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                //        image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                //        if (Mathf.Abs(moveFwdDir.x) < 3.0f) moveFwdDir = new Vector3(0, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
                //        else moveFwdDir = (moveFwdDir + new Vector3(-scaleX * 3.5f, 0, 0)).normalized;
                //        transform.position += speed * 1.2f * deltaTime * moveFwdDir;
                //    }
                //    else
                //    {

                //        atkCol[0].localPosition = new Vector3(scaleX * atkColOffset[0], atkCol[0].localPosition.y, atkCol[0].localPosition.z);
                //        atkCol[1].localPosition = new Vector3(scaleX * atkColOffset[1], atkCol[1].localPosition.y, atkCol[1].localPosition.z);
                //        startAttack = true;
                //        endure = true;
                //        animator.speed = 1.0f;
                //        animator.SetInteger("attackType", attackType);
                //        animator.SetInteger("state", 2);
                //        animator.SetTrigger("attackOver");
                //    }
                //}
            }
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if ((aniInfo.IsName("quackAttack") && attackType == 0) || (aniInfo.IsName("leafAttack") && attackType == 1))
            {

                if (attackType == 0)
                {
                    if (quackStep == 0 && aniInfo.normalizedTime > 0.45f)
                    {
                        quackStep++;
                        AudioManager.SingletonInScene.PlaySound2D("Tree_Attack", 0.38f);
                        
                    }

                    else if (quackStep == 1 && aniInfo.normalizedTime > 0.7f) {
                        quackStep++;
                        MultiPlayerCamera.CamerashakingSingleton.StartShakeEasyOut(0.15f, 0.15f, 0.45f);
                    }
                }
                else {
                    if ( !hasShoot && aniInfo.normalizedTime >= 0.47f)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Leaf_Attack", 0.35f);
                        hasShoot = true;
                        goblinManager.UseLeaf(launchPos + shootFace * 1.5f, shootFace);
                        for (int i = 0; i <= 1; i++)
                        {
                            Vector3 shootDir = Quaternion.AngleAxis(25.0f + i * 30, Vector3.up) * shootFace;
                            goblinManager.UseLeaf(launchPos + shootDir * 1.5f, shootDir);

                            shootDir = Quaternion.AngleAxis(-25.0f - i * 30, Vector3.up) * shootFace;
                            goblinManager.UseLeaf(launchPos + shootDir * 1.5f, shootDir);
                        }
                    }
                }

                if (aniInfo.normalizedTime >= 0.95f)
                {
                    endure = false;
                    startAttack = false;
                    hasShoot = false;
                    quackStep = 0;
                    RandomATKType();
                    SetState(GoblinState.attackBreak);
                    //OverAttackDetectDist();
                }
            }
        }
    }

    public override void OnGettingHurt(int col, int atkValue, int playerID,Vector3 dir)
    {
        if (hp <= 0) return;
        AudioManager.SingletonInScene.PlayRandomWrong(1f);
        targetPlayer = playerID;
        targetPlayer2 = playerID;
        if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt && !endure)
        {
            hurtDir = dir.normalized;
            SetState(GoblinState.fakeHurt);
        }

        //if (col == 1 || col == 2 || col == 4)
        //{
        //    if (col == color)
        //    {
        //        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //        hp -= atkValue;
        //        targetPlayer = playerID;
        //        targetPlayer2 = playerID;
        //        if (hp > 0)
        //        {
        //            if (curState != GoblinState.hurt && !endure)
        //            {
        //                hurtDir = dir.normalized;
        //                SetState(GoblinState.hurt);
        //            }
        //        }
        //        else
        //        {
        //            hurtDir = dir.normalized;
        //            SetState(GoblinState.die);
        //        }
        //    }
        //    else {
        //        AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //        targetPlayer = playerID;
        //        targetPlayer2 = playerID;
        //        if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt && !endure)
        //        {
        //            hurtDir = dir.normalized;
        //            SetState(GoblinState.fakeHurt);
        //        }
        //    }

        //}
        //else
        //{
        //    if (col == 3)
        //    {
        //        if ((color == 1 || color == 2 || color == 3))
        //        {
        //            if (hp > 0)
        //            {
        //                AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //                hp -= atkValue;
        //                targetPlayer = playerID;
        //                targetPlayer2 = playerID;
        //                if (curState != GoblinState.hurt && !endure)
        //                {
        //                    hurtDir = dir.normalized;
        //                    SetState(GoblinState.hurt);
        //                }
        //                else if (hp <= 0) SetState(GoblinState.die);
        //            }
        //        }
        //        else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //    }
        //    else if (col == 5)
        //    {
        //        if ((color == 1 || color == 4 || color == 5))
        //        {
        //            if (hp > 0)
        //            {
        //                AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //                hp -= atkValue;
        //                targetPlayer = playerID;
        //                targetPlayer2 = playerID;
        //                if (curState != GoblinState.hurt && !endure)
        //                {
        //                    hurtDir = dir.normalized;
        //                    SetState(GoblinState.hurt);
        //                }
        //                else if (hp <= 0) SetState(GoblinState.die);
        //            }
        //        }
        //        else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //    }
        //    else if (col == 6)
        //    {
        //        if ((color == 2 || color == 4 || color == 6))
        //        {
        //            if (hp > 0)
        //            {
        //                AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //                hp -= atkValue;
        //                targetPlayer = playerID;
        //                targetPlayer2 = playerID;
        //                if (curState != GoblinState.hurt && !endure)
        //                {
        //                    hurtDir = dir.normalized;
        //                    SetState(GoblinState.hurt);
        //                }
        //                else if (hp <= 0) SetState(GoblinState.die);
        //            }
        //        }
        //        else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //    }
        //}
    }

    public override void OnGettingHurt(int col, int atkValue, int playerID, int playerID2, Vector3 dir)
    {
        if (hp <= 0) return;
        targetPlayer = playerID;
        targetPlayer2 = playerID2;
        //if (col == 1 || col == 2 || col == 4)
        //{
        //    if (col == color)
        //    {
        //        if (hp > 0)
        //        {
        //            AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //            hp -= atkValue;
        //            targetPlayer = playerID;
        //            targetPlayer2 = playerID2;
        //            if (curState != GoblinState.hurt && !endure)
        //            {
        //                moveFwdDir = dir.normalized;
        //                SetState(GoblinState.hurt);
        //            }
        //            else if (hp <= 0) SetState(GoblinState.die);
        //        }
        //    }
        //    else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //}
        if (col == 3 || col == 5 || col == 6)//else
        {
            if (col == 3)
            {
                if ((color == 1 || color == 2 || color == 3))
                {
                    AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
                    hp -= atkValue;
                    whiteScale = 1.0f;
                    targetPlayer = playerID;
                    targetPlayer2 = playerID2;
                    if (hp > 0)
                    {
                        if (curState != GoblinState.hurt && !endure)
                        {
                            hurtDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                    }
                    else {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.die);
                    }
                }
                else
                {
                    AudioManager.SingletonInScene.PlayRandomWrong(1f);
                    targetPlayer = playerID;
                    targetPlayer2 = playerID2;
                    if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt && !endure)
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.fakeHurt);
                    }
                }
            }
            else if (col == 5)
            {
                if ((color == 1 || color == 4 || color == 5))
                {
                    AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
                    hp -= atkValue;
                    whiteScale = 1.0f;
                    targetPlayer = playerID;
                    targetPlayer2 = playerID2;
                    if (hp > 0)
                    {
                        if (curState != GoblinState.hurt && !endure)
                        {
                            hurtDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                    }
                    else
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.die);
                    }
                }
                else {
                    AudioManager.SingletonInScene.PlayRandomWrong(1f);
                    targetPlayer = playerID;
                    targetPlayer2 = playerID2;
                    if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt && !endure)
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.fakeHurt);
                    }
                }
            }
            else if (col == 6)
            {
                if ((color == 2 || color == 4 || color == 6))
                {
                    AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
                    hp -= atkValue;
                    whiteScale = 1.0f;
                    targetPlayer = playerID;
                    targetPlayer2 = playerID2;
                    if (hp > 0)
                    {
                        if (curState != GoblinState.hurt && !endure)
                        {
                            hurtDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                    }
                    else
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.die);
                    }
                }
                else{
                    AudioManager.SingletonInScene.PlayRandomWrong(1f);
                    targetPlayer = playerID;
                    targetPlayer2 = playerID2;
                    if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt && !endure)
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.fakeHurt);
                    }
                }
            }
        }
    }

    public override void Die()
    {
        if (firstInState)
        {
            if (curPathRequest != null)
            {
                PathRequestManager.CancleRequest(curPathRequest);
                curPathRequest = null;
            }

            hurtAreaCol.enabled = false;
            atkCollider[0].enabled = false;
            atkCollider[1].enabled = false;
            atkCollider[2].enabled = false;
            AudioManager.SingletonInScene.PlaySound2D("Hob_Death", 0.5f);
            animator.speed = 1.0f;
            animator.SetTrigger("die");
            animator.SetInteger("state", 4);
            AudioManager.SingletonInScene.PlaySound2D("Drop_Money", 0.5f);
            if (targetPlayer == targetPlayer2) goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer);
            else goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer, targetPlayer2);
            firstInState = false;
            whiteScale = 1.0f;
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (whiteScale > .0f)
            {
                whiteScale -= deltaTime * 8.0f;
                renderer.material.SetFloat("_WhiteScale", whiteScale);
            }
            if (aniInfo.IsName("die") && aniInfo.normalizedTime >= 0.95f)
            {
                whiteScale = -1.0f;
                ResetUnit();
            }
        }
    }

    

    public override void ErroeCatch()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            animator.speed = 1f;
            firstInState = false;
            moveFwdDir = new Vector3(0 - selfPos.x, 0, 0 - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
            hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
        }
        else
        {
            transform.position += deltaTime * speed * moveFwdDir;
            if (inStateTime > 1.5f) SetState();
        }
    }

    public void ResetUnit()
    {
        hp = maxHp;
        firstInState = false;
        inStateTime = .0f;
        SetState(GoblinState.moveIn);
        hasShoot = false;
        endure = false;
        startAttack = false;
        quackStep = 0;
        hurtAreaCol.enabled = true;
        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}
