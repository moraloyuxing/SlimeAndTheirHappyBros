using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : IEnemyObjectPoolUnit
{
    float speed;
    Transform transform;

    GoblinManager goblinManager;


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager manager) {
        transform = t;
        goblinManager = manager;

    }
    public void ToActive(float _speed, Vector3 _pos, Vector3 _dir)
    {

    }
    public void Update() {

    }

    public void ResetUnit() {

    }
}
