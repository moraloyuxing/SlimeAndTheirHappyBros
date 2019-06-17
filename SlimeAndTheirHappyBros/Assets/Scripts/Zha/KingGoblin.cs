using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingGoblin : IEnemyUnit
{
    bool firstInState = false, waveOnce = false, punchShopOnce = false, punchBushOnce = false;
    int hp;
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
    }

    void SetState(KingState state) {
        curState = state;
        firstInState = false;

    }

    public void Update(float dt) {
        if (Input.GetKeyDown(KeyCode.D)) SetState(KingState.waveAtk);
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
                if (aniInfo.normalizedTime > 0.9f) {
                    if (!punchBushOnce)
                    {
                        punchShopOnce = true;
                        punchBush();
                    }
                    else {
                        if (aniInfo.normalizedTime >= 0.98f)
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
            animator.SetInteger("state", 1);
        }
    }
    void PunchAtk() {

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
                    animator.SetInteger("state", 1);
                    animator.SetTrigger("attackOver");
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
