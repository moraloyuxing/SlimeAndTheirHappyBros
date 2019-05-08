using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBase
{
    protected bool firstInState = true, followingPath = false;
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



    public void UpdateAllPlayerPos() {
        for (int i = 0; i < 4; i++) {
            playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
            Debug.Log("nearst: " + nearstPlayerDist + "    cur:" + playerDist[i]);
            if (playerDist[i] < nearstPlayerDist)
            {
                
                nearstPlayerDist = playerDist[i];
                targetPlayer = i;
            }
        }
    }

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
        firstInState = true;
        float opp = Random.Range(.0f, 10.0f);
        if (opp > 7.0f)
        {
            if (curState == GoblinState.idle) curState = GoblinState.ramble;
            else if (curState == GoblinState.ramble) curState = GoblinState.idle;
            else curState = GoblinState.idle;
        }
    }
    public void SetState(GoblinState state) {
        inStateTime = 0.0f;
        firstInState = true;
        curState = state;
    }

    public virtual void StateMachine()
    {
        inStateTime += deltaTime;
        Debug.Log("State:  " + curState);
        switch (curState)
        {
            case GoblinState.moveIn:
                DetectAttack();
                MoveIn();
                break;
            case GoblinState.idle:
                DetectChaseAttack();
                Idle();
                break;
            case GoblinState.ramble:
                if (OutBorder()) break;
                DetectChaseAttack();
                Ramble();
                break;
            case GoblinState.chase:
                if (OutBorder()) break;
                DetectAttack();
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
                ErroeCatch();
                break;

        }


    }

    public virtual void DetectChaseAttack() {
        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= sightDist * sightDist)
        {
            if (diff >= atkDist * atkDist) SetState(GoblinState.chase);
            else SetState(GoblinState.attack);
        }
    }
    public virtual void DetectAttack() {
        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= atkDist * atkDist) SetState(GoblinState.attack);
    }


    public virtual void MoveIn()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            firstInState = false;
            moveFwdDir = new Vector3(-selfPos.x, 0, -selfPos.z).normalized;
            Debug.Log("slef pos " + selfPos);
        }
        else {
            transform.position += deltaTime * speed * moveFwdDir;
            Debug.Log(moveFwdDir);
            if (inStateTime > 1.0f) SetState(GoblinState.chase);
        }
    }

    public virtual void Idle()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 0);
            firstInState = false;
            totalTime = Random.Range(0.5f, 1.5f);
        }
        else {
            if (inStateTime > totalTime) SetState();
        }
    }

    public bool OutBorder() {
        if (Mathf.Abs(selfPos.x) > goblinManager.mapBorder.x || Mathf.Abs(selfPos.y) > goblinManager.mapBorder.y)
        {
            SetState(GoblinState.erroeCatch);
            return true;
        }
        else return false;
    }

    public virtual void Ramble()
    {
        if(firstInState)
        {
            animator.SetInteger("state", 1);
            firstInState = false;
            totalTime = Random.Range(1.0f, 3.0f);
            Vector3 playerPos = goblinManager.PlayerPos[Random.Range(0, 3)] + new Vector3(Random.Range(-3,3),0, Random.Range(-3, 3));
            if (Mathf.Abs(playerPos.x) > 70) playerPos.x = Mathf.Sign(playerPos.x) * 70.0f;
            if(Mathf.Abs(playerPos.y) > 70) playerPos.y = Mathf.Sign(playerPos.y) * 70.0f;
            moveFwdDir = new Vector3(playerPos.x - selfPos.x, 0, playerPos.z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            transform.localScale = new Vector3(scaleX, 1, 1);
        }
        else {
            if (inStateTime < totalTime) {
                if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))
                {
                    float degree = Random.Range(135.0f, 225.0f);
                    moveFwdDir = Quaternion.AngleAxis(degree, Vector3.up) * moveFwdDir;
                }
                Debug.Log("Ramble dir " + moveFwdDir);
                transform.position += deltaTime * speed * moveFwdDir;
            }
            else SetState();
        }
    }
    public virtual void Chase()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            animator.speed = 1.0f;
            firstInState = false;
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
                        moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                        float scaleX = (moveFwdDir.x > .0f)?-1.0f:1.0f;
                        transform.localScale = new Vector3(scaleX, 1, 1);
                    }
                }
                Debug.Log("chase  " + moveFwdDir);
                transform.position += deltaTime * speed * 1.5f * moveFwdDir;

            }
        }
    }
    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {

            path = new Path(waypoints, transform.position, turnDist); 
            followingPath = true;
            pathIndex = 0;
            moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            transform.localScale = new Vector3(scaleX, 1, 1);

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
        if (firstInState)
        {
            animator.SetInteger("state", 2);
            animator.speed = 1.0f;
            firstInState = false;

            moveFwdDir = new Vector3(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, 0, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            transform.localScale = new Vector3(scaleX, 1, 1);
        }
        else {
            if (inStateTime > 0.6f && inStateTime < 1.0f) {

                float atkSpeed = (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))?.0f: speed * 3.0f;
                transform.position += deltaTime * atkSpeed * 1.5f * moveFwdDir;
            } 
            else if (inStateTime > 1.3f) {
                DetectChaseAttack();
            }
        }
    }

    public virtual void DetectGethurt() {

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
            animator.SetInteger("state", 1);
            animator.speed = 1f;
            firstInState = false;
            moveFwdDir = new Vector3(0 - selfPos.x, 0, 0 - selfPos.z).normalized;
        }
        else {
            transform.position += deltaTime * speed * moveFwdDir;
            if (inStateTime > 1.0f) SetState();
        }
    }

}
