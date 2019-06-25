using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingGoblin : IEnemyUnit
{
    bool showEnable = false;
    bool firstInState = false, waveOnce = false, punchShopOnce = false, punchBushOnce = false;
    bool throwOnce = false, goRoar = false;
    int hp = 150, punchStep = -1, throwId = 0, deathCount = 0, color = 0;
    int atkCount = 0, totalAtk = 1;
    float stateTime = .0f, idleTime = 2.0f;

    Vector3[] punchPos = new Vector3[4] { new Vector3(-3.5f, -0.1f, 19.22f), new Vector3(1.7f, -0.1f, 19.22f), new Vector3(14.57f, -0.1f, 20.54f), new Vector3(-17.52f, -0.1f, 19.85f) };
    Vector3[] punchDir = new Vector3[4];
    Quaternion[] punchRot = new Quaternion[4];

    System.Action punchShop, punchBush, changeColor;
    Animator animator;
    AnimatorStateInfo aniInfo;
    Transform transform;
    SpriteRenderer render;
    GoblinManager goblinManager;

    KingState curState = KingState.showUp;

    enum KingState {
        showUp, idle, punchAtk, waveAtk, throwAtk, roar
    }

    public void SubPunchCBK(System.Action shopCBK, System.Action bushCBK) {
        punchShop = shopCBK;
        punchBush = bushCBK;
    }

    public void SubChangeColorCBK(System.Action cbk) {
        changeColor = cbk;
    }

    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager) {
        transform = t;
        animator = transform.GetComponent<Animator>();
        goblinManager = manager;

        float rad = 260.0f * Mathf.Deg2Rad;
        punchDir[0] = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        punchDir[1] = new Vector3(-punchDir[0].x, 0, punchDir[0].z);
        rad = 285.0f * Mathf.Deg2Rad;
        punchDir[2] = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        punchDir[3] = new Vector3(-punchDir[2].x, 0, punchDir[2].z);

        punchRot[0] = Quaternion.Euler(90f, 0, 80f);
        punchRot[1] = Quaternion.Euler(90f, 0, 100f);
        punchRot[2] = Quaternion.Euler(90f, 0, 105f);
        punchRot[3] = Quaternion.Euler(90f, 0, 75f);

        render = transform.Find("BossSprite").GetComponent<SpriteRenderer>();
        render.enabled = false;
    }

    void SetState(KingState state) {
        curState = state;
        firstInState = false;

    }

    public void Update(float dt) {
        if (Input.GetKeyDown(KeyCode.J) && curState == KingState.idle) SetState(KingState.waveAtk);
        if (Input.GetKeyDown(KeyCode.K) && curState == KingState.idle) SetState(KingState.punchAtk);
        if (Input.GetKeyDown(KeyCode.L) && curState == KingState.idle) SetState(KingState.throwAtk);
        switch (curState) {
            case KingState.showUp:
                ShowUp();
                break;
            case KingState.idle:
                Idle(dt);
                break;
            case KingState.punchAtk:
                PunchAtk();
                break;
            case KingState.waveAtk:
                WaveAtk();
                break;
            case KingState.throwAtk:
                ThrowAtk();
                break;
            case KingState.roar:
                Roar();
                break;
        }
    }

    void ShowUp() {
        aniInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!showEnable && aniInfo.normalizedTime > 0.15) {
            showEnable = true;
            render.enabled = true;

        }

        if (aniInfo.normalizedTime > 0.66f) {
            if (!punchShopOnce)
            {
                AudioManager.SingletonInScene.PlaySound2D("HouseBoom", 0.3f);
                MultiPlayerCamera.CamerashakingSingleton.StartShakeEasyOut(0.1f, 0.5f,0.5f);
                punchShopOnce = true;
                punchShop();
            }
            else {
                if (aniInfo.normalizedTime > 0.93f) {
                    if (!punchBushOnce)
                    {
                        punchBushOnce = true;
                        punchBush();
                        MultiPlayerCamera.CamerashakingSingleton.StartShakeEasyOut(0.1f, 0.5f, 0.3f);
                        
                    }
                    else {
                        if (aniInfo.normalizedTime >= 0.96f)
                        {
                            SetState(KingState.idle);
                        }
                    }
                }
            }
        }

    }
    void Idle(float dt) {
        if (!firstInState)
        {
            firstInState = true;

            if (!goRoar)
            {
                animator.SetInteger("state", 1);
                idleTime = Random.Range(3.0f, 6.0f);
                Debug.Log("first idle  " + idleTime);
            }
            else {
                animator.SetInteger("state", 5);
                goRoar = false;
                SetState(KingState.roar);
                Debug.Log("go roar");
            }
            animator.SetTrigger("attackOver");

        }
        else {
            if (goRoar) {
                animator.SetInteger("state", 5);
                goRoar = false;
                SetState(KingState.roar);
                Debug.Log("go roar");
                return;
            }
            stateTime += dt;
            if (stateTime > idleTime)
            {
                stateTime = .0f;

                int op = Random.Range(0, 120);
                if (op < 40) SetState(KingState.punchAtk);
                else if (op < 80) SetState(KingState.waveAtk);
                else SetState(KingState.throwAtk);
                Debug.Log("idle end: " + op);
            }
        }
    }
    void PunchAtk() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 2);
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (punchStep < 0 && aniInfo.IsName("punchAtk")) punchStep = 0;
            else if (punchStep == 0 && aniInfo.normalizedTime >= 0.39f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 1;
                AudioManager.SingletonInScene.PlaySound2D("KingPunch", 0.5f);
            }
            else if (punchStep == 1 && aniInfo.normalizedTime >= 0.5f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 2;
                AudioManager.SingletonInScene.PlaySound2D("KingPunch", 0.5f);
            }
            else if (punchStep == 2 && aniInfo.normalizedTime >= 0.57f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 3;
                AudioManager.SingletonInScene.PlaySound2D("KingPunch", 0.5f);
            }
            else if (punchStep == 3 && aniInfo.normalizedTime >= 0.71f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 4;
                AudioManager.SingletonInScene.PlaySound2D("KingPunch", 0.5f);
            }
            else if (punchStep == 4 && aniInfo.normalizedTime >= 0.96f)
            {
                punchStep = -1;
                SetState(KingState.idle);
            }
        }

    }
    void WaveAtk() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 3);
            totalAtk = Random.Range(1,3);
            
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("hammerAttack") && aniInfo.normalizedTime >= (0.91f + atkCount)) {

                atkCount++;
                goblinManager.UseWave(transform.position);
                AudioManager.SingletonInScene.PlaySound2D("CircleAttack", 0.3f);
                MultiPlayerCamera.CamerashakingSingleton.StartShakeEasyOut(0.1f, 0.5f, 0.5f);
                if (atkCount >= totalAtk) {
                    atkCount = 0;
                    SetState(KingState.idle);
                }
                //if (!waveOnce) {
                //    waveOnce = true;
                //    goblinManager.UseWave(transform.position);
                //}
                //if (aniInfo.normalizedTime >= 0.98f)
                //{
                //    waveOnce = false;
                //    SetState(KingState.idle);
                //}
            }
        }
    }

    void ThrowAtk() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 4);
            totalAtk = Random.Range(2,5);
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("throwAtk") && aniInfo.normalizedTime >= (0.79f + atkCount)) {

                throwId = Random.Range(0, 99) % 4;
                while (goblinManager.PlayersDie[throwId])
                {
                    throwId++;
                    deathCount++;
                    if (throwId > 3) throwId = 0;
                    if (deathCount >= 4) break;
                }
                deathCount = 0;
                goblinManager.UseFallingGoblin(throwId);
                AudioManager.SingletonInScene.PlaySound2D("KingThrow", 0.6f);

                atkCount++;
                Debug.Log(atkCount + " < " + totalAtk);
                if (atkCount >= totalAtk) {
                    atkCount = 0;
                    SetState(KingState.idle);
                }


                //if (!throwOnce) {
                //    goblinManager.UseFallingGoblin(throwId);
                //    throwOnce = true;
                //}
                //if (aniInfo.normalizedTime >= 0.96f) {

                //    throwId = Random.Range(0, 99) % 4;
                //    while (goblinManager.PlayersDie[throwId]) {
                //        throwId++;
                //        deathCount++;
                //        if (throwId > 3) throwId = 0;
                //        if (deathCount >= 4) break;
                //    }

                //    throwOnce = false;

                //}
            }
        }
    }

    void Roar() {
        if (!firstInState)
        {
            Debug.Log("first roar");
            firstInState = true;
            //animator.SetInteger("state", 5);
            changeColor();
            AudioManager.SingletonInScene.PlaySound2D("KingRoar", 0.5f);
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("Howl") && aniInfo.normalizedTime > 0.95f) {
                SetState(KingState.idle);
            }
        }
    }

    public void GetHurt(int c, int value) {
        if(color == c) hp -= value;

    }

    public bool IsIdle() {
        if (curState == KingState.idle) return true;
        else return false;
    }

    public void SetRoar() {
        goRoar = true;
    }

    public void SetColor(int c) {
        color = c;
    }

    public void ResetUnit() {

    }

    public void Spawn(Vector3 pos, int col) {
        transform.gameObject.SetActive(true);

    }
}
