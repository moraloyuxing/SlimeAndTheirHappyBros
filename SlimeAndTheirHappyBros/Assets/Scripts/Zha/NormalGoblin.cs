using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class NormalGoblin: GoblinBase, IEnemyUnit
{
    float atkColOffset, atkSpeedOffset = 1.0f;
    Transform atkColTrans;
    Collider atkCol, hurtAreaCol;
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
        hurtAreaCol = transform.Find("HurtArea").GetComponent<Collider>();
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
        hurtAreaCol = transform.Find("HurtArea").GetComponent<Collider>();
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
        //SetState(GoblinState.moveIn);
    }

    //lin新增_專屬教學模式產生的
    public void Spawn_forTutorial(Vector3 pos, int col,Sprite s){
        if (col <= 0){
            col = Random.Range(1, 6);
        }
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, spawnHeight, pos.z);
        selfPos = transform.position;
        renderer.material.SetInt("_colorID", col);
        color = col;

        SpriteRenderer _SR = transform.Find("ColorHint").GetComponent<SpriteRenderer>();
        _SR.sprite = s;
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        //if (Input.GetKeyDown(KeyCode.S)) ResetUnit();

        deltaTime = dt;
        selfPos = transform.position;
       
        //if(hp > 0)DetectGethurt();  //傷害判定
        StateMachine();
    }


    public override void Attack()
    {
        if (firstInState)
        {
            if (curPathRequest != null)
            {
                PathRequestManager.CancleRequest(curPathRequest);
                curPathRequest = null;
            }
            blankTime = .0f;

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
                if (aniInfo.normalizedTime >= 0.46f && aniInfo.normalizedTime < 0.7f)//0.77
                {

                    //Debug.DrawRay(selfPos,moveFwdDir, Color.red, 3.0f);
                    if (inStateTime > 0.7f) atkSpeedOffset *= 0.8f;
                    float atkSpeed = (Physics.Raycast(selfPos, moveFwdDir, 3.0f, 1 << LayerMask.NameToLayer("Barrier"))) ? .0f 
                        :65.0f*atkSpeedOffset;
                    transform.position += deltaTime * atkSpeed * moveFwdDir;
                }
                else if (aniInfo.normalizedTime >= 0.95f)
                {
                    atkSpeedOffset = 1.0f;
                    //animator.SetTrigger("attackOver");
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
            if (curPathRequest != null)
            {
                PathRequestManager.CancleRequest(curPathRequest);
                curPathRequest = null;
            }

            atkCol.enabled = false;
            atkSpeedOffset = 1.0f;

            firstInState = false;
            //animator.Play("hurt");
            animator.SetTrigger("hurt");
            animator.SetInteger("state", 3);
            whiteScale = 1.0f;
            renderer.material.SetFloat("_WhiteScale", 1.0f);
        }
        else
        {

            if (!Physics.Raycast(selfPos, hurtDir, 2.0f, 1 << LayerMask.NameToLayer("Barrier"))) {
                transform.position += backSpeed * deltaTime * hurtDir;
            }

            if (whiteScale > .0f)
            {
                whiteScale -= deltaTime * 8.0f;
                renderer.material.SetFloat("_WhiteScale", whiteScale);
            }

            backSpeed -= deltaTime * 15.0f;
            if (backSpeed <= .0f) backSpeed = .0f;
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (aniInfo.IsName("hurt"))Debug.Log(aniInfo.normalizedTime);
            if (aniInfo.IsName("hurt") && aniInfo.normalizedTime >= 0.95f)
            {
                //if (hp <= 0) SetState(GoblinState.die);
                SetState(GoblinState.attackBreak); //OverAttackDetectDist();
                backSpeed = 10.0f;
                //renderer.material.SetInt("_colorID", color);
                whiteScale = -1.0f;
                renderer.material.SetFloat("_WhiteScale", -1);
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

            if (G_Tutorial.During_Tutorial == true) {
                SpriteRenderer _SR = transform.Find("ColorHint").GetComponent<SpriteRenderer>();
                _SR.sprite = null;
            }

            AudioManager.SingletonInScene.PlaySound2D("Goblin_Death", 0.26f);
            atkCol.enabled = false;
            atkSpeedOffset = 1.0f;
            hurtAreaCol.enabled = false;
            animator.speed = 1.0f;
            animator.SetTrigger("die");
            //animator.Play("die");
            animator.SetInteger("state", 4);

            //lin新增_教學階段不掉錢
            if (G_Tutorial.During_Tutorial == false) {
                AudioManager.SingletonInScene.PlaySound2D("Drop_Money", 0.6f);
                if (targetPlayer == targetPlayer2) goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer);
                else goblinManager.UseMoney(Random.Range(minMoney, maxMoney), selfPos, targetPlayer, targetPlayer2);
            }

            firstInState = false;
            whiteScale = 1.0f;
        }
        else
        {
            if (whiteScale > .0f)
            {
                whiteScale -= deltaTime * 8.0f;
                renderer.material.SetFloat("_WhiteScale", whiteScale);
            }

            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("die") && aniInfo.normalizedTime >= 0.95f)
            {
                whiteScale = -1.0f;
                ResetUnit();
            }
        }
    }

    public void ResetUnit() {
        hp = maxHp;
        firstInState = false;
        inStateTime = .0f;
        SetState(GoblinState.moveIn);
        hurtAreaCol.enabled = true;
        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}

