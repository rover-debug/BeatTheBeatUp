using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInForNar : GameInfor
{
    public float delay = 10.0f;
    public static bool fadeVolume = false;
    
    protected override void Awake()
    {
        gameStart = true;
        GetPlayerPosition();
        Invoke("StartOfMusicDelay", delay);
    }
    public void Skip()
    {
        fadeVolume = true;      
        StartCoroutine(GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOutLoadScene("Punching"));
    }

    protected override void Update()
    {

    }
}
