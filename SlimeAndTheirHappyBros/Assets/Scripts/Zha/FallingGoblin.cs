using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGoblin : IEnemyObjectPoolUnit
{
    int traceID = 0;
    float time = .0f, scaleTime = .0f, traceSpeed = 20.0f, height = 50.0f,fallingSpeed = .0f, addSpeed = 5.0f;
    Transform transform, goblinTransform, hintTransform;
    GoblinManager goblinManager;
    Vector3 smallScale, totalScale;
    Collider hurtCollider;

    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinTransform = t.Find("goblin");
        hintTransform = t.Find("hintSprite");
        hurtCollider = goblinTransform.GetComponent<Collider>();
        goblinManager = manager;
        smallScale = new Vector3(1f,1f,1f);
        totalScale = new Vector3(7.5f,3f,1f);
    }

    public void ToActive(Vector3 pos, Vector3 dir)
    {
        transform.position = pos;
    }
    public void ToActive(int id)
    {
        transform.position = goblinManager.PlayerPos[id] + 8.0f * new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f)).normalized;
        traceID = id;
        transform.gameObject.SetActive(true);
    }

    // Update is called once per frame
    public void Update( float dt)
    {
        time += dt;
        if (time <= 1.25f)
        {
            Vector3 dir = new Vector3(goblinManager.PlayerPos[traceID].x - transform.position.x,.0f,goblinManager.PlayerPos[traceID].z - transform.position.z);
            if (dir.sqrMagnitude > 0.04f) {
                transform.position += traceSpeed * dt * dir.normalized;
            }
            scaleTime += dt*0.8f;
            hintTransform.localScale = Vector3.Lerp(smallScale, totalScale, scaleTime);
        }
        else if (time >= 1.8f) {
            if (goblinTransform.localPosition.y > 0) {
                fallingSpeed += addSpeed * dt;
                Vector3 nextPos = goblinTransform.localPosition + new Vector3(0, -fallingSpeed, 0);
                if (nextPos.y < 0) goblinTransform.localPosition = new Vector3(0, 0, 0);
                else goblinTransform.localPosition = nextPos;
            }
            else {
                hurtCollider.enabled = false;
                if (time > 4.0f) ResetUnit();
            }

        }
    }
    public void ResetUnit()
    {
        time = .0f;
        fallingSpeed = .0f;
        hintTransform.localScale = smallScale;
        goblinTransform.localPosition = new Vector3(0, height, 0);
        hurtCollider.enabled = true;
        transform.gameObject.SetActive(false);
        goblinManager.RecyleFallingGoblin(this);
    }
}
