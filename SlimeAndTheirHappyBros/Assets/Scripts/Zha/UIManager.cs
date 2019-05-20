using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    float length;


    RectTransform goblinHeadTrans;

    Image[] tutorialImg; 
    Image goblinProgress, goblinHead;
    Image breakTimeImg;

    Animator animator;

    public float headStart, headEnd;

    // Start is called before the first frame update
    private void Awake()
    {
        length = headEnd - headStart;
        breakTimeImg = transform.Find("BreakTime").GetComponent<Image>();

        animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
