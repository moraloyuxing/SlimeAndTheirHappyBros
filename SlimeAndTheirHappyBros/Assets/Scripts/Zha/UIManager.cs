using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    bool progressMove = false, goBoss = false;
    int tutorialProgres = 0;
    int curRound = 1;
    float length, progressTime = .0f, totalTime = .0f;

    Vector2 oringinProgress, targetProgress;

    Transform goblinProgressParent;
    RectTransform goblinHeadTrans, bossHp;

    Image tutorialImg; 
    Image goblinProgress, goblinHead;
    Image breakTimeImg, roundImg;
    Text roundTxt;

    Animator animator;

    System.Action CountDownCBK, BossCountDownCBK;

    public float headStart, headEnd;
    public Sprite[] RoundSprite;
    public Sprite bossRoundSpirte;
    TutorialStep _tutorialstep;

    // Start is called before the first frame update
    private void Awake()
    {
        length = headEnd - headStart;
        breakTimeImg = transform.Find("BreakTime").GetComponent<Image>();
        tutorialImg = transform.Find("Tutorial").GetChild(0).GetComponent<Image>();
        roundImg = transform.Find("CountDown").GetChild(3).GetComponent<Image>();

        goblinProgressParent = transform.Find("GoblinProgress");
        goblinHeadTrans = goblinProgressParent.GetChild(0).GetComponent<RectTransform>();
        roundTxt = goblinProgressParent.GetChild(1).GetComponent<Text>();
        bossHp = transform.Find("KingBlood").GetChild(0).GetComponent<RectTransform>();

        animator = GetComponent<Animator>();
    }
    void Start()
    {
        //animator.SetTrigger("Tutorial");
        oringinProgress = new Vector2(headStart, 0);
        targetProgress = new Vector2(headEnd,0);
        goblinProgressParent.gameObject.SetActive(false);
        _tutorialstep = GameObject.Find("GameManager").GetComponent<TutorialStep>();
    }

    // Update is called once per frame
    void Update()
    {
        if (progressMove) {
            if (progressTime <= totalTime)
            {
                progressTime += Time.deltaTime;
                goblinHeadTrans.anchoredPosition = Vector2.Lerp(oringinProgress, targetProgress, progressTime / totalTime);
            }
            else progressMove = false;
        }
    }

    public void SetTotalTime(float time) {
        totalTime = time;
    }

    public void SubCountDownCallBack(System.Action cbk) {
        CountDownCBK = cbk;
    }
    public void SubBossCountDownCallBack(System.Action cbk)
    {
        BossCountDownCBK = cbk;
    }

    public void AskTutorial() {
        animator.SetTrigger("Ask");
        _tutorialstep.AskTimeFunc(false);
    }

    public void TutorialAskResult(int r) {
        animator.SetInteger("AskResult", r);
    }

    public void StartTutorial() {
        animator.SetTrigger("Tutorial");
    }

    public void NextTutorial() {
        tutorialProgres++;
        if (tutorialProgres < 6)AudioManager.SingletonInScene.PlaySound2D("Teach_Enter", 0.7f);
        if (tutorialProgres >= 6) {
            tutorialImg.enabled = false;
            FirstRound();
        }
        animator.SetInteger("TutorialProgress", tutorialProgres);
    }

    public void FirstRound() {
        animator.SetTrigger("FirstRound");
        animator.Play("PlayerState_In",1);
        Debug.Log("line103");
    }

    public void StartRound(int round) {
        curRound++;
        roundTxt.text = curRound.ToString();
        roundImg.sprite = RoundSprite[round];
        goblinHeadTrans.anchoredPosition = oringinProgress;
        animator.SetTrigger("Round");

    }

    //應無作用
    public void StartBreak() {
        animator.SetTrigger("BreakTime");
    }

    public void StartBossRound() {
        roundImg.sprite = bossRoundSpirte;
        animator.SetTrigger("BossLevel");
        goBoss = true;
    }

    public void CountDownEnd() {
        if (!goBoss)
        {
            CountDownCBK();
            progressMove = true;
            progressTime = .0f;
        }
        else {
            BossCountDownCBK();
        }
    }

    public void GoblinProgress(float percent) {
        progressMove = true;
        oringinProgress = goblinHeadTrans.anchoredPosition;
        targetProgress = new Vector2(headStart + length * percent, 0);
    }

    public void GoBreakTime() {
        //GoblinProgress(0);
        animator.Play("breakTimeIn");
    }

    public void DecreaseBossHp(float percent) {
        bossHp.sizeDelta = new Vector2(117f, 620.0f*percent);
    }

    public void GoWin() {
        animator.SetTrigger("Win");
    }

    public void GoLose() {
        animator.SetTrigger("Lose");
    }

    public void RoundStartMusic() {
        AudioManager.SingletonInScene.PauseBGM();
        AudioManager.SingletonInScene.PlaySound2D("Round_Begin",0.3f);
    }
}
