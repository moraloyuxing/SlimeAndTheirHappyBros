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
    public GameObject Title;
    float alpha = 0.02f;

    bool tranOce = false;
    public CameraTrasnsEffect cameraTransEffect;

    void Start(){
        for (int i = 0; i < 4; i++) {
            Which_Player[i] = Four_Player[i].name;
            animator[i] = Four_Player[i].gameObject.GetComponent<Animator>();
        }
    }


    void Update(){

        for (int i = 0; i < 4; i++) {
            Four_Player[i].transform.LookAt(Four_Player[i].position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
            a_button[i] = Input.GetButtonDown(Which_Player[i] + "MultiFunction") || Input.GetKeyDown(KeyCode.J);

            if (a_button[i] && Already_Player[i] == false) {
                Already_Player[i] = true;
                animator[i].SetBool("join", true);
                AudioManager.SingletonInScene.PlaySound2D("Revive", 1f);
                Ready_Count++;
                Ready_Moment = Time.time;
            }
        }

        if (Ready_Count == 4 && Time.time > Ready_Moment + 1.0f) {
            if (!tranOce) {
                Title.SetActive(false);
                cameraTransEffect.GoTransOut();
                tranOce = true;
            }

            if (Time.time > Ready_Moment + 3.0f) SceneManager.LoadScene(1);
            //BlackPanel.color = new Color(0.0f, 0.0f, 0.0f, BlackPanel.color.a + alpha);
            //if (BlackPanel.color.a >= 1.0f) SceneManager.LoadScene(1);
        }

    }

}
