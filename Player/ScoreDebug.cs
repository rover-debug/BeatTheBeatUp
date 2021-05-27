using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDebug : MonoBehaviour
{

    public GameObject PerfectHit;
    public GameObject GoodHit;
    public GameObject BadHit;
    public GameObject LateHit;
    public GameObject WrongSpot;

    private GameObject CurrText;
    public float displaytime = 0.5f;
    float time;
    public bool showTotalScore;

    private string ScoreTextToShow;

    // Start is called before the first frame update
    void Start()
    {
        // text = GetComponent<Text>();
        // text.enabled = false;
            
    }

    // Update is called once per frame
    void Update()
    {

        //  if (showTotalScore)
        //  {
        //      //if (Curr)
        //      ShowHitScore(Score.instance.totalScore); 
        //  }
        //  else
        {
            if (time >= 0)
            {
                time -= Time.deltaTime;
                if (time <= 0 && CurrText)
                {
                    time = 0;
                    CurrText.GetComponent<MeshRenderer>().enabled = false; // SetActive(false);
                    // CurrText.GetComponent<scoreAnimation>().TriggerCondition(false);
                }
            }
        }
    }
       
    //}
    public void SetText(string ScoreText)
    {
        ScoreTextToShow = ScoreText;
    }

    public void SetStreak(int ScoreStreak)
    {
        if (ScoreStreak > Score.instance.perfectHitLimit) ScoreTextToShow += " x2";
    }

    public void ShowHitScore()
    {
        if (CurrText)
            CurrText.GetComponent<MeshRenderer>().enabled = false;
            
        if (ScoreTextToShow.Contains("Perfect"))
        {
            PerfectHit.GetComponent<MeshRenderer>().enabled = true; // SetActive(true);
            PerfectHit.GetComponent<scoreAnimation>().TriggerCondition();
            CurrText = PerfectHit;
        }
        else if (ScoreTextToShow.Contains("Good"))
        {
            GoodHit.GetComponent<MeshRenderer>().enabled = true; // SetActive(true);
            GoodHit.GetComponent<scoreAnimation>().TriggerCondition();
            CurrText = GoodHit;
        }
        else if (ScoreTextToShow.Contains("Too late"))
        {
            LateHit.GetComponent<MeshRenderer>().enabled = true; // SetActive(true);
            CurrText = LateHit;
        }
        else if (ScoreTextToShow.Contains("Wrong part"))
        {
            WrongSpot.GetComponent<MeshRenderer>().enabled = true; // SetActive(true);
            CurrText = WrongSpot;
        }
        else
        {
            BadHit.GetComponent<MeshRenderer>().enabled = true; // SetActive(true);
            CurrText = BadHit;
        }
        
        time = displaytime;
    }
    public void ShowHitScore(int score)
    {
       this.GetComponent<Text>().text = score.ToString();
    }
}
