using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pool : MonoBehaviour{

    //群組
    public GameObject Group_Bullet;
    public GameObject Group_MergeSlime;

    //子彈
    public GameObject prefabBullet;
    public int initailsize = 50;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    //合體史萊姆
    public MultiPlayerCamera CameraFocus;
    public GameObject prefabMergeSlime;
    public Bullet_Manager _bulletpool;
    int ini = 2;
    private Queue<GameObject> _MSlimepool = new Queue<GameObject>();

    void Awake(){
        //生成合體史萊姆
        for (int cnt = 0; cnt < ini; cnt++){
            GameObject clone_MergeSlime = Instantiate(prefabMergeSlime) as GameObject;
            clone_MergeSlime.GetComponent<Merge_Control>().SetMSlimePool(_bulletpool,this);
            _MSlimepool.Enqueue(clone_MergeSlime);
            clone_MergeSlime.transform.SetParent(Group_MergeSlime.transform);
            //推入攝影機
            CameraFocus.AllPlayers.Add(clone_MergeSlime.transform);
            clone_MergeSlime.SetActive(false);
        }
    }

    //合體史萊姆
    public void MSlime_Reuse(Vector3 position, Quaternion rotation,int Dyeing_Color,  GameObject PlayerA,GameObject PlayerB) {
        if (_MSlimepool.Count > 0){
            GameObject MSlime_reuse = _MSlimepool.Dequeue();
            MSlime_reuse.transform.position = position;
            MSlime_reuse.transform.rotation = rotation;
            MSlime_reuse.SetActive(true);
            MSlime_reuse.GetComponent<Merge_Control>().SetUp_DyeingColor(Dyeing_Color);
            MSlime_reuse.GetComponent<Merge_Control>().Decide_TwoPlayer_Control(PlayerA, PlayerB);
        }

        else{
            GameObject clone_MSlime = Instantiate(prefabMergeSlime) as GameObject;
            clone_MSlime.transform.position = position;
            clone_MSlime.transform.rotation = rotation;
            clone_MSlime.GetComponent<Merge_Control>().SetUp_DyeingColor(Dyeing_Color);
            clone_MSlime.GetComponent<Merge_Control>().Decide_TwoPlayer_Control(PlayerA, PlayerB);
        }
    }


    public void MSlime_Recovery(GameObject recovery) {
        _MSlimepool.Enqueue(recovery);
        recovery.SetActive(false);
    }

}
