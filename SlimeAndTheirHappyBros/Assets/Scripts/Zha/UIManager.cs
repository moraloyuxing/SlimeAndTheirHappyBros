using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    bool progressMove = false;
    int tutorialProgres = 0;
    int curRound = 1;
    float length, progressTime = .0f, totalTime = .0f;

    Vector2 oringinProgress, targetProgress;

    RectTransform goblinHeadTrans;

    Image tutorialImg; 
    Image goblinProgress, goblinHead;
    Image breakTimeImg, roundImg;
    Text roundTxt;

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
        roundTxt = transform.Find("GoblinProgress").GetChild(1).GetComponent<Text>();

        animator = GetComponent<Animator>();
    }
    void Start()
    {
        animator.SetTrigger("Tutorial");
        oringinProgress = new Vector2(headStart, 0);
        targetProgress = new Vector2(headEnd,0);
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
        countDownCBK = cbk;
    }

    public void NextTutorial() {
        tutorialProgres++;
        animator.SetInteger("TutorialProgress", tutorialProgres);
        AudioManager.SingletonInScene.PlaySound2D("Teach_Enter",0.7f);
        if (tutorialProgres >= 4) tutorialImg.enabled = false;
    }

    public void FirstRound() {
        animator.SetTrigger("FirstRound");
    }

    public void StartRound(int round) {
        curRound++;
        roundTxt.text = curRound.ToString();
        roundImg.sprite = RoundSprite[round];
        goblinHeadTrans.anchoredPosition = oringinProgress;
        animator.SetTrigger("Round");

    }
    public void StartBreak() {
        animator.SetTrigger("BreakTime");
    }

    public void CountDownEnd() {
        countDownCBK();
        progressMove = true;
    }

    public void GoblinProgress(float percent) {
        progressMove = true;
        oringinProgress = goblinHeadTrans.anchoredPosition;
        targetProgress = new Vector2(headStart + length * percent, 0);
    }


    public void CountDownCBK() {

    }
    public void GoBreakTime() {
        //GoblinProgress(0);
        animator.Play("breakTimeIn");
    }

    public void GoLose() {
        animator.SetTrigger("Lose");
    }

    public void RoundStartMusic() {
        AudioManager.SingletonInScene.PauseBGM();
        AudioManager.SingletonInScene.PlaySound2D("Round_Begin",0.3f);
    }
}
