using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinManager : MonoBehaviour
{
    List<NormalGoblin> freeNormalGoblins, UsedNormalGoblins;


    // Start is called before the first frame update
    private void Awake()
    {
        Transform goblin;
        Transform goblins;
        goblins = transform.Find("NormalGoblins");
        freeNormalGoblins = new List<NormalGoblin>();
        for (int i = 0; i < goblins.childCount; i++) {
            goblin = goblins.GetChild(i);
            freeNormalGoblins.Add(goblin.GetComponent<NormalGoblin>());
            freeNormalGoblins[i].Init(goblin);
            freeNormalGoblins[i].SubCallback(Recycle);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Recycle<T>(T goblin) where T : IEnemyUnit {
        if (goblin is NormalGoblin) {
            freeNormalGoblins.Add(goblin as NormalGoblin);
        }
        
    }

}
