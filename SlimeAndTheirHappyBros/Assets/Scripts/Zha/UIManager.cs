using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    int tutorialProgres = 0;
    float length;


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

        animator = GetComponent<Animator>();
    }
    void Start()
    {
        animator.SetTrigger("Tutorial");
    }

    // Update is called once per frame
    void Update()
    {
        
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
        goblinHeadTrans.anchoredPosition = new Vector2(headStart + length * percent, 0);
    }

    public void CountDownCBK() {

    }
    public void GoBreakTime() {
        animator.Play("breakTimeIn");
    }
}
