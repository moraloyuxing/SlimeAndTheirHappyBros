using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class ArcherGoblin : GoblinBase, IEnemyUnit
{
    bool hasShoot = false;
    int maxHp;
    int delayShoot = 0;
    float scaleX;
    Vector3 shootPos;
    Transform shootLauncher;

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
    }

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        shootLauncher = t.Find("shootPos");
        imgScale = image.localScale.x;
        hp = info.hp;
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

        deltaTime = Time.deltaTime;
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

            if (delayShoot == 0) {
                //Debug.Log("sssstart attack ");
                moveFwdDir = (goblinManager.PlayerPos[targetPlayer] - selfPos).normalized;
                scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                //Debug.Log("scale   " + scaleX);
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                int dir = 0;
                if (moveFwdDir.z < -0.6f) dir = 0;
                else if (moveFwdDir.z > 0.6f) dir = 2;
                else dir = 1;

                animator.speed = 1.0f;
                animator.SetInteger("shootDir", dir);
                animator.SetInteger("state", 2);
                animator.SetTrigger("attackOver");
            } 
            else if (delayShoot == 2) {
                //Debug.Log("first oooover ");
                shootPos = new Vector3(scaleX * shootLauncher.localPosition.x, shootLauncher.localPosition.y, shootLauncher.localPosition.z);
                moveFwdDir = goblinManager.PlayerPos[targetPlayer] - (selfPos + shootPos);
                hasShoot = false;
                firstInState = false;

            }
            //Debug.Log("delayyyyy plus");
            delayShoot++;
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsTag("attack"))
            {

                if (!hasShoot && aniInfo.normalizedTime > 0.64f) {
                    //Debug.Log("start sssssssssssshooot");
                    hasShoot = true;
                    goblinManager.UseArrow(transform.position + shootPos, moveFwdDir);
                }
                if (aniInfo.normalizedTime >= 0.99f)
                {
                    //Debug.Log("verrrrrr attack");
                    delayShoot = 0;
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
        hasShoot = false;
        inStateTime = .0f;
        curState = GoblinState.moveIn;

        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}
