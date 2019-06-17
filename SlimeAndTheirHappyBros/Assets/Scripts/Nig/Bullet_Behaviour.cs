using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behaviour : MonoBehaviour{
    int BulletATK = 0;
    int color;
    float  offset, scaleOffset = 1.0f;
    float FadeTime = 0.75f;
    public Player_Control WhichPlayer;
    public Player_Control WhichPlayer2;
    public Player_Control Rescue_Which;
    public float speed = 20.0f;
    public float alpha = -0.04f;
    public float recoveryTime = 2.0f;
    private float _timer;
    private Transform _myTransform;
    Color BulletAlpha;
    Bullet_Manager bulletPool;
    Vector3 Attack_Dir;
    int PenetrateMaxCount = 1;
    int NowPenetrate = 0;
    bool isLeaf = false;
    List<Collider> colliderRecord = new List<Collider>();

    void Awake(){
        _myTransform = transform;
        BulletAlpha = GetComponent<SpriteRenderer>().color;
    }

    public void SetPool(Bullet_Manager pool) {
        bulletPool = pool;
    }

    void OnEnable(){
        _timer = Time.time;
        BulletAlpha.a = 1.0f;
        GetComponent<SpriteRenderer>().color = BulletAlpha;
        transform.localEulerAngles = new Vector3(20.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void SetAttackDir(Vector3 current_angle,Player_Control xSlime,int Shader_Number,bool PlayerDeath) {
        if (PlayerDeath == false){
            GetComponent<Animator>().enabled = true;
            FadeTime = FadeTime + xSlime.Base_BulletTime;
            speed = 20.0f * xSlime.Base_BulletSpeed;
            scaleOffset = xSlime.Base_BulletScale;
            _myTransform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
            PenetrateMaxCount = xSlime.Base_Penetrate;
            BulletATK = xSlime.Base_ATK;
        }
        else LeafType_Initial();

        isLeaf = PlayerDeath;
        color = Shader_Number;
        GetComponent<SpriteRenderer>().material.SetInt("_colorID", Shader_Number);
        WhichPlayer = xSlime;
        WhichPlayer2 = xSlime;
        Attack_Dir = current_angle.normalized;
        offset = Mathf.Abs(Attack_Dir.x);
        offset = Mathf.Clamp(offset, 0.5f, 0.8f);

        //speed = 20.0f * Mathf.Pow(1.25f, xSlime.BulletSpeed_Superimposed);
        Attack_Dir *= speed;

    }

    public void SetAttackDir(Vector3 current_angle, Player_Control xSlime,Player_Control xSlime2, int Shader_Number,Merge_Control xMSlime){
        FadeTime = FadeTime + xSlime.Base_BulletTime;
        color = Shader_Number;
        GetComponent<SpriteRenderer>().material.SetInt("_colorID", Shader_Number);
        WhichPlayer = xSlime;
        WhichPlayer2 = xSlime2;
        Attack_Dir = current_angle.normalized;
        offset = Mathf.Abs(Attack_Dir.x);
        offset = Mathf.Clamp(offset, 0.5f, 0.8f);
        scaleOffset = xMSlime.Base_BulletScale;
        //scaleOffset = Mathf.Pow(1.25f, xMSlime.Bullet_Superimposed);
        //speed = 20.0f * Mathf.Pow(1.25f, xMSlime.BulletSpeed_Superimposed);
        speed = 20.0f * xSlime.Base_BulletSpeed;
        Attack_Dir *= speed;
        _myTransform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
        PenetrateMaxCount = xMSlime.Base_Penetrate;
        BulletATK = xMSlime.Base_ATK;
    }

    void Update(){
        if (!gameObject.activeInHierarchy) return;
        if (Time.time > _timer + FadeTime) {
            BulletAlpha.a = BulletAlpha.a + alpha;
            GetComponent<SpriteRenderer>().color = BulletAlpha;
        }

        if (BulletAlpha.a <= 0.0f) {
            colliderRecord.Clear();
            NowPenetrate = 0;
            bulletPool.Bullet_Recovery(gameObject);
        }

        _myTransform.position += new Vector3(Attack_Dir.x*Time.deltaTime,0,Attack_Dir.z*Time.deltaTime);
        _myTransform.position = new Vector3(_myTransform.position. x, _myTransform.position.y,  _myTransform.position.z);
        Bullet_Detect();
    }

    void Bullet_Detect() {
        Collider[]colliders = Physics.OverlapBox(_myTransform.position, scaleOffset * new Vector3(0.22f, 0.15f, 0.025f), Quaternion.Euler(25, 0, 0), 
            1 << LayerMask.NameToLayer("GoblinHurtArea") | 1<< LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("PlayerReviveArea"));
        int i = 0;
        while (NowPenetrate < PenetrateMaxCount && i < colliders.Length ){
            Transform c = colliders[i].transform.parent;

            if (!colliderRecord.Contains(colliders[i])) {
                colliderRecord.Add(colliders[i]);
                if (c.tag == "Goblin"){
                    NowPenetrate++;
                    if (WhichPlayer != WhichPlayer2)bulletPool._goblinmanager.FindGoblin(c.name).OnGettingHurt(color, BulletATK, WhichPlayer.PlayerID, WhichPlayer2.PlayerID2, Attack_Dir);
                    else bulletPool._goblinmanager.FindGoblin(c.name).OnGettingHurt(color, BulletATK, WhichPlayer.PlayerID, Attack_Dir);
                }

                if (c.tag == "Player" && isLeaf == false) {
                    Rescue_Which = c.GetComponent<Player_Control>();
                    Rescue_Which.GetRescued();
                }

                if (colliders[i].tag == "Barrier" || c.tag == "Barrier") {
                    NowPenetrate++;
                    AudioManager.SingletonInScene.PlaySound2D("Mistake_Color", 0.5f);
                    NowPenetrate = PenetrateMaxCount;//抵達障礙物直接給最大值，取消繼續穿透
                }

                if (NowPenetrate == PenetrateMaxCount) {
                    Attack_Dir = Vector3.zero;
                    if (c.tag == "Player" || isLeaf == true) ExplosionEnd();
                    else GetComponent<Animator>().Play("SlimeBullet_Explosion");
                } 
            }
            i++;
        }
    }

    public void ExplosionEnd() {
        colliderRecord.Clear();
        NowPenetrate = 0;
        bulletPool.Bullet_Recovery(gameObject);
    }

    //死亡子彈為落葉，數值重置
    void LeafType_Initial() {
        GetComponent<Animator>().enabled = false;
        FadeTime = 0.75f;//子彈存活時間
        speed = 20.0f;//子彈飛行速度
        _myTransform.localScale = new Vector3(1.6f, 1.0f,1.0f);
        PenetrateMaxCount =1;//子彈穿透數量
        BulletATK = 0;//子彈攻擊力(無色無攻)
    }

}
