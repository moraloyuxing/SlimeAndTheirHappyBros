using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinWave : IEnemyObjectPoolUnit
{
    int count = 72;
    float lifeTime = .0f;
    Vector3[] pointPos;
    LineRenderer lineRender;
    Transform transform;
    GoblinManager goblinManager;


    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinManager = manager;
        lineRender = transform.GetComponent<LineRenderer>();
        lineRender.positionCount = count;
        pointPos = new Vector3[count];
        //lineRender.startWidth = 1.0f;
        //lineRender.endWidth = 1.0f;
    }

    public void Update(float dt)
    {
        lifeTime += dt;
        int i = 0;
        while (i < count)
        {
            float upD = (i * 5) * Mathf.Deg2Rad;
            float downD = (180 + i * 5) * Mathf.Deg2Rad;
            lineRender.SetPosition(i, pointPos[i] + new Vector3(Mathf.Cos(upD), 0, Mathf.Sin(upD)));
            lineRender.SetPosition(i + 1, pointPos[i+1] + new Vector3(Mathf.Cos(downD), 0, Mathf.Sin(downD)));
            i += 2;
        }
    }

    public void ToActive(Vector3 _pos, Vector3 _dir) {
        transform.position = _pos;
        int i = 0;
        while (i < count)
        {
            float upD = (i * 5) * Mathf.Deg2Rad;
            float downD = (180 + i*5) * Mathf.Deg2Rad;
            Vector3 upPoint = _pos + 10.0f * new Vector3(Mathf.Cos(upD), 0, Mathf.Sin(upD));
            Vector3 downPoint = _pos + 10.0f * new Vector3(Mathf.Cos(downD), 0, Mathf.Sin(downD));
            lineRender.SetPosition(i, upPoint);
            pointPos[i] = upPoint;
            lineRender.SetPosition(i + 1,  downPoint);
            pointPos[i + 1] = downPoint;

            i += 2;
        }
        transform.gameObject.SetActive(true);
    }

    public void ResetUnit() {
        lifeTime = .0f;
        transform.gameObject.SetActive(false);

    }
}
