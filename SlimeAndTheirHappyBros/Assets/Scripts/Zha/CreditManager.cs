using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    bool transEffect = false;
    float transTime = .0f;
    Animator animator;

    //public CameraTrasnsEffect cameraTransEffect;

    Rewired.Player[] playerInput = new Rewired.Player[4];

    // Start is called before the first frame update
    void Start()
    {
        playerInput[0] = Rewired.ReInput.players.GetPlayer(0);
        playerInput[1] = Rewired.ReInput.players.GetPlayer(1);
        playerInput[2] = Rewired.ReInput.players.GetPlayer(2);
        playerInput[3] = Rewired.ReInput.players.GetPlayer(3);

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!transEffect && (playerInput[0].GetButtonDown("Skip") || playerInput[1].GetButtonDown("Skip")
           || playerInput[2].GetButtonDown("Skip") || playerInput[3].GetButtonDown("Skip") || Input.GetKeyDown(KeyCode.Space)))
        {
            transEffect = true;
            animator.SetTrigger("BlackOut");
        }
        
    }

    public void TransEffectAniEvent() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
