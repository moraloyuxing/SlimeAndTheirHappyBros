using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Image[] tutorialImg; 
    Image goblinProgress, goblinHead;
    Image breakTimeImg;

    Animator animator;

    // Start is called before the first frame update
    private void Awake()
    {
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

    public void CountDownCBK() {

    }
    public void GoBreakTime() {
        animator.Play("breakTimeIn");
    }
}
