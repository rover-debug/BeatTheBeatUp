using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class RankSystem : MonoBehaviour
{
    [Range(0f, 1f)] public float SPlusBaseline = 0.9f;
    [Range(0f, 1f)] public float SBaseline = 0.85f;
    [Range(0f, 1f)] public float APlusBaseline = 0.75f;
    [Range(0f, 1f)] public float ABaseline = 0.7f;
    [Range(0f, 1f)] public float BPlusBaseline = 0.6f;
    [Range(0f, 1f)] public float BBaseline = 0.5f;
    [Range(0f, 1f)] public float CPlusBaseline = 0.4f;
    [Range(0f, 1f)] public float CBaseline = 0.3f;

    private float score;
    private int ranking;
    private string[] rankText = new string[9]{ "S+","S","A+","A","B+","B","C+","C","F" };
    public AudioClip[] RankSounds;
    public AudioClip[] RankCallOuts;
    public AudioSource RankSoundSource;
    private Transform ScoreText;

    private void OnValidate()
    {
        SBaseline = Mathf.Min(SBaseline, SPlusBaseline);
        APlusBaseline = Mathf.Min(APlusBaseline, SBaseline);
        ABaseline = Mathf.Min(ABaseline, APlusBaseline);
        BPlusBaseline = Mathf.Min(BPlusBaseline, ABaseline);
        BBaseline = Mathf.Min(BBaseline, BPlusBaseline);
        CPlusBaseline = Mathf.Min(CPlusBaseline, BBaseline);
        CBaseline = Mathf.Min(CBaseline, CPlusBaseline);
    }

    private void Awake()
    {
        score = Score.instance.percentScore;
        GetRanking();
        DisplayRanking();
    }

    private void GetRanking()
    {
        if(score >= SPlusBaseline)
        {
            ranking = 0;
        }
        else if (score >= SBaseline)
        {
            ranking = 1;
        }
        else if (score >= APlusBaseline)
        {
            ranking = 2;
        }
        else if (score >= ABaseline)
        {
            ranking = 3;
        }
        else if (score >= BPlusBaseline)
        {
            ranking = 4;
        }
        else if (score >= BBaseline)
        {
            ranking = 5;
        }
        else if (score >= CPlusBaseline)
        {
            ranking = 6;
        }
        else if (score >= CBaseline)
        {
            ranking = 7;
        }
        else
        {
            ranking = 8;
        }
    }


    private void DisplayRanking()
    {
        ScoreText = transform.Find("Text");
        if(ScoreText != null)
        {
            ScoreText.GetComponent<Text>().text = rankText[ranking];
            RankSoundSource.PlayOneShot(RankSounds[ranking]);
            StartCoroutine(waitForRankingClipSFX(RankSounds[ranking]));

        }
    }

    IEnumerator waitForRankingClipSFX(AudioClip sfxClip)
    {
        yield return new WaitForSeconds(sfxClip.length);
        RankSoundSource.PlayOneShot(RankCallOuts[ranking]);
    }
}
