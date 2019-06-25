using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpirit:IEnemyUnit
{
    bool firstInState = false, floatBack = false, nextAtk = false, goChangeColor = false, changeColorOnce = false;
    int posID = 1, targetPlayer = 0, deathCount = 0, attackCount = 0;
    int color = 0, colorChangeNum = 0;
    float moveTime = .0f, idleTime = 2.0f, changeColorTime = .0f;
    float deltaTime, stateTime = -4.0f, floatAngle = .0f, floatLength = 1.0f, floatTime = .0f, floatSpeed = 20.0f;
    Vector3 showUpPos = new Vector3(-1.65f, 2.9f, 33.0f); //18
    Vector3 backPos, goalPos, atkDir;
    Vector3[] idlePos = new Vector3[2] { new Vector3(-25.0f, 17.5f, 17.8f), new Vector3(22.0f,17.5f,17.8f)};
    Vector3[] atkPos = new Vector3[3] { new Vector3(-25.3f,2.2f,17.8f), new Vector3(-1.65f, 2.2f, 17.8f), new Vector3(22.0f, 2.2f, 17.8f) };
    Transform transform;
    SpriteRenderer render;
    KingGoblin kingGoblin;
    GoblinManager goblinManager;

    Animator animator;
    AnimatorStateInfo aniInfo;

    SpiritState curState = SpiritState.showUp;
    enum SpiritState {
        showUp, idle, move, circleAttack, xAttack, colorChange
    }


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager manager, KingGoblin king)
    {
        transform = t;
        goblinManager = manager;
        animator = transform.GetComponent<Animator>();
        render = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        t.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
        kingGoblin = king;
        kingGoblin.SubChangeColorCBK(SetColorState);

        int op = Random.Range(0,90);
        if (op < 30) color = 1;
        else if (op < 61) color = 2;
        else color = 4;
        render.material.SetInt("_colorID", color);
        kingGoblin.SetColor(color);
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
                ChangeColor();
                break;
        }
        if (!goChangeColor) {
            changeColorTime += deltaTime;
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
        animator.SetInteger("state", 0);
    }
    void MoveEndAttack() {
        nextAtk = false;
        //SetState(SpiritState.circleAttack);
        if (Random.Range(0, 100) > 40) SetState(SpiritState.circleAttack);
        else SetState(SpiritState.xAttack);
    }

    public void SetColorState() {
        SetState(SpiritState.colorChange);
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
        transform.position += deltaTime * scale *  new Vector3(0, 4.0f, 0);
        if (stateTime > idleTime) {

            if (!goChangeColor)
            {
                if (changeColorTime < 2.0f)
                {
                    Debug.Log("change color time  " + changeColorTime);
                    int op = Random.Range(0, 100);
                    if (op < 10) SetState(SpiritState.idle);
                    else if (op < 40) IdleSetIdle();
                    else SetAtk();
                }
                else
                {
                    changeColorTime = .0f;
                    if (Random.Range(0, 100) > 80)
                    {
                        if (kingGoblin.IsIdle())
                        {
                            goChangeColor = true;
                            colorChangeNum = 0;
                            kingGoblin.SetRoar();
                        }
                        else {
                            colorChangeNum++;
                            if (colorChangeNum >= 3)
                            {
                                goChangeColor = true;
                                colorChangeNum = 0;
                                kingGoblin.SetRoar();
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("too shame to change");
                        colorChangeNum++;
                        if (colorChangeNum >= 3)
                        {
                            goChangeColor = true;
                            colorChangeNum = 0;
                            kingGoblin.SetRoar();
                        }
                    }
                }
            }

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
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 1);
        }
        else {
            stateTime += deltaTime;
            if (attackCount == 0)
            {
                if (stateTime > 0.3f) {
                    atkDir = new Vector3(0, 0, -1);
                    Vector3 pos = new Vector3(transform.position.x + 3.0f * atkDir.x, 0.1f, transform.position.z + 3.0f * atkDir.z);
                    goblinManager.UseEnergyBall(pos, atkDir, 30.0f);
                    for (int i = 1; i < 5; i++) {
                        Vector3 dir = Quaternion.Euler(0, 20.0f*i , 0) * atkDir;
                        pos = new Vector3(transform.position.x + 3.0f * dir.x, 0.1f, transform.position.z + 3.0f * dir.z);
                        goblinManager.UseEnergyBall(pos, dir, 30.0f);

                        dir = Quaternion.Euler(0, -20.0f * i, 0) * atkDir;
                        pos = new Vector3(transform.position.x + 3.0f * dir.x, 0.1f, transform.position.z + 3.0f * dir.z);
                        goblinManager.UseEnergyBall(pos, dir, 30.0f);
                    }
                    attackCount++;
                }
            }
            else {
                if (stateTime > 1.0f)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 dir = Quaternion.Euler(0, 10 + 20.0f * i, 0) * atkDir;
                        Vector3 pos = new Vector3(transform.position.x + 3.0f * dir.x, 0.1f, transform.position.z + 3.0f * dir.z);
                        goblinManager.UseEnergyBall(pos, dir, 30.0f);

                        dir = Quaternion.Euler(0, -10 - 20.0f * i, 0) * atkDir;
                        pos = new Vector3(transform.position.x + 3.0f * dir.x, 0.1f, transform.position.z + 3.0f * dir.z);
                        goblinManager.UseEnergyBall(pos, dir, 30.0f);
                    }
                    attackCount = 0;
                    AtkSetIdle();
                }
            }
        }
    }
    void XAttack() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 1);
            int t = Random.Range(0, 3);
            while (goblinManager.PlayersDie[t]) {
                t++;
                deathCount++;
                if (t > 3) t = 0;
                if (deathCount >= 4) break;
            }
            targetPlayer = t;
        }
        else {
            stateTime += deltaTime;
            if (attackCount == 0) {
                if (stateTime > 0.5f)
                {
                    atkDir = goblinManager.PlayerPos[targetPlayer] - transform.position;
                    atkDir = new Vector3(atkDir.x, 0, atkDir.z).normalized;
                    Vector3 pos = new Vector3(transform.position.x + 3.0f * atkDir.x, 0.1f, transform.position.z + 3.0f * atkDir.z);
                    goblinManager.UseEnergyBall(pos, atkDir, 40.0f);

                    for (int i = 1; i <= 2; i++)
                    {
                        float angle = 20.0f * i;
                        pos = transform.position + 3.0f * (Quaternion.Euler(0, angle, 0) * atkDir);
                        pos.y = 0.1f;
                        goblinManager.UseEnergyBall(pos, atkDir,40.0f);
                        pos = transform.position + 3.0f * (Quaternion.Euler(0, -angle, 0) * atkDir);
                        pos.y = 0.1f;
                        goblinManager.UseEnergyBall(pos, atkDir,40.0f);
                    }
                    attackCount++;
                }
            }
            else if (attackCount == 1)
            {
                if (stateTime > 1.0f)
                {
                    float count = 3.0f;
                    float angle = 20.0f;
                    for (int i = 1; i < 4; i++)
                    {
                        Vector3 dir = Quaternion.Euler(0, angle * count, 0) * atkDir;
                        Vector3 pos = transform.position + 6.0f * dir;
                        pos.y = 0.1f;
                        dir = new Vector3(-dir.z, 0, dir.x);
                        goblinManager.UseEnergyBall(pos, dir, 20.0f);

                        dir = Quaternion.Euler(0, -angle * count, 0) * atkDir;
                        pos = transform.position + 6.0f * dir;
                        pos.y = 0.1f;
                        dir = new Vector3(dir.z, 0, -dir.x);
                        goblinManager.UseEnergyBall(pos, dir,20.0f);

                        count += 1.0f;
                    }
                    attackCount++;
                }
            }
            else if (attackCount == 2)
            {
                if (stateTime > 1.5f)
                {
                    Vector3 pos = new Vector3(transform.position.x + 3.0f * atkDir.x, 0.1f, transform.position.z + 3.0f * atkDir.z);
                    goblinManager.UseEnergyBall(pos, atkDir,40.0f);

                    for (int i = 1; i <= 2; i++)
                    {
                        float angle = 10.0f * i;
                        pos = transform.position + 3.0f * (Quaternion.Euler(0, angle, 0) * atkDir);
                        pos.y = 0.1f;
                        goblinManager.UseEnergyBall(pos, atkDir,40.0f);
                        pos = transform.position + 3.0f * (Quaternion.Euler(0, -angle, 0) * atkDir);
                        pos.y = 0.1f;
                        goblinManager.UseEnergyBall(pos, atkDir,40.0f);
                    }
                    attackCount = 0;
                    AtkSetIdle();
                }

            }
            //if (aniInfo.IsName("Attack"))
            //{
            //    Debug.Log(" ani time " + aniInfo.normalizedTime);
            //    if(aniInfo.normalizedTime > )
            //}
        }
        //stateTime += deltaTime;
        //if (stateTime > 0.5f) AtkSetIdle();
    }

    void ChangeColor() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 2);
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("ChangeColor") && aniInfo.normalizedTime >= 0.55f) {
                if (!changeColorOnce)
                {
                    changeColorOnce = true;
                    int c = (Random.Range(0, 100) % 6) + 1;
                    if (c == color)
                    {
                        c += Random.Range(1, 4);
                        if (c > 6) c -= 6;
                    }
                    color = c;
                    kingGoblin.SetColor(color);
                    render.material.SetInt("_colorID", color);
                }
                if (aniInfo.normalizedTime >= 0.96f) {
                    changeColorOnce = false;
                    IdleSetIdle();
                    animator.SetInteger("state", 0);
                    goChangeColor = false;
                } 
            }
        }
        

    }

    public void ResetUnit() {

    }

}
