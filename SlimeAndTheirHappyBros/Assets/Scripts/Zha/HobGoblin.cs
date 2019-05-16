using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobGoblin : GoblinBase, IEnemyUnit
{
    bool startAttack = false, endure = false;
    int maxHp;
    int attackType = 0;
    float imgOffset;
    float[] atkColOffset = new float[2];
    Transform[] atkCol = new Transform[2];


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        atkCol[0] = t.Find("AtkCollider");
        atkCol[1] = t.Find("AtkCollider (1)");
        imgScale = image.localScale.x;
        imgOffset = image.localPosition.x;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
        maxHp = info.hp;
        hp = maxHp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
        
    }

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        atkCol[0] = t.Find("AtkCollider");
        atkCol[1] = t.Find("AtkCollider (1)");
        imgScale = image.localScale.x;
        imgOffset = image.localPosition.x;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
        maxHp = info.hp;
        hp = maxHp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
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

        nearstPlayerDist = 500.0f;
        for (int i = 0; i < 4; i++)
        {
            playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
            if (playerDist[i] < nearstPlayerDist)
            {
                nearstPlayerDist = playerDist[i];
                targetPlayer = i;
            }
            //if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
        }
        if(hp > 0) DetectGethurt();
        StateMachine();
    }

    public override void DetectGethurt()
    {
        Collider[] colliders = Physics.OverlapBox(image.position, new Vector3(0.4f, 2.9f, 0.1f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("DamageToGoblin"));
        int i = 0;
        while (i < colliders.Length)
        {
            hp--;
            if (i == 0 && curState != GoblinState.hurt && !endure)
            {
                SetState(GoblinState.hurt);
                moveFwdDir = -Vector3.forward;
            }
            //Debug.Log(colliders[i].name);
            i++;
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
        }
        else
        {
            if (inStateTime < totalTime)
            {
                if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))
                {
                    float degree = Random.Range(135.0f, 225.0f);
                    moveFwdDir = Quaternion.AngleAxis(degree, Vector3.up) * moveFwdDir;
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
            animator.speed = 2.0f;
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
                    }
                }
                transform.position += deltaTime * speed * 2.0f * moveFwdDir;

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

                firstInState = false;
                if (false)//Random.Range(0, 100) < 70
                { //槌擊
                    animator.speed = 2.0f;
                    animator.SetInteger("state", 1);
                    attackType = 0;
                }
                else {//葉子
                    attackType = 1;
                    startAttack = true;
                    atkCol[0].localPosition = new Vector3(scaleX * atkColOffset[0], atkCol[0].localPosition.y, atkCol[0].localPosition.z);

                    animator.speed = 1.0f;
                    animator.SetInteger("attackType", attackType);
                    animator.SetInteger("state", 2);
                    endure = true;
                }
            }

            moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z);
            if (Mathf.Abs(moveFwdDir.z) > 1.2f)
            {
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                image.localPosition = new Vector3(scaleX * imgOffset, 0, 0);

                if (Mathf.Abs(moveFwdDir.x) < 5.0f) moveFwdDir = new Vector3(0, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
                else moveFwdDir = (moveFwdDir + new Vector3(-scaleX * 4.0f, 0, 0)).normalized;
                transform.position += speed*2.0f * deltaTime * moveFwdDir;
            }
            else {
                startAttack = true;
                endure = true;
                animator.SetInteger("attackType", attackType);
                animator.SetInteger("state", 2);
            }
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsTag("attack"))
            {
                if (attackType == 1 && aniInfo.normalizedTime >= 0.47f) {

                }
                if (aniInfo.normalizedTime >= 0.99f)
                {
                    endure = false;
                    startAttack = false;
                    animator.SetTrigger("attackOver");
                    OverAttackDetectDist();
                }
            }
        }
    }

    public override void Die()
    {
        if (firstInState)
        {
            animator.speed = 1.0f;
            animator.SetInteger("state", 4);
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

    public void ResetUnit()
    {
        hp = maxHp;
        firstInState = false;
        inStateTime = .0f;
        curState = GoblinState.moveIn;

        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}
