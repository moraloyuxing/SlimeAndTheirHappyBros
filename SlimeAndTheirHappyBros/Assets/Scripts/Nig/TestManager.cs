using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour{
    public Sprite[] ItemSprite = new Sprite[6];
    public Player_Control[] FourPlayer = new Player_Control[4];

    void Update(){
        //if (Input.GetKeyDown(KeyCode.T)) {for (int p = 0; p < 4; p++) FourPlayer[p].Ability_Modify(0, ItemSprite[0], 0);}
        //if (Input.GetKeyDown(KeyCode.Y)) { for (int p = 0; p < 4; p++) FourPlayer[p].Ability_Modify(1, ItemSprite[1], 0); }
        //if (Input.GetKeyDown(KeyCode.U)) { for (int p = 0; p < 4; p++) FourPlayer[p].Ability_Modify(2, ItemSprite[2], 0); }
        //if (Input.GetKeyDown(KeyCode.I)) { for (int p = 0; p < 4; p++) FourPlayer[p].Ability_Modify(3, ItemSprite[3], 0); }
        //if (Input.GetKeyDown(KeyCode.O)) { for (int p = 0; p < 4; p++) FourPlayer[p].Ability_Modify(4, ItemSprite[4], 0); }
        //if (Input.GetKeyDown(KeyCode.P)) { for (int p = 0; p < 4; p++) FourPlayer[p].Ability_Modify(5, ItemSprite[5], 0); }
    }

}
