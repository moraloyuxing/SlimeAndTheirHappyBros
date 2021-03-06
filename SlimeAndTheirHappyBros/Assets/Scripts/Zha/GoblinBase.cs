﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBase
{
    protected bool firstInState = true, followingPath = false, getAniInfo = false;
    protected bool startFindPath = false, closingPlayer = false;
    protected int hp,maxHp, atkValue, color, pathIndex;
    protected float deltaTime, inStateTime, totalTime;  //calculateDistTime = .0f
    protected float speed, atkDist, sightDist, spawnHeight, turnDist;
    protected float blankTime = .0f, closeTime = .0f;
    protected float backSpeed = 10.0f;
    protected float imgScale, whiteScale = -1.0f;
    protected int minMoney, maxMoney, closeNum = 0;
    

    public int GetColor
    {
        get
        {
            return color;
        }
    }

    protected int targetPlayer = -1, targetPlayer2 = -1;
    protected float nearstPlayerDist = 5000;
    protected float[] playerDist = new float[4] { 5000, 5000, 5000, 5000 };

    protected Vector3 selfPos, moveFwdDir, hurtDir, oldTargetPos;

    protected Transform transform, image;
    protected Animator animator;
    protected AnimatorStateInfo aniInfo;
    protected SpriteRenderer renderer;

    protected PathFinder.Path path;
    protected PathRequestManager.PathRequest curPathRequest;
    protected GoblinManager goblinManager;

    public enum GoblinState
    {
        moveIn, idle, ramble, closePlayer, chase, attack, hurt, fakeHurt, die, attackBreak, erroeCatch
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
        startFindPath = false;
        inStateTime = 0.0f;
        firstInState = true;
        if (hp <= 0) curState = GoblinState.die;
        else {
            float opp = Random.Range(.0f, 10.0f);
            if (opp > 7.0f)
            {
                if (curState == GoblinState.idle) curState = GoblinState.ramble;
                else if (curState == GoblinState.ramble) curState = GoblinState.idle;
                else curState = GoblinState.idle;
            }
        }
    }
    public virtual void SetState(GoblinState state) {

        startFindPath = false;
        inStateTime = 0.0f;
        firstInState = true;
        if (hp <= 0) curState = GoblinState.die;
       else curState = state;
    }

    public virtual void StateMachine()
    {
        inStateTime += deltaTime;
        switch (curState)
        {
            case GoblinState.moveIn:
                DecideTargetPlayer();
                MoveIn();
                DetectAttack();
                break;
            case GoblinState.idle:
                DecideTargetPlayer();
                Idle();
                DetectChaseAttack();
                break;
            case GoblinState.ramble:
                DecideTargetPlayer();
                Ramble();
                DetectChaseAttack();
                if (OutBorder()) break;
                break;
            case GoblinState.chase:
                DecideTargetPlayer();
                Chase();
                DetectAttack();
                if (OutBorder()) break;
                break;
            case GoblinState.closePlayer:
                DecideTargetPlayer();
                ClosePlayer();
                DetectAttack();
                break;
            case GoblinState.attack:
                Attack();
                break;
            case GoblinState.attackBreak:
                AttackBreak();
                AttackBreakDetectDist();
                break;
            case GoblinState.hurt:
                GetHurt();
                break;
            case GoblinState.fakeHurt:
                KnockBack();
                break;
            case GoblinState.die:
                Die();
                break;
            case GoblinState.erroeCatch:
                DecideTargetPlayer();
                ErroeCatch();
                DetectAttack();
                break;

        }
        if (whiteScale >= .0f)
        {
            whiteScale -= deltaTime * 8.0f;
            renderer.material.SetFloat("_WhiteScale", whiteScale);
            if (whiteScale < .0f) whiteScale = -1.0f;
        }

    }

    public virtual void DecideTargetPlayer() {
        if (hp <= 0) return;
        nearstPlayerDist = 500.0f;
        for (int i = 0; i < 4; i++)
        {
            if (goblinManager.PlayersDie[i]) continue;
            playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
            if (playerDist[i] < nearstPlayerDist)
            {
                nearstPlayerDist = playerDist[i];
                targetPlayer = i;
            }
            //if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
        }
    }
    public virtual void HurtOverDecideTargetPlayer()
    {
        if (hp <= 0) return;
        for (int i = 0; i < 4; i++)
        {
            if (goblinManager.PlayersDie[i]) continue;
            float diff = new Vector2(goblinManager.PlayerPos[i].x - selfPos.x, goblinManager.PlayerPos[i].z - selfPos.z).sqrMagnitude;
            if (diff <= atkDist * atkDist )
            {
                targetPlayer = i;
                return;
            }
            //if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
        }
    }

    public virtual void DetectChaseAttack() {

        blankTime += deltaTime;
        if (blankTime > 10.0f) {
            blankTime = .0f;
            closeNum++;
            if (closeNum < 3) closingPlayer = true;
            else closingPlayer= false;
            if (!startFindPath) CalculatePath();
            return;
        }

        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= sightDist * sightDist)
        {
            if (diff >= atkDist * atkDist) {
                if (!startFindPath) {
                    CalculatePath();
                } 
            } //SetState(GoblinState.chase);
            else SetState(GoblinState.attack);
        }
    }
    public virtual void DetectAttack() {
        float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
        if (diff <= atkDist * atkDist) {
            SetState(GoblinState.attack);
        } 
    }
    public virtual void AttackBreakDetectDist() {
        if (goblinManager.PlayersDie[targetPlayer]) SetState(GoblinState.ramble);
        else {
            float diff = new Vector2(goblinManager.PlayerPos[targetPlayer].x - selfPos.x, goblinManager.PlayerPos[targetPlayer].z - selfPos.z).sqrMagnitude;
            if (diff <= atkDist * atkDist) SetState(GoblinState.attack);
            else
            {
                if (!startFindPath)
                {
                    CalculatePath();
                }
                //SetState(GoblinState.idle);   //先進idle或ramble再尋路，以免尋路過久會不知要做啥
            }
        }
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
            if (inStateTime > 6.5f) SetState(GoblinState.ramble);
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
            if (inStateTime > totalTime && !followingPath) SetState();   //沒有找到尋路繼續idle或ramble
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
                if (Physics.Raycast(selfPos, moveFwdDir, 3.0f, 1 << LayerMask.NameToLayer("Barrier")))
                {
                    float degree = Random.Range(135.0f, 225.0f);
                    moveFwdDir = Quaternion.AngleAxis(degree, Vector3.up) * moveFwdDir;
                    float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                }
                transform.position += deltaTime * speed * moveFwdDir;
            }
            else if(!followingPath) SetState();   //沒有找到尋路繼續idle或ramble
        }
    }

    public void ClosePlayer() {
        if (firstInState)
        {
            startFindPath = false;
            animator.SetInteger("state", 1);
            animator.speed = 1.0f;
            firstInState = false;
            //followingPath = false;
            //PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
        }
        else
        {
            inStateTime += deltaTime;
            if (inStateTime > 2.0f) {
                followingPath = false;
                SetState();
                return;
            }

            Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
            if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                {
                    followingPath = false;
                    SetState();
                }
                else
                {
                    pathIndex++;
                    moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                    float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                }
            }
            transform.position += deltaTime * speed * moveFwdDir;

        }
    }

    public virtual void CalculatePath() {
        startFindPath = true;
        //followingPath = false;


        if(!closingPlayer)curPathRequest =  PathRequestManager.RequestPath( curPathRequest,selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
        else curPathRequest = PathRequestManager.RequestPath(curPathRequest, selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFoundClosePlayer);
        if (curPathRequest != null)
        {
            goblinManager.CalculatePath = true;
        }

    }

    public virtual void Chase()
    {
        if (firstInState)
        {
            startFindPath = false;
            animator.SetInteger("state", 1);
            animator.speed = 1.2f;
            firstInState = false;
            blankTime = .0f;
            //followingPath = false;
            //PathRequestManager.RequestPath(selfPos, goblinManager.PlayerPos[targetPlayer], OnPathFound);
        }
        else {
            if (inStateTime < 0.3f)
            {
                inStateTime += deltaTime;
            }
            else {
                if (goblinManager.PlayersMove[targetPlayer])
                {
                    CalculatePath();
                    inStateTime = 0.0f;
                }
            }

            Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
            if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                {
                    followingPath = false;
                    SetState(GoblinState.attackBreak);
                    //OverAttackDetectDist();
                }
                else
                {
                    pathIndex++;
                    moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                    float scaleX = 1.0f;
                    if (moveFwdDir.x > 0.3f) scaleX = -1.0f;
                    else if (moveFwdDir.x < -0.3f) scaleX = 1.0f;
                    image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                }
            }
            transform.position += deltaTime * speed * 1.2f * moveFwdDir;


            //if (followingPath) {

            //}
        }
    }

    public virtual void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        curPathRequest = null;
        if (pathSuccessful)
        {
            if (curState == GoblinState.idle || curState == GoblinState.ramble || curState == GoblinState.chase || curState == GoblinState.attackBreak) {
                path = new PathFinder.Path(waypoints, selfPos, turnDist);
                followingPath = true;
                pathIndex = 0;
                moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                SetState(GoblinState.chase);
            }     
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
        }
        else
        {
            //Debug.Log("Can't Find");
            SetState(GoblinState.erroeCatch);
        }
    }

    public virtual void OnPathFoundClosePlayer(Vector3[] waypoints, bool pathSuccessful)
    {
        //Debug.Log(transform.name + "     Find");
        curPathRequest = null;
        if (pathSuccessful)
        {
            if (curState == GoblinState.idle || curState == GoblinState.ramble || curState == GoblinState.attackBreak)
            {
                path = new PathFinder.Path(waypoints, selfPos, turnDist);
                followingPath = true;
                pathIndex = 0;
                moveFwdDir = new Vector3(path.lookPoints[pathIndex].x - selfPos.x, 0, path.lookPoints[pathIndex].z - selfPos.z).normalized;
                float scaleX = (moveFwdDir.x > .0f) ? -1.0f : 1.0f;
                image.localScale = new Vector3(scaleX * imgScale, imgScale, imgScale);
                SetState(GoblinState.closePlayer);
            }
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
        }
    }

    public virtual void Attack()
    {
        
    }

    public virtual void AttackBreak() {
        if (firstInState) {
            animator.speed = 1.0f;
            animator.SetInteger("state", 0);
            animator.SetTrigger("attackOver");
            startFindPath = false;
            followingPath = false; //需註記尋路結束
            closingPlayer = false;
        }
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
            if (curPathRequest != null) {
                PathRequestManager.CancleRequest(curPathRequest);
                curPathRequest = null;
            } 
            firstInState = false;
            animator.SetTrigger("hurt");
            //animator.Play("hurt");
            animator.SetInteger("state",3);
            renderer.material.SetFloat("_WhiteScale", 1.0f);
        }
        else {
            if (!Physics.Raycast(selfPos, hurtDir, 2.0f, 1 << LayerMask.NameToLayer("Barrier")))
            {
                transform.position += (backSpeed) * deltaTime * hurtDir;
            }

            backSpeed -= deltaTime * 15.0f;
            if (backSpeed <= .0f) backSpeed = .0f;
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (aniInfo.IsName("hurt"))Debug.Log(aniInfo.normalizedTime);
            if (aniInfo.IsName("hurt") && aniInfo.normalizedTime >= 0.95f) {
                //if (hp <= 0) SetState(GoblinState.die);
                SetState(GoblinState.attackBreak); //OverAttackDetectDist();
                backSpeed = 10.0f;
                //renderer.material.SetInt("_colorID", color);
                renderer.material.SetFloat("_WhiteScale", -1);
                HurtOverDecideTargetPlayer();  //被打完判斷最近玩家
            }
        }
    }

    public virtual void KnockBack() {
        if (firstInState)
        {
            if (curPathRequest != null) {
                PathRequestManager.CancleRequest(curPathRequest);
                curPathRequest = null;
            } 
            firstInState = false;
            //animator.SetTrigger("hurt");
            //animator.Play("hurt");
            //animator.SetInteger("state", 3);
        }
        else
        {
            if (!Physics.Raycast(selfPos, hurtDir, 2.0f, 1 << LayerMask.NameToLayer("Barrier")))
            {
                transform.position += (backSpeed) * deltaTime * hurtDir;
            }

            backSpeed -= deltaTime * 30.0f;
            if (backSpeed <= .0f) backSpeed = .0f;
            aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (aniInfo.IsName("hurt"))Debug.Log(aniInfo.normalizedTime);
            if (inStateTime > 0.3f)
            {
                
                SetState(GoblinState.attackBreak); //OverAttackDetectDist();
                backSpeed = 10.0f;
            }
        }
    }

    public virtual void Die()
    {
        
    }
    public virtual void OnGettingHurt(int col, int atkValue, int playerID, Vector3 dir)
    {
        if (hp <= 0) return;
        targetPlayer = playerID;
        targetPlayer2 = playerID;
        if (col == color)
        {
            AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
            hp -= atkValue;
            whiteScale = 1.0f;
            //targetPlayer = playerID;
            //targetPlayer2 = playerID;

            if (hp > 0)
            {
                if (curState != GoblinState.hurt)
                {
                    hurtDir = dir.normalized;
                    SetState(GoblinState.hurt);
                }

            }
            else
            {
                hurtDir = dir.normalized;
                SetState(GoblinState.die);
            }
        }
        else
        {
            //targetPlayer = playerID;
            //targetPlayer2 = playerID;
            if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt && curState != GoblinState.attack)
            {
                hurtDir = dir.normalized;
                SetState(GoblinState.fakeHurt);
            }
            AudioManager.SingletonInScene.PlayRandomWrong(1f);
        }

        //if (col == 1 || col == 2 || col == 4)
        //{

        //}
        //else {
        //    if (col == 3)
        //    {
        //        if ((color == 1 || color == 2 || color == 3))
        //        {
        //            if (hp > 0)
        //            {
        //                AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //                hp -= atkValue;
        //                targetPlayer = playerID;
        //                targetPlayer2 = playerID;
        //                moveFwdDir = dir.normalized;
        //                SetState(GoblinState.hurt);
        //            }
        //        }
        //        else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //    }
        //    else if (col == 5) {
        //        if ((color == 1 || color == 4 || color == 5))
        //        {
        //            if (hp > 0)
        //            {
        //                AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //                hp -= atkValue;
        //                targetPlayer = playerID;
        //                targetPlayer2 = playerID;
        //                moveFwdDir = dir.normalized;
        //                SetState(GoblinState.hurt);
        //            }
        //        }
        //        else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //    }
        //    else if (col == 6)
        //    {
        //        if ((color == 2 || color == 4 || color == 6))
        //        {
        //            if (hp > 0)
        //            {
        //                AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //                hp -= atkValue;
        //                targetPlayer = playerID;
        //                targetPlayer2 = playerID;
        //                moveFwdDir = dir.normalized;
        //                SetState(GoblinState.hurt);
        //            }
        //        }
        //        else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //    }
        //}
    }
    public virtual void OnGettingHurt(int col, int atkValue, int playerID,int playerID2, Vector3 dir)
    {
        if (hp <= 0) return;
        targetPlayer = playerID;
        targetPlayer2 = playerID2;
        //if (col == 1 || col == 2 || col == 4)
        //{
        //    if (col == color)
        //    {
        //        if (hp > 0)
        //        {
        //            AudioManager.SingletonInScene.PlaySound2D("Currect_Color", 0.35f);
        //            hp -= atkValue;
        //            targetPlayer = playerID;
        //            targetPlayer2 = playerID2;
        //            moveFwdDir = dir.normalized;
        //            SetState(GoblinState.hurt);
        //        }
        //    }
        //    else AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 1f);
        //}
        if (col == 3 || col == 5 || col == 6)//else
        {
            if (col == 3)
            {
                if ((color == 1 || color == 2 || color == 3))
                {
                    AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
                    hp -= atkValue;
                    whiteScale = 1.0f;
                    //targetPlayer = playerID;
                    //targetPlayer2 = playerID2;
                    if (hp > 0)
                    {
                        if (curState != GoblinState.hurt)
                        {
                            hurtDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                    }
                    else
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.die);
                    }
                }
                else {
                    //targetPlayer = playerID;
                    //targetPlayer2 = playerID2;
                    if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt)
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.fakeHurt);
                    }
                    AudioManager.SingletonInScene.PlayRandomWrong(1f);
                } 
            }
            else if (col == 5)
            {
                if ((color == 1 || color == 4 || color == 5))
                {
                    AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
                    hp -= atkValue;
                    whiteScale = 1.0f;
                    //targetPlayer = playerID;
                    //targetPlayer2 = playerID2;
                    if (hp > 0)
                    {
                        if (curState != GoblinState.hurt)
                        {
                            hurtDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                    }
                    else
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.die);
                    }
                }
                else {
                    //targetPlayer = playerID;
                    //targetPlayer2 = playerID2;
                    if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt)
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.fakeHurt);
                    }
                    AudioManager.SingletonInScene.PlayRandomWrong(1f);
                }
            }
            else if (col == 6)
            {
                if ((color == 2 || color == 4 || color == 6))
                {
                    AudioManager.SingletonInScene.PlayRandomCorrect(0.42f);
                    hp -= atkValue;
                    whiteScale = 1.0f;
                    //targetPlayer = playerID;
                    //targetPlayer2 = playerID2;
                    if (hp > 0)
                    {
                        if (curState != GoblinState.hurt)
                        {
                            hurtDir = dir.normalized;
                            SetState(GoblinState.hurt);
                        }
                    }
                    else
                    {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.die);
                    }
                }
                else {
                    //targetPlayer = playerID;
                    //targetPlayer2 = playerID2;
                    if (curState != GoblinState.hurt && curState != GoblinState.fakeHurt) {
                        hurtDir = dir.normalized;
                        SetState(GoblinState.fakeHurt);
                    }
                    AudioManager.SingletonInScene.PlayRandomWrong(1f);
                } 
            }
        }
    }


    public virtual void ForceRamble() {

    }

    public virtual void GrowMaxHp() {
        maxHp += 2;
        hp = maxHp;
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
            if (inStateTime > 1.5f) SetState();
        }
    }

    public void SetGameOver() {
        animator.speed = .0f;
    }

}
