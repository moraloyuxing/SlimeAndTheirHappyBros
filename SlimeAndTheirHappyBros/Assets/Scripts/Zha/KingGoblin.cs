using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingGoblin : IEnemyUnit
{
    bool firstInState = false, waveOnce = false, punchShopOnce = false, punchBushOnce = false;
    bool throwOnce = false;
    int hp, punchStep = -1, throwId = 0;

    Vector3[] punchPos = new Vector3[4] { new Vector3(-3.5f, -0.1f, 19.22f), new Vector3(1.7f, -0.1f, 19.22f), new Vector3(14.57f, -0.1f, 20.54f), new Vector3(-17.52f, -0.1f, 19.85f) };
    Vector3[] punchDir = new Vector3[4];
    Quaternion[] punchRot = new Quaternion[4];

    System.Action punchShop, punchBush;
    Animator animator;
    AnimatorStateInfo aniInfo;
    Transform transform;

    GoblinManager goblinManager;

    KingState curState = KingState.showUp;

    enum KingState {
        showUp, idle, punchAtk, waveAtk, throwAtk
    }

    public void SubPunchCBK(System.Action shopCBK, System.Action bushCBK) {
        punchShop = shopCBK;
        punchBush = bushCBK;
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
                Idle();
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
        }
    }

    void ShowUp() {
        aniInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (aniInfo.normalizedTime > 0.64f) {
            if (!punchShopOnce)
            {
                punchShopOnce = true;
                punchShop();
            }
            else {
                if (aniInfo.normalizedTime > 0.93f) {
                    if (!punchBushOnce)
                    {
                        punchBushOnce = true;
                        punchBush();
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
    void Idle() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 1);
            animator.SetTrigger("attackOver");
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
            }
            else if (punchStep == 1 && aniInfo.normalizedTime >= 0.5f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 2;
            }
            else if (punchStep == 2 && aniInfo.normalizedTime >= 0.57f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 3;
            }
            else if (punchStep == 3 && aniInfo.normalizedTime >= 0.71f)
            {
                goblinManager.UsePunch(punchPos[punchStep], punchDir[punchStep], punchRot[punchStep]);
                punchStep = 4;
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
            
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("hammerAttack") && aniInfo.normalizedTime >= 0.91f) {
                if (!waveOnce) {
                    waveOnce = true;
                    goblinManager.UseWave(transform.position);
                }
                if (aniInfo.normalizedTime >= 0.98f)
                {
                    waveOnce = false;
                    SetState(KingState.idle);
                }
            }
        }
    }

    void ThrowAtk() {
        if (!firstInState)
        {
            firstInState = true;
            animator.SetInteger("state", 4);
        }
        else {
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("throwAtk") && aniInfo.normalizedTime >= 0.79f) {
                if (!throwOnce) {
                    goblinManager.UseFallingGoblin(throwId);
                    throwOnce = true;
                }
                if (aniInfo.normalizedTime >= 0.96f) {

                    throwId = Random.Range(0, 99) % 4;
                    while (goblinManager.PlayersDie[throwId]) {
                        throwId++;
                        if (throwId > 3) throwId = 0;
                    }

                    throwOnce = false;
                    SetState(KingState.idle);
                }
            }
        }
    }

    public void ResetUnit() {

    }

    public void Spawn(Vector3 pos, int col) {
        transform.gameObject.SetActive(true);
    }
}
