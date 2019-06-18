using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    bool shopBreakdown = false, bushBreakdown = false;
    int[] aniID = new int[2] { 0,0};
    float dt;
    float[] aniTime = new float[2] { .0f,.0f};
    SpriteRenderer shopSmogRender, bushSmogRender;
    GameObject shop, ruinShop, bush;

    public Sprite[] smogAnis;

    // Start is called before the first frame update
    private void Awake()
    {
        shop = GameObject.Find("Shop");
        ruinShop = transform.Find("ruin").gameObject;
        ruinShop.SetActive(false);
        bush = GameObject.Find("05tree060");
        shopSmogRender = transform.Find("ShopSmog").GetComponent<SpriteRenderer>();
        bushSmogRender = transform.Find("BushSmog").GetComponent<SpriteRenderer>();
        shopSmogRender.enabled = false;
        bushSmogRender.enabled = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;
        if (shopBreakdown) ShopSmogAni();
        if (bushBreakdown) BushSmogAni();

    }

    public System.Action GetShopCBK() {
        return BreakShop;
    }
    public void BreakShop() {
        shopSmogRender.enabled = true;
        shopBreakdown = true;
    }
    public System.Action GetBushCBK()
    {
        return BreakBush;
    }
    public void BreakBush() {
        bushSmogRender.enabled = true;
        bushBreakdown = true;
    }

    void ShopSmogAni() {
        aniTime[0] += dt;
        if (aniTime[0] > 0.06f) {
            if (aniID[0] >= 33) {
                shopBreakdown = false;
                return;
            }
            else if (aniID[0] > 16) {
                shop.SetActive(false);
                ruinShop.SetActive(true);
            }
            aniTime[0] = .0f;
            aniID[0]++;
            shopSmogRender.sprite = smogAnis[aniID[0]];
            
        }
    }

    void BushSmogAni() {
        aniTime[1] += dt;
        if (aniTime[1] > 0.06f)
        {
            if (aniID[1] >= 33)
            {
                bushBreakdown = false;
                return;
            }
            else if (aniID[1] > 16)
            {
                bush.SetActive(false);
            }
            aniTime[1] = .0f;
            aniID[1]++;
            bushSmogRender.sprite = smogAnis[aniID[1]];

        }
    }

}
