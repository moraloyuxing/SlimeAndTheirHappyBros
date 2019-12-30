using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour {

    public int CurrentStep = -1;
    public GameObject CheckHint;
    public GameObject RescueSlimeGroup;
    public float ReadingTime = 3.0f;
    float Trigger_Moment;
    int OlderStep = -1;
    GameManager _gamemanager;
    GoblinManager _goblinmanager;

    //達標後短暫休息
    bool AchieveFlag = false;
    public float RestTime = 3.0f;
    float Achieve_Moment;

    //各教學階段條件變數
    public bool[] DyeFlag = new bool[4];    //教學階段1_染色
    public bool[] WashFlag = new bool[4];    //教學階段2_洗澡
    public int BaseGoblinCount;    //教學階段3_攻擊相同顏色
    public int MergeGoblinCount;    //教學階段4_混色與攻擊
    public int RescueCount;    //教學階段5_救人

    //各種提示
    public GameObject[] T_Words = new GameObject[6];
    public GameObject[] HintElement = new GameObject[7];//0~2：顏料池提示；3：澡盆提示；5~7：基礎色哥布林提示；8~10：進階色哥布林提示
    public Vector3[] TutorialSpawnPos = new Vector3[5];//0~2：基礎色出生處；3~4進階色出生處

    //教學詢問按鈕
    public Image[] TutorialAskBtn = new Image[2];
    bool During_Ask = false;

    //確認環
    public Image PressTorus;
    public float PressTime = 1.5f;

    void Awake() {
        _gamemanager = GetComponent<GameManager>();
        _goblinmanager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();
        //未參與的玩家編號直接設true，避免flag無法達成
        for (int i = 0; i < 4; i++) {
            if (G_PlayerSetting.JoinPlayer[i] == false) {
                DyeFlag[i] = true;
                WashFlag[i] = true;
            }
        }
    }

    void Update() {

        if (During_Ask == true && Time.time > Trigger_Moment + ReadingTime) AskTimeFunc(true);

        if (OlderStep != CurrentStep) {
            if (Time.time > Trigger_Moment + ReadingTime) {
                CheckHint.SetActive(true);
                _gamemanager.CanNextTut = true;
                OlderStep = CurrentStep;
            }
        }

        if (AchieveFlag == true) {
            if (Time.time > Achieve_Moment + RestTime) {
                for (int T = 0; T < 6; T++) T_Words[T].SetActive(false);
                for (int E = 0; E < 4; E++) HintElement[E].SetActive(false);
                _gamemanager.NextTutorial();
                AchieveFlag = false;
            }
        }

    }

    //啟動詢問是否需要新手教學的計時(3sec)
    public void AskTimeFunc(bool state) {

        if (state == true){
            for (int i = 0; i < 2; i++) TutorialAskBtn[i].enabled = true;
            During_Ask = false;
            _gamemanager.CanDecideAsk = true;
        }

        else {
            During_Ask = true;
            Trigger_Moment = Time.time;
        }

    }

    //開啟下一階段教學階段之示範，準備計時3秒跳出確認鍵
    public void SetTutorialStep(int _step) {
        if (_step < 6) {
            CurrentStep = _step;
            PressTorus.fillAmount = 0.0f;
            CheckHint.SetActive(false);
            Trigger_Moment = Time.time;
        }
    }

    //持續按住A確認時，確認環的fillAmount
    public void FillPressTorus(bool pressed) {
        if (pressed == true){
            PressTorus.fillAmount += 1.0f / PressTime * Time.deltaTime;
            if(PressTorus.fillAmount >=1.0f)_gamemanager.TorusCheck();
        }
        else {
            PressTorus.fillAmount = 0.0f;
        }
    }


    //關閉此教學階段之示範，進入演練並開啟對應UI
    public void ShowTutorialUI(){
        switch (CurrentStep) {
            case 1:
                T_Words[0].SetActive(true);
                for (int i = 0; i < 3; i++) HintElement[i].SetActive(true);
                break;

            case 2:
                T_Words[1].SetActive(true);
                HintElement[3].SetActive(true);
                break;

            case 3:
                BaseGoblinCount = 3;
                for (int g = 0; g < 3; g++) _goblinmanager.SpawnNormalGoblin_BaseforTutorial(g, TutorialSpawnPos[g]);
                T_Words[2].SetActive(true);
                break;

            case 4:
                MergeGoblinCount = 2;
                for (int h = 3; h < 5; h++) _goblinmanager.SpawnNormalGoblin_MultiforTutorial(0, TutorialSpawnPos[h]);
                T_Words[3].SetActive(true);
                break;
            case 5:
                RescueCount = 3;
                T_Words[5].SetActive(true);
                RescueSlimeGroup.SetActive(true);
                break;
        }
    }

    //確認此教學階段實際操作的條件是否達成
    public void CheckStepProgress() {
        switch (CurrentStep) {
            case 1:
                AchieveFlag = true;
                for (int i=0; i < 4; i++) {if (DyeFlag[i] == false) AchieveFlag = false;}
                if (AchieveFlag == true) Achieve_Moment = Time.time;
                break;

            case 2:
                AchieveFlag = true;
                for (int i = 0; i < 4; i++) { if (WashFlag[i] == false) AchieveFlag = false; }
                if (AchieveFlag == true) Achieve_Moment = Time.time;
                break;

            case 3:
                BaseGoblinCount--;
                if (BaseGoblinCount <= 0) {
                    AchieveFlag = true;
                    Achieve_Moment = Time.time;
                }
                break;

            case 4:
                MergeGoblinCount--;
                if (MergeGoblinCount == 1) {
                    T_Words[3].SetActive(false);
                    T_Words[4].SetActive(true);
                }
                else if (MergeGoblinCount <= 0){
                    AchieveFlag = true;
                    Achieve_Moment = Time.time;
                }

                break;

            case 5:
                RescueCount--;
                if (RescueCount <= 0) {
                    AchieveFlag = true;
                    Achieve_Moment = Time.time;
                    RescueSlimeGroup.SetActive(false);
                }
                break;
        }
    }


}
