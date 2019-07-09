using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class ArcherGoblin : GoblinBase, IEnemyUnit
{
    bool hasShoot = false;
    int delayShoot = 0;
    float scaleX;
    Vector3 shootPos;
    Transform shootLauncher;
    Collider hurtAreaCol;

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        shootLauncher = t.Find("shootPos");
        imgScale = image.localScale.x;
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
        hurtAreaCol = transform.Find("HurtArea").GetComponent<Collider>();
    }

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        shootLauncher = t.Find("shootPos");
        imgScale = image.localScale.x;
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
        hurtAreaCol = transform.Find("HurtArea").GetComponent<Collider>();
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

        deltaTime = Time.deltaTime;
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

    public override void SetState()
    {
        delayShoot = 0;
        inStateTime = 0.0f;
        firstInState = true;
        float opp = Random.Range(.0f, 10.0f);
        if (opp > 7.0f)
        {
            if (curState == GoblinState.idle) curState = GoblinState.ramble;
            else if (curState == GoblinState.ramble) curState = GoblinState.idle;
            else curState = GoblinState.idle;
        }
    }
    public override void SetState(GoblinState state)
    {
        delayShoot = 0;
        inStateTime = 0.0f;
        firstInState = true;
        curState = state;
    }


    public override void Attack()
    {


        if (firstInState )
        {
            if (curPathRequest != null)
            {
                PathRequestManager.CancleRequest(curPathRequest);
                curPathRequest = null;
            }

            blankTime = .0f;

            if (delayShoot == 0) {
                moveFwdDir = (goblinManager.PlayerPos[targetPlayer] - selfPos).normalized;
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                int dir = 0;
                if (moveFwdDir.z < -0.6f) dir = 0;
                else if (moveFwdDir.z > 0.6f) dir = 2;
                else dir = 1;

                animator.speed = 1.0f;
                animator.SetInteger("shootDir", dir);
                animator.SetInteger("state", 2);
                //animator.SetTrigger("attackOver");
            } 
            else if (delayShoot == 2) {
                shootPos = new Vector3(scaleX * shootLauncher.localPosition.x, shootLauncher.localPosition.y, shootLauncher.localPosition.z);
                moveFwdDir = goblinManager.PlayerPos[targetPlayer] - (selfPos + shootPos);
                hasShoot = false;
                firstInState = false;

            }
            delayShoot++;
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsTag("attack"))
            {
                if (!hasShoot && aniInfo.normalizedTime > 0.64f) {
                    hasShoot = true;
                    goblinManager.UseArrow(transform.position + shootPos, moveFwdDir + new Vector3(0,-0.5f,0));
                    AudioManager.SingletonInScene.PlaySound2D("Shoot_Bow", 0.75f);
                }
                if (aniInfo.normalizedTime >= 0.95f)
                {
                    delayShoot = 0;
                    SetState(GoblinState.attackBreak);
                    //OverAttackDetectDist();
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

            AudioManager.SingletonInScene.PlaySound2D("Goblin_Death", 0.26f);
            hurtAreaCol.enabled = false;
            animator.speed = 1.0f;
            animator.SetTrigger("die");
            animator.SetInteger("state", 4);
            AudioManager.SingletonInScene.PlaySound2D("Drop_Money", 0.6f);
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

    public void ResetUnit()
    {
        hp = maxHp;
        firstInState = false;
        hasShoot = false;
        inStateTime = .0f;
        SetState(GoblinState.moveIn);
        hurtAreaCol.enabled = true;
        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}
