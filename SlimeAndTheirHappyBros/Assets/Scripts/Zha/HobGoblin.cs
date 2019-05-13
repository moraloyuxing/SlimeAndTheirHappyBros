using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobGoblin : GoblinBase, IEnemyUnit
{
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
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
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
        atkCol[0] = t.Find("AtkCollider");
        atkCol[1] = t.Find("AtkCollider (1)");
        imgScale = image.localScale.x;
        atkColOffset[0] = atkCol[0].localPosition.x;
        atkColOffset[1] = atkCol[1].localPosition.x;
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
        DetectGethurt();
        StateMachine();
    }

    public override void DetectGethurt()
    {
        Collider[] colliders = Physics.OverlapBox(image.position, new Vector3(0.4f, 2.9f, 0.1f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("DamageToGoblin"));
        int i = 0;
        while (i < colliders.Length)
        {
            if (i == 0 && curState != GoblinState.hurt && curState != GoblinState.attack)
            {
                SetState(GoblinState.hurt);
                moveFwdDir = -Vector3.forward;
            }
            //Debug.Log(colliders[i].name);
            i++;
        }
    }


    public override void Attack()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 2);

            animator.speed = 1.0f;
            firstInState = false;

            moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
            atkCol[0].localPosition = new Vector3(scaleX * atkColOffset[0], atkCol[0].localPosition.y, atkCol[0].localPosition.z);
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
                    OverAttackDetectDist();
                }
            }
        }
    }

    public void ResetUnit()
    {
        goblinManager.RecycleGoblin(this);
        transform.gameObject.SetActive(false);
    }
}
