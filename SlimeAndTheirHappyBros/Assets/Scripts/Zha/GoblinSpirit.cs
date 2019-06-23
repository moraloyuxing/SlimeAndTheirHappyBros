using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpirit:IEnemyUnit
{
    bool firstInState = false, floatBack = false, nextAtk = false;
    int posID = 1;
    float moveTime = .0f, idleTime = .0f;
    float deltaTime, stateTime = -4.0f, floatAngle = .0f, floatLength = 1.0f, floatTime = .0f, floatSpeed = 20.0f;
    Vector3 showUpPos = new Vector3(-1.65f, 2.9f, 33.0f); //18
    Vector3 backPos, goalPos;
    Vector3[] idlePos = new Vector3[2] { new Vector3(-25.0f, 17.5f, 17.8f), new Vector3(22.0f,17.5f,17.8f)};
    Vector3[] atkPos = new Vector3[3] { new Vector3(-25.3f,2.2f,17.8f), new Vector3(-1.65f, 2.2f, 17.8f), new Vector3(22.0f, 2.2f, 17.8f) };
    Transform transform;
    GoblinManager goblinManager;
    SpiritState curState = SpiritState.showUp;
    enum SpiritState {
        showUp, idle, move, circleAttack, xAttack, colorChange
    }


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager manager)
    {
        transform = t;
        goblinManager = manager;
        t.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;

    }
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager) {

    }
    public void Spawn(Vector3 pos, int col) {
        transform.gameObject.SetActive(true);

    }

    // Update is called once per frame
    public void Update(float dt)
    {
        deltaTime = dt;
        switch (curState) {
            case SpiritState.showUp:
                ShowUp();
                break;
            case SpiritState.idle:
                Idle();
                break;
            case SpiritState.move:
                Move();
                break;
            case SpiritState.circleAttack:
                CircleAttack();
                break;
            case SpiritState.xAttack:
                XAttack();
                break;
            case SpiritState.colorChange:
                break;
        }
    }

    void SetState(SpiritState state) {
        curState = state;
        firstInState = false;
        stateTime = .0f;
    }
    void MoveEndIdle() {
        idleTime = Random.Range(2.0f,5.0f);
        SetState(SpiritState.idle);
    }
    void IdleSetIdle() {
        floatAngle = .0f;
        moveTime = 2.0f;
        if (posID == 0) posID = 1;
        else posID = 0;
        goalPos = idlePos[posID];
        SetState(SpiritState.move);
    }
    void AtkSetIdle() {
        floatAngle = .0f;
        int nextPos = Random.Range(0, 99) % 2;
        if (posID == 0)
        {
            if (nextPos == 0) moveTime = 0.6f;
            else moveTime = 2.0f;
        }
        else if (posID == 1)
        {
            moveTime = 1.5f;
        }
        else {
            if (nextPos == 0) moveTime = 2.0f;
            else moveTime = 0.6f;
        }
        posID = nextPos;
        goalPos = idlePos[posID];
        SetState(SpiritState.move);
    }

    void SetAtk() {
        nextAtk = true;
        int nextPos = Random.Range(0, 99) % 3;
        if (posID == 0)
        {
            if (nextPos == 0) moveTime = 0.6f;
            else if (nextPos == 1) moveTime = 1.5f;
            else moveTime = 2.0f;
        }
        else if (posID == 1)
        {
            if (nextPos == 2) moveTime = 0.6f;
            else if (nextPos == 1) moveTime = 1.5f;
            else moveTime = 2.0f;
        }
        posID = nextPos;
        goalPos = atkPos[posID];
        SetState(SpiritState.move);
    }
    void MoveEndAttack() {
        nextAtk = false;
        if (Random.Range(0, 100) > 40) SetState(SpiritState.circleAttack);
        else SetState(SpiritState.xAttack);
    }

    void ShowUp() {
        stateTime += deltaTime;

        if (stateTime > .0f) {
            if (!firstInState && stateTime > 0.5f) {
                transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = true;
                firstInState = true;
            } 
            float z = Mathf.Lerp(33.0f, 18.0f, stateTime);
            transform.position = new Vector3(showUpPos.x, showUpPos.y, z);
            if (stateTime > 1.0) AtkSetIdle();
        }

    }

    void Idle() {
        stateTime += deltaTime;
        floatAngle += deltaTime * 2.0f;
        float scale = Mathf.Cos(floatAngle);
        Debug.Log("idle angle  " +  scale);
        transform.position += deltaTime * scale *  new Vector3(0, 4.0f, 0);
        if (stateTime > idleTime) {
            int op = Random.Range(0, 100);
            if (op < 10) SetState(SpiritState.idle);
            else if (op < 40) IdleSetIdle();
            else SetAtk();
        }
    }

    void Move() {
        stateTime += deltaTime;
        
        Vector3 dir = (goalPos - transform.position).normalized;
        Vector3 normal = new Vector3(-dir.y, dir.x, dir.z);
        if (stateTime < moveTime)
        {
            floatTime += deltaTime;
            if (floatTime > 0.5f)
            {
                floatTime = .0f;
                floatLength = Random.Range(0.0f, 1.5f);
                floatAngle = Random.Range(0, 180.0f);
            }
            floatAngle += deltaTime * 5.0f;
            float scale = floatLength * Mathf.Cos(floatAngle);
            transform.position += deltaTime * floatSpeed * (dir + scale * normal).normalized;

        }
        else {

            if (!floatBack) {
                floatBack = true;
                backPos = transform.position;
                floatTime = .0f;
            }
            floatTime += deltaTime * 2.0f;
            transform.position = Vector3.Lerp(backPos, goalPos, floatTime);
            if (floatTime >= 1.0f) {
                floatTime = .0f;
                floatBack = false;
                floatLength = 1.0f;
                if (posID <= 0) transform.localScale = new Vector3(1, 1, 1);
                else transform.localScale = new Vector3(-1, 1, 1);
                if (nextAtk) MoveEndAttack();
                else MoveEndIdle();
            }
        }
    }

    void CircleAttack() {
        stateTime += deltaTime;
        if (stateTime > 0.5f) AtkSetIdle();
    }
    void XAttack() {
        stateTime += deltaTime;
        if (stateTime > 0.5f) AtkSetIdle();
    }

    public void ResetUnit() {

    }

}
