﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    bool progressMove = false;
    int tutorialProgres = 0;
    float length, progressTime = .0f;

    Vector2 oringinProgress, targetProgress;

    RectTransform goblinHeadTrans;

    Image tutorialImg; 
    Image goblinProgress, goblinHead;
    Image breakTimeImg, roundImg;

    Animator animator;

    System.Action countDownCBK;

    public float headStart, headEnd;
    public Sprite[] RoundSprite;

    // Start is called before the first frame update
    private void Awake()
    {
        length = headEnd - headStart;
        breakTimeImg = transform.Find("BreakTime").GetComponent<Image>();
        tutorialImg = transform.Find("Tutorial").GetChild(0).GetComponent<Image>();
        roundImg = transform.Find("CountDown").GetChild(3).GetComponent<Image>();

        goblinHeadTrans = transform.Find("GoblinProgress").GetChild(0).GetComponent<RectTransform>();

        animator = GetComponent<Animator>();
    }
    void Start()
    {
        animator.SetTrigger("Tutorial");
    }

    // Update is called once per frame
    void Update()
    {
        if (progressMove) {
            progressTime += Time.deltaTime*2.0f;

            goblinHeadTrans.anchoredPosition = Vector2.Lerp(oringinProgress, targetProgress, progressTime);
            if (progressTime >= 1.0f) {
                progressMove = false;
                progressTime = .0f;
            }
        }
    }

    public void SubCountDownCallBack(System.Action cbk) {
        countDownCBK = cbk;
    }

    public void NextTutorial() {
        tutorialProgres++;
        animator.SetInteger("TutorialProgress", tutorialProgres);
        if (tutorialProgres >= 4) tutorialImg.enabled = false;
    }

    public void FirstRound() {
        animator.SetTrigger("FirstRound");
    }

    public void StartRound(int round) {

        roundImg.sprite = RoundSprite[round];
        animator.SetTrigger("Round");
    }
    public void StartBreak() {
        animator.SetTrigger("BreakTime");
    }

    public void CountDownEnd() {
        countDownCBK();
    }

    public void GoblinProgress(float percent) {
        progressMove = true;
        oringinProgress = goblinHeadTrans.anchoredPosition;
        targetProgress = new Vector2(headStart + length * percent, 0);
    }

    public void CountDownCBK() {

    }
    public void GoBreakTime() {
        GoblinProgress(0);
        animator.Play("breakTimeIn");
    }
    public void RoundStartMusic() {
        AudioManager.SingletonInScene.PauseBGM();
        AudioManager.SingletonInScene.PlaySound2D("Round_Begin",1.0f);
    }
}
