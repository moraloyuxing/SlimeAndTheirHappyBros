using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class NormalGoblin: GoblinBase, IEnemyUnit
{
    float atkColOffset;
    Transform atkCol;

    //TestPlayerManager playerManager;
    //Player_Manager playerManager;

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager) {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        atkCol = t.Find("AtkCollider");
        imgScale = image.localScale.x;
        atkColOffset = atkCol.localPosition.x;
        hp = info.hp;
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
        atkCol = t.Find("AtkCollider");
        imgScale = image.localScale.x;
        atkColOffset = atkCol.localPosition.x;
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
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) ResetUnit();

        deltaTime = Time.deltaTime;
        selfPos = transform.position;

        nearstPlayerDist = 500.0f;
        for (int i = 0; i < 4; i++) {
            playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
            if (playerDist[i] < nearstPlayerDist)
            {
                nearstPlayerDist = playerDist[i];
                targetPlayer = i;
            }
            //if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
        }
        DetectGethurt();
        StateMachine();
    }


    public override void Attack()
    {
        if (firstInState)
        {
            Debug.Log("start attack");
            animator.SetInteger("state", 2);
            animator.speed = 1.0f;
            firstInState = false;

            moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            atkCol.localPosition = new Vector3(scaleX * atkColOffset, atkCol.localPosition.y, atkCol.localPosition.z);
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
                else if (aniInfo.normalizedTime >= 0.95f)
                {
                    OverAttackDetectDist();
                }
            }
        }
    }

    public void ResetUnit() {
        goblinManager.Recycle(this);
        transform.gameObject.SetActive(false);
    }
}

