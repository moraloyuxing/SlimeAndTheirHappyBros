using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour{
    public Transform[] All_Player = new Transform[4]; 
    public GameObject[] All_Item = new GameObject[6];
    int[] Item_Focused = new int[6];


    void Start(){
        for (int i = 0; i < 6; i++) Item_Focused[i] = 0;
    }

    void Update(){
        for (int i = 0; i < 6; i++) {
            for (int k = 0; k < 4; k++) {

            }
        }
    }

    public void ShowWhichIntro(GameObject ItemName) {
        for (int i = 0; i < 6; i++) {
            if (All_Item[i] == ItemName) {
                All_Item[i].transform.GetChild(0).gameObject.SetActive(true);
                Item_Focused[i]++;
            }
        }
    }

    public void Decrease_FocusCount(GameObject ItemName) {
        for (int i = 0; i < 6; i++) {
            if (All_Item[i] == ItemName) Item_Focused[i]--;
            if (Item_Focused[i] == 0) All_Item[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

}
