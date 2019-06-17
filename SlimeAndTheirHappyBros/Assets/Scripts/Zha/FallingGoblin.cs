using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGoblin : IEnemyObjectPoolUnit
{
    int chaseID = 0;
    float time = .0f;
    Transform transform, goblinTransform, hintTransform;
    GoblinManager goblinManager;


    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinManager = manager;
    }

    public void ToActive(Vector3 pos, Vector3 dir)
    {
        transform.position = pos;
    }

    // Update is called once per frame
    public void Update( float dt)
    {
        
    }
    public void ResetUnit()
    {
        transform.gameObject.SetActive(false);
    }
}
