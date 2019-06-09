using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingGoblin : IEnemyUnit
{
    bool firstInState = false;
    int hp;
    Animator animator;
    AnimatorStateInfo aniInfo;
    Transform transform;

    GoblinManager goblinManager;

    KingState curState = KingState.showUp;

    enum KingState {
        showUp, idle, punchAtk, waveAtk
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
        if (aniInfo.normalizedTime >= 0.98f) {
            SetState(KingState.waveAtk);
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
            animator.SetInteger("state", 1);
        }
    }

    public void ResetUnit() {

    }

    public void Spawn(Vector3 pos, int col) { }
}
