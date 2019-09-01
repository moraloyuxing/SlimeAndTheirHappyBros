using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinCountDown : MonoBehaviour {
    public JoinGame _playermanager;
    public Sprite[] CountSprite = new Sprite[10];
    public Image CountDown_Img;
    Animator anim;
    int Base_Timer = 5;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        // if (Input.GetKeyDown(KeyCode.M)) Start_CountDown();
    }

    public void Start_CountDown() {
        InvokeRepeating("Join_Timer", 1.0f, 1.0f);
    }

    void Join_Timer() {
        Base_Timer--;
        if (Base_Timer < 0){
            CancelInvoke("Join_Timer");
            _playermanager.Fake_Ready();
        }
        else {
            CountDown_Img.sprite = CountSprite[Base_Timer];
            anim.Play("TitleCountDown");
        }
    }

    public void ForceCancelCD() {
        CancelInvoke("Join_Timer");
    }

}
