using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JoinGame : MonoBehaviour{
    public Camera m_camera;
    public Transform[] Four_Player = new Transform[4];
    Animator[] animator = new Animator[4];
    bool[] a_button = new bool[4];
    string[] Which_Player = new string[4];
    bool[] Already_Player = new bool[4];
    int Ready_Count = 0;
    float Ready_Moment;

    public Image BlackPanel;
    float alpha = 1.0f;

    bool tranOce = false;
    public CameraTrasnsEffect cameraTransEffect;
    public Animator titleAni;

    //彈性玩家人數設定--Start
    public CheckPlayer _checkplayer;
    public JoinCountDown _CountDownUI;
    bool CanJoin = true;

    void Start(){
        for (int i = 0; i < 4; i++) {
            Which_Player[i] = Four_Player[i].name;
            animator[i] = Four_Player[i].gameObject.GetComponent<Animator>();
        }
    }


    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        for (int i = 0; i < 4; i++) {
            Four_Player[i].transform.LookAt(Four_Player[i].position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
            a_button[i] = Input.GetButtonDown(Which_Player[i] + "MultiFunction") || Input.GetKeyDown(KeyCode.W);//|| Input.GetKeyDown(KeyCode.J)

            //測試
            a_button[0] = Input.GetButtonDown(Which_Player[0] + "MultiFunction") || Input.GetKeyDown(KeyCode.U);
            a_button[1] = Input.GetButtonDown(Which_Player[1] + "MultiFunction") || Input.GetKeyDown(KeyCode.I);
            a_button[2] = Input.GetButtonDown(Which_Player[2] + "MultiFunction") || Input.GetKeyDown(KeyCode.O);
            a_button[3] = Input.GetButtonDown(Which_Player[3] + "MultiFunction") || Input.GetKeyDown(KeyCode.P);

            if (a_button[i] && Already_Player[i] == false &&CanJoin == true) {
                Already_Player[i] = true;
                animator[i].SetBool("join", true);
                AudioManager.SingletonInScene.PlaySound2D("Revive", 1f);
                Ready_Count++;
                Ready_Moment = Time.time;
                _checkplayer.PlayerCount++;//總人數+1，新增by辰0803
                if (Ready_Count == 2) _CountDownUI.Start_CountDown();
                if (Ready_Count == 4) _CountDownUI.ForceCancelCD();
            }
        }

        if (Ready_Count == 4 && Time.time > Ready_Moment + 1.0f) {
            if (!tranOce) {
                cameraTransEffect.GoTransOut();
                tranOce = true;
                titleAni.SetTrigger("disappear");
            }
            if (Time.time > Ready_Moment + 3.0f) {
                SceneManager.LoadScene(1);
                DontDestroyOnLoad(_checkplayer);//保留給遊戲場景做人數設定後刪除，新增by辰0803
            }

            //BlackPanel.color = new Color(0.0f, 0.0f, 0.0f, BlackPanel.color.a + alpha);
            //if (BlackPanel.color.a >= 1.0f) SceneManager.LoadScene(1);
        }

    }

    public void Fake_Ready() {
        Ready_Count = 4;//假性4人都ready，真正人數取決於_checkplayer.PlayerCount
        Ready_Moment = Time.time;
        CanJoin = false;
    }

}
