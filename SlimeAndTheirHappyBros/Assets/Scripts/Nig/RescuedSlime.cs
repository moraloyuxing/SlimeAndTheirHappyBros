using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可被救的Area及相關function
//救起後觸發後續動畫
//動畫播畢(抵達房屋內)呼叫TutorialStep確認條件，完成後觸發號角音效，進入Day1流程

public class RescuedSlime : MonoBehaviour{

    TutorialStep _tutorialstep;
    int Rescue_count = 0;
    public BoxCollider ReviveArea;
    Animator anim;

    void Start(){
        _tutorialstep = GameObject.Find("GameManager").GetComponent<TutorialStep>();
        anim = GetComponent<Animator>();
        switch (gameObject.name) {
            case "RescueSlime_1":
                anim.SetInteger("RescueNumber", 1);
                break;
            case "RescueSlime_2":
                anim.SetInteger("RescueNumber", 2);
                break;
            case "RescueSlime_3":
                anim.SetInteger("RescueNumber", 3);
                break;
        }
        AudioManager.SingletonInScene.PlaySound2D("Slime_Jump_Death", 0.55f);
    }

    void Update(){
        
    }

    //開啟救援觸發區域
    public void ReviveArea_Switch() {
        ReviveArea.enabled = true;
    }

    //被救援時放出動畫
    public void GetRescued(){
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Slime_CureEffect")) { anim.SetTrigger("InterruptCure"); }
        GetComponent<Animator>().Play("Slime_CureEffect");
        Rescue_count++;
        AudioManager.SingletonInScene.PlaySound2D("Heal", 0.5f);

        if (Rescue_count >= 3) {
            Rescue_count = 0;
            ReviveArea.enabled = false;
            GetComponent<Animator>().Play("Slime_Revive_Tut");
            AudioManager.SingletonInScene.PlaySound2D("Revive", 0.5f);
        }

    }

    //抵達房屋，送出確認條件
    public void ArriveHouse() {
        _tutorialstep.CheckStepProgress();
    }
}
