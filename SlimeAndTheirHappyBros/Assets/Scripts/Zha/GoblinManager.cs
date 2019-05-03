using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinManager : MonoBehaviour
{
    List<NormalGoblin>normalGoblins;


    // Start is called before the first frame update
    private void Awake()
    {
        Transform goblin;
        Transform goblins;
        goblins = transform.Find("NormalGoblins");
        normalGoblins = new List<NormalGoblin>();
        for (int i = 0; i < goblins.childCount; i++) {
            goblin = goblins.GetChild(i);
            normalGoblins.Add(goblin.GetComponent<NormalGoblin>());
            normalGoblins[i].Init(goblin);
            //normalGoblins[i].SubCallback(Recycle);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Recycle(NormalGoblin goblin) {

    }

}
