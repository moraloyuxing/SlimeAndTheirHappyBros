using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBase
{
    protected bool firstInState = false, followingPath = false;
    protected int hp, atkValue, color, pathIndex;
    protected float deltaTime, inStateTime, totalTime;
    protected float speed, atkDist, sightDist, spawnHeight, turnDist;

    protected int targetPlayer = -1;
    protected float nearstPlayerDist = 5000;
    protected float[] playerDist = new float[4] { 5000, 5000, 5000, 5000 };

    protected Vector3 selfPos, moveFwdDir, oldTargetPos;


    protected Transform transform;
    protected Animator animator;
    protected SpriteRenderer renderer;

    Path path;
    protected GoblinManager goblinManager;

    public enum GoblinState
    {
        moveIn, idle, ramble, chase, attack, hurt, die, erroeCatch
    }
    protected GoblinState curState = GoblinState.moveIn;



    public void UpdatePlayerPos(int id)
    {

        playerDist[id] = Mathf.Abs(goblinManager.PlayerPos[id].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[id].z - selfPos.z);
        if (playerDist[id] < nearstPlayerDist)
        {
            nearstPlayerDist = playerDist[id];
            targetPlayer = id;
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
                DetectChaseAttack();
                Idle();
                break;
            case GoblinState.ramble:
                DetectChaseAttack();
                Ramble();
                break;
            case GoblinState.chase:
                DetectChaseAttack();
                Chase();
                break;
            case GoblinState.attack:
                Attack();
                break;
            case GoblinState.hurt:
                break;
            case GoblinState.die:
                break;
            case GoblinState.erroeCatch:
                break;

        }


    }

    public virtual void DetectChaseAttack() {
        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= sightDist*sightDist) {
            if (diff <= atkDist*atkDist) SetState(GoblinState.chase);
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
            if (Mathf.Abs(playerPos.x) > 70) playerPos.x = Mathf.Sign(playerPos.x) * 70.0f;
            if(Mathf.Abs(playerPos.y) > 70) playerPos.y = Mathf.Sign(playerPos.y) * 70.0f;
            moveFwdDir = new Vector3(playerPos.x - selfPos.x, 0, playerPos.z - selfPos.z).normalized;
        }
        else {
            if (inStateTime < totalTime) {
                if (Mathf.Abs(selfPos.x) <= 71 && Mathf.Abs(selfPos.y) <= 74)
                {
                    if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))
                    {
                        float degree = Random.Range(135.0f, 225.0f);
                        moveFwdDir = Quaternion.AngleAxis(degree,Vector3.up) * moveFwdDir;
                    }
                }
                else {
                    SetState(GoblinState.erroeCatch);
                }
                transform.position += speed * moveFwdDir;
            }
            else SetState();
        }
    }
    public virtual void Chase()
    {
        if (firstInState)
        {
            PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
        }
        else {
            if (inStateTime < 0.2f)
            {
                inStateTime += deltaTime;
            }
            else {
                if (goblinManager.PlayersMove[targetPlayer])
                {
                    PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
                    inStateTime = 0.0f;
                }
            }
            if (followingPath) {
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                    {
                        followingPath = false;
                        SetState(GoblinState.attack);
                    }
                    else
                    {
                        pathIndex++;
                    }
                }
            }
        }
    }
    void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {

            path = new Path(waypoints, transform.position, turnDist); 
            followingPath = true;
            pathIndex = 0;
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
        }
        else
        {
            Debug.Log("Can't Find");
        }
    }
    public virtual void Attack()
    {
        if (firstInState) {

        }
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

    public void ErroeCatch() {
        if (firstInState)
        {
            moveFwdDir = new Vector3(0 - selfPos.x, 0, 0 - selfPos.z).normalized;
        }
        else {
            transform.position += speed * moveFwdDir;
            if (inStateTime > 1.0f) SetState();
        }
    }

}
