using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBase
{
    protected bool firstInState = false;
    protected int hp, atkValue, color, hitCatch;
    protected float deltaTime, inStateTime, totalTime;
    protected float speed, atkDist, sightDist, spawnHeight;

    protected int targetPlayer = -1;
    protected float nearstPlayerDist = 5000;
    protected float[] playerDist = new float[4] { 5000, 5000, 5000, 5000 };

    protected Vector3 selfPos, moveFwdDir;


    protected Transform transform;
    protected Animator animator;
    protected SpriteRenderer renderer;

    protected GoblinManager goblinManager;

    public enum GoblinState
    {
        moveIn, idle, ramble, chase, attack, hurt, die
    }
    protected GoblinState curState = GoblinState.moveIn;



    public void UpdatePlayerPos(int id)
    {

        playerDist[id] = Mathf.Abs(goblinManager.PlayerPos[id].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[id].z - selfPos.z);
        if (id == targetPlayer)
        {

        }
        if (playerDist[id] < nearstPlayerDist)
        {
            nearstPlayerDist = playerDist[id];
        }
    }

    public void SetState()
    {
        inStateTime = 0.0f;
        firstInState = false;
        float opp = Random.Range(.0f, 10.0f);
        if (opp > 7.0f)
        {
            if (curState == GoblinState.idle) curState = GoblinState.ramble;
            else curState = GoblinState.ramble;
        }
    }
    public void SetState(GoblinState state) {
        inStateTime = 0.0f;
        firstInState = false;
        curState = state;
    }

    public virtual void StateMachine()
    {
        inStateTime += deltaTime;
        switch (curState)
        {
            case GoblinState.moveIn:
                MoveIn();
                break;
            case GoblinState.idle:
                break;
            case GoblinState.ramble:
                break;
            case GoblinState.chase:
                break;
            case GoblinState.attack:
                break;
            case GoblinState.hurt:
                break;
            case GoblinState.die:
                break;

        }


    }

    public virtual void DetectChaseAttack() {
        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= sightDist) {
            if (diff <= atkDist) SetState(GoblinState.chase);
            else SetState(GoblinState.attack);
        }
    }

    public virtual void MoveIn()
    {
        if (firstInState)
        {
            moveFwdDir = new Vector3(-selfPos.x, 0, -selfPos.z).normalized;
        }
        else {
            transform.position += speed * moveFwdDir;
            if (inStateTime > 1.0f) SetState();
        }
    }

    public virtual void Idle()
    {
        if (firstInState)
        {
            totalTime = Random.Range(0.5f, 1.5f);
        }
        else {
            if (inStateTime > totalTime) SetState();
        }
    }
    public virtual void Ramble()
    {
        if(firstInState)
        {
            totalTime = Random.Range(0.5f, 1.5f);
            Vector3 playerPos = goblinManager.PlayerPos[Random.Range(0, 3)] + new Vector3(Random.Range(-3,3),0, Random.Range(-3, 3));
            moveFwdDir = new Vector3(playerPos.x - selfPos.x, 0, playerPos.z - selfPos.z).normalized;
        }
        else {
            if (inStateTime < totalTime) {
                if (hitCatch <= 5)
                {
                    if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))
                    {
                        hitCatch++;
                        moveFwdDir = -1.0f * moveFwdDir;
                    }
                }
                else {
                    moveFwdDir = new Vector3(0 - selfPos.x, 0, 0 - selfPos.z).normalized;
                }
                transform.position += speed * moveFwdDir;
            }
            else SetState();
        }
    }
    public virtual void Chase()
    {

    }
    public virtual void Attack()
    {

    }
    public virtual void GetHurt()
    {

    }
    public virtual void Die()
    {

    }
    public virtual void OnGetHurt(int value)
    {

    }

}
