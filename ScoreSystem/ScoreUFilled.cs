using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUFilled : MonoBehaviour
{
    private float currentFill;
    private float startFill;
    private float endFill;
    private float time = 0f;
    private float addingTime = 0.01f;
    private void Awake()
    {
        startFill = 0.0f;
        endFill = Score.instance.percentScore;
        this.GetComponentInChildren<Text>().text = Score.instance.totalScore.ToString();
    }
    private void Update()
    {
        currentFill =  Mathf.Lerp(0.0f, endFill, time);
        time += addingTime;
        this.GetComponent<Image>().fillAmount = currentFill;
    }
}
