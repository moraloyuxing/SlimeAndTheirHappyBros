using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_Bullet : MonoBehaviour{
    public Player_Control Player;
    public Merge_Control MSlimePlayer;
    public GameObject Hint_Arrow;
    public Bullet_Manager _bulletPool;
    Vector3 shooting_direction;
    int PlayerID;
    int PlayerID2;
    bool From_MSlime = false;

    void Start(){
        if(Player !=null)PlayerID = Player.PlayerID;
        if (_bulletPool == null) _bulletPool = GameObject.Find("Bullet_Group").GetComponent<Bullet_Manager>();
    }

    public void SetMSlimeMovingPlayer(int MovingID,int ShootingID){
        PlayerID = MovingID;
        PlayerID2 = ShootingID;
        From_MSlime = true; 
    }

    public void ShootBullet(Vector3 current_angle,int Shader_Number) {
        if(From_MSlime == false)_bulletPool.Bullet_ReUse(Player.transform.position, Hint_Arrow.transform.rotation, current_angle,PlayerID,Shader_Number);
        else _bulletPool.Bullet_ReUse(MSlimePlayer.transform.position, Hint_Arrow.transform.rotation, current_angle, PlayerID,PlayerID2, Shader_Number);
    }
}
