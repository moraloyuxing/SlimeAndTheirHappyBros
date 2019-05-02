using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGoblin: IEnemyUnit
{
    Transform transform;
    Animator animator;

    // Start is called before the first frame update
    public void Init(Transform t) {
        transform = t;

    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public void DecideState() {

    }
    public void StateMachine() {

    }
    public void Idle() {

    }
    public void Ramble() {

    }
    public void Chase() {

    }
    public void Attack() {

    }
    public void GetHurt() {

    }
    public void Die() {

    }
    public void OnGetHurt(int value) {

    }
    public void ResetUnit() {

    }
}

}
