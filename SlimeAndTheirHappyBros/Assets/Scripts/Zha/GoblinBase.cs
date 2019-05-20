using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBase
{
    protected bool firstInState = true, followingPath = false, getAniInfo = false;
    protected int hp, atkValue, color, pathIndex;
    protected float deltaTime, inStateTime, totalTime;  //calculateDistTime = .0f
    protected float speed, atkDist, sightDist, spawnHeight, turnDist;
    protected float backSpeed = 10.0f;
    protected float imgScale;
    protected int minMoney, maxMoney;


    public int GetColor
    {
        get
        {
            return color;
        }
    }

    protected int targetPlayer = -1;
    protected float nearstPlayerDist = 5000;
    protected float[] playerDist = new float[4] { 5000, 5000, 5000, 5000 };

    protected Vector3 selfPos, moveFwdDir, oldTargetPos;

    protected Transform transform, image;
    protected Animator animator;
    protected AnimatorStateInfo aniInfo;
    protected SpriteRenderer renderer;

    protected PathFinder.Path path;
    protected GoblinManager goblinManager;

    public enum GoblinState
    {
        moveIn, idle, ramble, chase, attack, hurt, die, erroeCatch
    }
    protected GoblinState curState = GoblinState.moveIn;



    public void UpdateAllPlayerPos() {
        for (int i = 0; i < 4; i++) {
            playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
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

    public virtual void SetState()
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
    public virtual void SetState(GoblinState state) {
        inStateTime = 0.0f;
        firstInState = true;
        curState = state;
    }

    public virtual void StateMachine()
    {
        inStateTime += deltaTime;
        switch (curState)
        {
            case GoblinState.moveIn:
                MoveIn();
                DetectAttack();
                break;
            case GoblinState.idle:
                Idle();
                DetectChaseAttack();
                break;
            case GoblinState.ramble:
                Ramble();
                DetectChaseAttack();
                if (OutBorder()) break;
                break;
            case GoblinState.chase:
                Chase();
                DetectAttack();
                if (OutBorder()) break;
                break;
            case GoblinState.attack:
                Attack();
                break;
            case GoblinState.hurt:
                GetHurt();
                break;
            case GoblinState.die:
                Die();
                break;
            case GoblinState.erroeCatch:
                ErroeCatch();
                DetectAttack();
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
        if (diff <= atkDist * atkDist) {
            SetState(GoblinState.attack);
        } 
    }
    public virtual void OverAttackDetectDist() {
        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= atkDist * atkDist) SetState(GoblinState.attack);
        else SetState(GoblinState.chase);
    }



    public virtual void MoveIn()
    {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            firstInState = false;
            moveFwdDir = new Vector3(-selfPos.x, 0, -selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
        }
        else {
            transform.position += deltaTime * speed * moveFwdDir;
            if (inStateTime > 6.0f) SetState(GoblinState.ramble);
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
        if (Mathf.Abs(selfPos.x) > goblinManager.mapBorder.x || Mathf.Abs(selfPos.z) > goblinManager.mapBorder.y)
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
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
        }
        else {
            if (inStateTime < totalTime) {
                if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, LayerMask.NameToLayer("barrier")))
                {
                    float degree = Random.Range(135.0f, 225.0f);
                    moveFwdDir = Quaternion.AngleAxis(degree, Vector3.up) * moveFwdDir;
                    float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                }
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
            animator.speed = 1.2f;
            firstInState = false;
            followingPath = false;
            PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
        }
        else {
            if (inStateTime < 0.3f)
            {
                inStateTime += deltaTime;
            }
            else {
                if (goblinManager.PlayersMove[targetPlayer])
                {
                    Debug.Log("request path find");
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
                        Debug.Log("reach path goal");
                        followingPath = false;
                        SetState(GoblinState.attack);
                    }
                    else
                    {
                        pathIndex++;
                        moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                        float scaleX = (moveFwdDir.x > .0f)?-1.0f:1.0f;
                        image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                    }
                }
                transform.position += deltaTime * speed * 1.2f * moveFwdDir;

            }
        }
    }
    public virtual void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {

            path = new PathFinder.Path(waypoints, selfPos, turnDist); 
            followingPath = true;
            pathIndex = 0;
            moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);

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
        
    }


    //public virtual void DetectGethurt() {
    //    Collider[]colliders =  Physics.OverlapBox(selfPos, new Vector3(0.55f, 1.4f, 0.1f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("DamageToGoblin"));
    //    int i = 0;
    //    while (i < colliders.Length) {
    //        hp--;
    //        if (i == 0 && curState != GoblinState.hurt) {
    //            SetState(GoblinState.hurt);
    //            moveFwdDir = -Vector3.forward;
    //        }
    //        //Debug.Log(colliders[i].name);
    //        i++;
    //    }
    //}

    public virtual void GetHurt()
    {
        if (firstInState)
        {
            firstInState = false;
            //animator.SetTrigger("hurt");
            animator.Play("hurt");
            animator.SetInteger("state",3);
        }
        else {
            transform.position += (backSpeed)* deltaTime * moveFwdDir;
            backSpeed -= deltaTime * 15.0f;
            if (backSpeed <= .0f) backSpeed = .0f;
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (aniInfo.IsName("hurt"))Debug.Log(aniInfo.normalizedTime);
            if (aniInfo.IsName("hurt") && aniInfo.normalizedTime >= 0.99f) {
                if (hp <= 0) SetState(GoblinState.die);
                else OverAttackDetectDist();
                backSpeed = 10.0f;

            }
        }
    }
    public virtual void Die()
    {
        
    }
    public virtual void OnGettingHurt(int col, int atkValue, int playerID, Vector3 dir)
    {
        if (col == 1 || col == 2 || col == 4)
        {
            if (col == color) {
                if (hp > 0)
                {
                    AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                    hp -= atkValue;
                    targetPlayer = playerID;
                    moveFwdDir = dir.normalized;
                    SetState(GoblinState.hurt);
                }
            }
            else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        }
        else {
            if (col == 3)
            {
                if ((color == 1 || color == 2))
                {
                    if (hp > 0)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                        hp -= atkValue;
                        targetPlayer = playerID;
                        moveFwdDir = dir.normalized;
                        SetState(GoblinState.hurt);
                    }
                }
                else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
            }
            else if (col == 5) {
                if ((color == 1 || color == 4))
                {
                    if (hp > 0)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                        hp -= atkValue;
                        targetPlayer = playerID;
                        moveFwdDir = dir.normalized;
                        SetState(GoblinState.hurt);
                    }
                }
                else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
            }
            else if (col == 6)
            {
                if ((color == 2 || color == 4))
                {
                    if (hp > 0)
                    {
                        AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
                        hp -= atkValue;
                        targetPlayer = playerID;
                        moveFwdDir = dir.normalized;
                        SetState(GoblinState.hurt);
                    }
                }
                else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
            }
        }
    }

    public virtual void ErroeCatch() {
        if (firstInState)
        {
            animator.SetInteger("state", 1);
            animator.speed = 1f;
            firstInState = false;
            moveFwdDir = new Vector3(0 - selfPos.x, 0, 0 - selfPos.z).normalized;
            float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
            image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
        }
        else {
            transform.position += deltaTime * speed * moveFwdDir;
            if (inStateTime > 1.0f) SetState();
        }
    }

}
