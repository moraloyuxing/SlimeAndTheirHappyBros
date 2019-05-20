﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobGoblin : GoblinBase, IEnemyUnit
{
    bool startAttack = false, endure = false, hasShoot = false;
    bool quackSound = false;
    int maxHp;
    int attackType = 0;
    float imgOffset, hurtAreaOffset;
    Vector3 shootOffset;
    Vector3 launchPos, shootFace;
    float[] atkColOffset = new float[3];
    Transform[] atkCol = new Transform[3];
    Transform hurtArea;

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
        imgScale = image.localScale.x;
        imgOffset = image.localPosition.x;
        shootOffset = t.Find("ShootPos").localPosition;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
        atkColOffset[2] = atkCol[2].localPosition.x;
        hurtArea = t.Find("HurtArea");
        hurtAreaOffset = hurtArea.localPosition.x;
        maxHp = info.hp;
        hp = maxHp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
        minMoney = info.minMoney;
        maxMoney = info.maxMoney;
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
        imgScale = image.localScale.x;
        imgOffset = image.localPosition.x;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
        atkColOffset[2] = atkCol[2].localPosition.x;
        hurtArea = t.Find("HurtArea");
        hurtAreaOffset = hurtArea.localPosition.x;
        maxHp = info.hp;
        hp = maxHp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
        minMoney = info.minMoney;
        maxMoney = info.maxMoney;
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
            if (inStateTime > 1.0f) SetState(GoblinState.ramble);
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
                if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))
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
            else SetState();
        }
    }
    public override void Chase()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            animator.speed = 1.2f;
            firstInState = false;
            followingPath = false;
            PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
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
                    Debug.Log("request path find");
                    PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
                    inStateTime = 0.0f;
                }
            }
            if (followingPath)
            {
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                    {
                        Debug.Log("reach path goal");
                        followingPath = false;
                        SetState(GoblinState.attack);
                    }
                    else
                    {
                        pathIndex++;
                        moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                        float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                        image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                        image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                        hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);
                    }
                }
                transform.position += deltaTime * speed * 1.2f * moveFwdDir;

            }
        }
    }

    public override void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {

            path = new PathFinder.Path(waypoints, selfPos, turnDist);
            followingPath = true;
            pathIndex = 0;
            moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
            hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);

            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
        }
        else
        {
            Debug.Log("Can't Find");
        }
    }

    public override void Attack()
    {
        if (firstInState || !startAttack)
        {
            float scaleX = .0f;
            if (firstInState) {

                moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                hurtArea.localPosition = new Vector3(scaleX * hurtAreaOffset, 0, 0);

                firstInState = false;
                if (attackType == 1)//Random.Range(0, 100) < 70
                { //槌擊
                    animator.speed = 1.2f;
                    animator.SetInteger("state", 1);
                    attackType = 0;
                }
                else {//葉子
                    attackType = 1;
                    startAttack = true;
                    atkCol[2].localPosition = new Vector3(scaleX * atkColOffset[2], atkCol[2].localPosition.y, atkCol[2].localPosition.z);

                    launchPos = selfPos + new Vector3(scaleX * shootOffset.x, shootOffset.y, shootOffset.z);
                    shootFace = new Vector3(-scaleX, 0, 0);

                    animator.speed = 1.0f;
                    animator.SetInteger("attackType", attackType);
                    animator.SetInteger("state", 2);
                    animator.SetTrigger("attackOver");
                    endure = true;
                }
            }

            if (!startAttack) {
                moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z);
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                if (moveFwdDir.sqrMagnitude > 16.0f) //Mathf.Abs(moveFwdDir.z) > 1.2f && 
                {
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                    image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);
                    if (Mathf.Abs(moveFwdDir.x) < 3.0f) moveFwdDir = new Vector3(0, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
                    else moveFwdDir = (moveFwdDir + new Vector3(-scaleX * 3.5f, 0, 0)).normalized;
                    transform.position += speed * 1.2f * deltaTime * moveFwdDir;
                }
                else
                {

                    atkCol[0].localPosition = new Vector3(scaleX * atkColOffset[0], atkCol[0].localPosition.y, atkCol[0].localPosition.z);
                    atkCol[1].localPosition = new Vector3(scaleX * atkColOffset[1], atkCol[1].localPosition.y, atkCol[1].localPosition.z);
                    startAttack = true;
                    endure = true;
                    animator.speed = 1.0f;
                    animator.SetInteger("attackType", attackType);
                    animator.SetInteger("state", 2);
                    animator.SetTrigger("attackOver");
                }
            }
            
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if ((aniInfo.IsName("quackAttack") && attackType == 0) || (aniInfo.IsName("leafAttack") && attackType == 1))
            {

                if (attackType == 0)
                {
                    if (!quackSound && aniInfo.normalizedTime > 0.6f)
                    {
                        quackSound = true;
                        AudioManager.SingletonInScene.PlaySound2D("Tree_Attack", 0.38f);
                    }
                }
                else {
                    if ( !hasShoot && aniInfo.normalizedTime >= 0.47f)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Leaf_Attack", 0.35f);
                        hasShoot = true;
                        float scaleX = (goblinManager.PlayerPos[targetPlayer].x > selfPos.x) ? -1.0f : 1.0f;
                        for (int i = 0; i <= 1; i++)
                        {
                            Vector3 shootDir = Quaternion.AngleAxis(25.0f + i * 30, Vector3.up) * shootFace;
                            goblinManager.UseLeaf(launchPos + shootDir * 1.5f, shootDir);

                            shootDir = Quaternion.AngleAxis(-25.0f - i * 30, Vector3.up) * shootFace;
                            goblinManager.UseLeaf(launchPos + shootDir * 1.5f, shootDir);
                        }
                    }
                }

                if (aniInfo.normalizedTime >= 0.99f)
                {
                    endure = false;
                    startAttack = false;
                    hasShoot = false;
                    quackSound = false;
                    OverAttackDetectDist();
                }
            }
        }
    }

    public override void OnGettingHurt(int col, int atkValue, int playerID,Vector3 dir)
    {
        if (col == 1 || col == 2 || col == 4)
        {
            if (col == color)
            {
                if (hp > 0)
                {
                    AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                    hp -= atkValue;
                    targetPlayer = playerID;
                    if (curState != GoblinState.hurt && !endure)
                    {
                        moveFwdDir = dir.normalized;
                        SetState(GoblinState.hurt);
                    }
                    else if (hp <= 0) SetState(GoblinState.die);
                }
            }
            else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        }
        else
        {
            if (col == 3)
            {
                if ((color == 1 || color == 2 || color == 3))
                {
                    if (hp > 0)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                        hp -= atkValue;
                        targetPlayer = playerID;
                        if (curState != GoblinState.hurt && !endure)
                        {
                            moveFwdDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                        else if (hp <= 0) SetState(GoblinState.die);
                    }
                }
                else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
            }
            else if (col == 5)
            {
                if ((color == 1 || color == 4 || color == 5))
                {
                    if (hp > 0)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                        hp -= atkValue;
                        targetPlayer = playerID;
                        if (curState != GoblinState.hurt && !endure)
                        {
                            moveFwdDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                        else if (hp <= 0) SetState(GoblinState.die);
                    }
                }
                else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
            }
            else if (col == 6)
            {
                if ((color == 2 || color == 4 || color == 6))
                {
                    if (hp > 0)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                        hp -= atkValue;
                        targetPlayer = playerID;
                        if (curState != GoblinState.hurt && !endure)
                        {
                            moveFwdDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                        else if (hp <= 0) SetState(GoblinState.die);
                    }
                }
                else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
            }
        }
    }

    public override void Die()
    {
        if (firstInState)
        {
            AudioManager.SingletonInScene.PlaySound2D("Hob_Death", 0.5f);
            animator.speed = 1.0f;
            animator.SetInteger("state", 4);
            AudioManager.SingletonInScene.PlaySound2D("Drop_Money", 0.5f);
            goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer);
            firstInState = false;
            
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("die") && aniInfo.normalizedTime >= 0.99f)
            {
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
            if (inStateTime > 1.0f) SetState();
        }
    }

    public void ResetUnit()
    {
        hp = maxHp;
        firstInState = false;
        inStateTime = .0f;
        curState = GoblinState.moveIn;
        hasShoot = false;
        endure = false;
        startAttack = false;
        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}
