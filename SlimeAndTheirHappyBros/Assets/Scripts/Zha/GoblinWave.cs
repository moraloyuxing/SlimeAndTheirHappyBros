using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinWave : IEnemyObjectPoolUnit
{
    int halfCount = 55;
    float degree = 2;
    float speed = 30.0f, lifeTime = .0f;
    Vector3[] pointPos, pointVec;
    LineRenderer lineRender;
    Transform transform;
    GoblinManager goblinManager;


    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinManager = manager;
        lineRender = transform.GetComponent<LineRenderer>();
        lineRender.positionCount = halfCount*2 + 1;
        pointPos = new Vector3[halfCount * 2 + 1];
        pointVec = new Vector3[halfCount * 2  + 1];
        //lineRender.startWidth = 1.0f;
        //lineRender.endWidth = 1.0f;
    }

    public void Update(float dt)
    {
        lifeTime += dt;

        pointPos[halfCount] += dt * speed * pointVec[halfCount];
        lineRender.SetPosition(halfCount, pointPos[halfCount]);

        int i = 1;
        while (i <= halfCount)
        {
            pointPos[halfCount - i] += dt * speed * pointVec[halfCount - i];
            lineRender.SetPosition(halfCount - i, pointPos[halfCount - i]);
            pointPos[halfCount + i] += dt * speed * pointVec[halfCount + i];
            lineRender.SetPosition(halfCount + i, pointPos[halfCount + i]);
            i ++;
        }

        if (lifeTime > 3.5f) ResetUnit();
    }

    public void ToActive(Vector3 _pos, Vector3 _dir) {
        Vector3 pos = new Vector3(_pos.x,0,_pos.z);

        lineRender.SetPosition(halfCount, pos + new Vector3(0,0,-15.0f));
        pointPos[halfCount] = lineRender.GetPosition(halfCount);
        pointVec[halfCount] = new Vector3(0, 0, -1.0f);

        int i = 1;
        while (i <= halfCount)
        {
            float downDegree = (270 - i * degree) * Mathf.Deg2Rad;
            float upDegree = (270 + i * degree) * Mathf.Deg2Rad;

            Vector3 downVec = new Vector3(Mathf.Cos(downDegree),0,Mathf.Sin(downDegree));
            Vector3 upVec = new Vector3(Mathf.Cos(upDegree), 0, Mathf.Sin(upDegree));
            Debug.Log("degree: " + (270 - i * degree) + "   " + downVec);
            Vector3 downPoint = pos + 15.0f * downVec;
            Vector3 upPoint = pos + 15.0f * upVec;
            Debug.Log(downPoint);
            lineRender.SetPosition(halfCount - i, downPoint);
            pointPos[halfCount - i] = downPoint;
            pointVec[halfCount - i] = downVec;
            lineRender.SetPosition(halfCount + i, upPoint);
            pointPos[halfCount + i] = upPoint;
            pointVec[halfCount + i] = upVec; 
            i ++;
        }
        transform.gameObject.SetActive(true);
    }

    public void ResetUnit() {
        lifeTime = .0f;
        transform.gameObject.SetActive(false);
        goblinManager.RecycleWave(this);

    }
}
