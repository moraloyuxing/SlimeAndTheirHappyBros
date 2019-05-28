using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class NormalGoblin: GoblinBase, IEnemyUnit
{
    int maxHp;
    float atkColOffset;
    Transform atkColTrans;
    Collider atkCol;

    //TestPlayerManager playerManager;
    //Player_Manager playerManager;

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager) {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        atkColTrans = t.Find("AtkCollider");
        atkCol = atkColTrans.GetComponent<Collider>();
        imgScale = image.localScale.x;
        atkColOffset =atkColTrans.localPosition.x;
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
        atkColTrans = t.Find("AtkCollider");
        atkCol = atkColTrans.GetComponent<Collider>();
        imgScale = image.localScale.x;
        atkColOffset = atkColTrans.localPosition.x;
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


    public void Spawn(Vector3 pos, int col)
    {
        if (col <= 0) {
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


    public override void Attack()
    {
        if (firstInState)
        {
            if (curPathRequest != null) PathRequestManager.CancleRequest(curPathRequest);

            AudioManager.SingletonInScene.PlaySound2D("Goblin_Attack", 0.18f);
            animator.SetInteger("state", 2);

            animator.speed = 1.0f;
            firstInState = false;

            moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            atkColTrans.localPosition = new Vector3(scaleX * atkColOffset, atkColTrans.localPosition.y, atkColTrans.localPosition.z);
        }
        else
        {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("attack"))
            {
                if (aniInfo.normalizedTime >= 0.46f && aniInfo.normalizedTime < 0.77f)
                {
                    //Debug.DrawRay(selfPos,moveFwdDir, Color.red, 3.0f);
                    float atkSpeed = (Physics.Raycast(selfPos, moveFwdDir, 3.0f, 1 << LayerMask.NameToLayer("Barrier"))) ? .0f : speed * 3.0f;
                    transform.position += deltaTime * atkSpeed * moveFwdDir;
                }
                else if (aniInfo.normalizedTime >= 0.99f)
                {
                    animator.SetTrigger("attackOver");
                    SetState(GoblinState.attackBreak);
                    //OverAttackDetectDist();
                }
            }
        }
    }


    public override void GetHurt()
    {
        if (firstInState)
        {
            if (curPathRequest != null) PathRequestManager.CancleRequest(curPathRequest);

            atkCol.enabled = false;
            firstInState = false;
            //animator.Play("hurt");
            animator.SetTrigger("hurt");
            animator.SetInteger("state", 3);
        }
        else
        {
            if (!Physics.Raycast(selfPos, moveFwdDir, 2.0f, LayerMask.NameToLayer("barrier"))) {
                transform.position += backSpeed * deltaTime * hurtDir;
            }
            backSpeed -= deltaTime * 15.0f;
            if (backSpeed <= .0f) backSpeed = .0f;
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (aniInfo.IsName("hurt"))Debug.Log(aniInfo.normalizedTime);
            if (aniInfo.IsName("hurt") && aniInfo.normalizedTime >= 0.99f)
            {
                Debug.Log("hurrrrt  over");
                if (hp <= 0) SetState(GoblinState.die);
                else SetState(GoblinState.attackBreak); //OverAttackDetectDist();
                backSpeed = 10.0f;

            }
        }
    }

    public override void Die()
    {
        if (firstInState)
        {
            if (curPathRequest != null) PathRequestManager.CancleRequest(curPathRequest);

            AudioManager.SingletonInScene.PlaySound2D("Goblin_Death", 0.26f);
            atkCol.enabled = false;
            animator.speed = 1.0f;
            animator.SetTrigger("die");
            //animator.SetInteger("state", 4);
            AudioManager.SingletonInScene.PlaySound2D("Drop_Money", 0.6f);
            
            if(targetPlayer == targetPlayer2) goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer);
            else goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer, targetPlayer2);
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

    public void ResetUnit() {
        hp = maxHp;
        firstInState = false;
        inStateTime = .0f;
        curState = GoblinState.moveIn;

        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}

