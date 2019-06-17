using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnergyPunch : IEnemyObjectPoolUnit
{
    int countID;
    float speed = 50.0f;
    Transform transform;
    GoblinManager goblinManager;

    Vector3 dir;
    Vector3[] punchPos = new Vector3[4] {new Vector3(-6.95f,-1.7f,20.28f), new Vector3(4.2f,-1.7f,20.28f), new Vector3(13.99f,-1.7f,17.94f), new Vector3(-18.42f,-1.7f, 17.21f) };
    Vector3[] punchDir = new Vector3[4];

    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinManager = manager;

        float rad = 230.0f * Mathf.Deg2Rad;
        punchDir[0] = new Vector3(Mathf.Cos(rad),0,Mathf.Sin(rad));
        punchDir[1] = new Vector3(-punchDir[0].x,0, punchDir[0].z);
        rad = 290.0f * Mathf.Deg2Rad;
        punchDir[2] = new Vector3(Mathf.Cos(rad),0, Mathf.Sin(rad));
        punchDir[3] = new Vector3(-punchDir[2].x,0, punchDir[2].z);

    }
    public void ToActive(int count) {
        countID = count;
        transform.position = punchPos[countID];
        dir = punchDir[countID];
        transform.gameObject.SetActive(true);
    }
    public void ToActive(Vector3 pos, Vector3 dir) {

    }
    public void Update(float dt) {
        transform.position += dt * speed * dir;
    }
    public void ResetUnit() {
        transform.gameObject.SetActive(false);
    }
}
