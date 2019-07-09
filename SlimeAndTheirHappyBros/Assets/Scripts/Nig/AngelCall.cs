using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCall : MonoBehaviour{
    public NPC_Manager _npcManager;

    public void Angel_StandBy(){
        _npcManager.Angel_Ready = true;
    }


}
