using System;
using System.Collections;

using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class Score : MonoBehaviour

{

    public int totalScore;
    public int punchScore;
    public int blockScore;
    public int grabScore;
    public int throwScore;
    public int totalEnemy = 0;
    public float percentScore;
    public ScoreDebug LeftScoreText;
    public ScoreDebug RightScoreText;

    public ScoreDebug MainScoreText;
    public AnimationCurve scoreCurve;

    private int perfectStrike = 0;
    private float perfectLimit = 0.8f;                      //the lowest score to get a perfect hit
    private int strikeLookUpRange = 10;                     //how many hit we are looking at
    public int perfectHitLimit = 5;                        //lowest number of perfect need to have special effect 
    private Queue<bool> strikeQueue = new Queue<bool>();
    public bool isPerfectStrike = false;
    private int lastTime = 40;
    private float StrikingCoolDown;

    #region Singleton
    public static Score instance;

    private void Start()
    {
        // LeftScoreText = GameObject.Find("ScoreTextLeft").GetComponent<ScoreDebug>();
        // RightScoreText = GameObject.Find("ScoreTextRight").GetComponent<ScoreDebug>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of Score found!");
            return;
        }

        instance = this;
    }
    #endregion

    public void Add(HitType type, HitLocation loc, float normalizedScore, bool IsLeftHand = false)
    {
        int baseScore = 0;
        switch (type)
        {
            case HitType.Punch:
                baseScore = punchScore;
                break;
            case HitType.Grab:
                baseScore = grabScore;
                break;
            case HitType.Throw:
                baseScore = throwScore;
                break;
            case HitType.Block:
                baseScore = blockScore;
                break;
        }
        int singleScore = (int)(baseScore * normalizedScore);
        totalScore += singleScore;
        if(totalEnemy != 0)
        {
            percentScore = (float)totalScore / (totalEnemy * 100);
        }
        UpdateStrike(singleScore);

        // if (IsLeftHand)
        // {
        //     LeftScoreText.ShowHitScore(singleScore);
        // }
        // else
        // {
        //     RightScoreText.ShowHitScore(singleScore);
        // }
        
        
    }

    public int GetScoreStreak()
    {
        return perfectStrike;
    }

    public void ResetScoreStreak()
    {
        strikeQueue.Clear();
        perfectStrike = 0;
    }

    /*
     -- updating Striking table
     */
    private void UpdateStrike(float score)
    {
        // remove the first score in the queue
        while(strikeQueue.Count >= strikeLookUpRange)
        {
            bool removed = strikeQueue.Dequeue();
            if (removed == true)
                perfectStrike--;
        }

        //add a new score in queue
        if(score > perfectLimit)
        {
            strikeQueue.Enqueue(true);
            perfectStrike++;
        }
        else
        {
            strikeQueue.Enqueue(false);
        }
        
        //check performance
        isPerfectStrike = (perfectStrike >= perfectHitLimit && strikeQueue.Count == strikeLookUpRange) ? true : false;
        if (score > perfectLimit)
        {
            StrikingCoolDown = lastTime * Time.deltaTime;
        }
    }

    public void MoveScoreText(Vector3 PosToMove, Quaternion ScoreRot)
    {
        if (PosToMove != null) {
            //PosToMove.y = 0.4f;
            MainScoreText.transform.parent.position = PosToMove;
        }
        if (ScoreRot != null) MainScoreText.transform.parent.rotation = Quaternion.AngleAxis(180, Vector3.up) * ScoreRot;

        MainScoreText.ShowHitScore();
    }

    //public void SetScore(int ScoreToShow)
    //{
    //    scoretext.ShowHitScore(ScoreToShow);
    //}

    // return an normalized score based on timing, input should be in [0, 1]
    public float NormalizeScore(float timing)
    {
        return scoreCurve.Evaluate(timing);
    }

    // evaluate score and normalize based on different color sections
    public float GetColorFrac(float frac, float startFrac, float endFrac)
    {
        float start = scoreCurve.Evaluate(startFrac);
        float end = scoreCurve.Evaluate(endFrac);
        float score = scoreCurve.Evaluate(frac);
        if (start < end)
        {
            return (score - start) / (end - start);
        }
        else
        {
            return (start - score) / (start - end);
        }
    }


    public int GetScore()
    {
        return totalScore;
    }

    void Update()
    {
        if (isPerfectStrike && StrikingCoolDown > 0)
        {
            // OVRInput.SetControllerVibration(0.002f, 0.3f, OVRInput.Controller.LTouch);
            // OVRInput.SetControllerVibration(0.002f, 0.3f, OVRInput.Controller.RTouch);
        }

        StrikingCoolDown -= Time.deltaTime;
        if (StrikingCoolDown < 0)
            StrikingCoolDown = 0;
    }

}

