using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    bool shopBreakdown = false;
    int aniID = 0;
    float dt, aniTime;
    SpriteRenderer smogRender;
    GameObject shop, ruinShop;

    public Sprite[] smogAnis;

    // Start is called before the first frame update
    private void Awake()
    {
        shop = GameObject.Find("Shop");
        ruinShop = GameObject.Find("ruin");
        smogRender = transform.Find("Smog").GetComponent<SpriteRenderer>(); ;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;
        if (shopBreakdown) SmogAni();
    }

    public void BreakShop() {
        shopBreakdown = true;
    }

    void SmogAni() {
        aniTime += dt;
        if (aniTime > 0.06f) {
            if (aniID >= 33) {
                shopBreakdown = false;
                return;
            }
            else if (aniID > 16) {
                shop.SetActive(false);
                ruinShop.SetActive(true);
            }
            aniTime = .0f;
            aniID++;
            smogRender.sprite = smogAnis[aniID];
            
        }
    }



}
