using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using UnityScript.Scripting.Pipeline;

public class LeaderBoard : MonoBehaviour
{
    private Transform scoreContainerTransform;
    private Transform scoreEntryTransform;
    private List<ScoreEntry> scoreList;
    private List<Transform> scoreTransformList;
    private string fileName = "Leaderboard_temp";
    private GameObject endUI;
    private GameObject UIhelper;
    private void Awake()
    {
        scoreContainerTransform = transform.Find("ScoreContainer");
        scoreEntryTransform = scoreContainerTransform.Find("ScoreEntry");

        scoreEntryTransform.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString(fileName);
        if (String.IsNullOrEmpty(jsonString))
        {
            //Debug.Log("can't find json file, create a new one");
            scoreList = new List<ScoreEntry>() { };
            ScoreList scoreListJs = new ScoreList { ScoreEntryList = scoreList };
            string json = JsonUtility.ToJson(scoreListJs);
            PlayerPrefs.SetString(fileName, json);
            PlayerPrefs.Save();
            //if you want to find this file
            //click start, type "regedit"  
            //Registry  Editor ==> HKEY_CURRENT_USER ==> Software ==> corresponding prject file 
            //or search the fie with same name

            AddScoreEntry(900, "Rick");
            AddScoreEntry(500, "Tom"); 
            AddScoreEntry(100, "Rock");
            AddScoreEntry(30, "May");

            /*scoreList = new List<ScoreEntry>() {
                new ScoreEntry{ Score = 900 , Name = "Rick"},
                new ScoreEntry{ Score =  500, Name  = "Tom"},
                new ScoreEntry { Score = 100, Name = "Rock"},
            };*/
        }

        //disable UI at begining
        this.gameObject.SetActive(false);
        
        endUI = GameObject.Find("UIEnd");
        endUI.SetActive(false);

        UIhelper = GameObject.Find("UIHelpers");
        UIhelper.SetActive(false);
    }

    public void LoadScore()
    {
        //active leaderboard UI
        this.gameObject.SetActive(true);
        endUI.SetActive(true);
        UIhelper.SetActive(true);
        foreach (Transform child in UIhelper.transform)
        {
            child.gameObject.SetActive(true);
        }

        //load information from json file
        string jsonString = PlayerPrefs.GetString(fileName);
        if (String.IsNullOrEmpty(jsonString))
        {
            //Debug.LogError("can't find json file");
        }
        else
        {
            ScoreList scoreEntryList = JsonUtility.FromJson<ScoreList>(jsonString);
            scoreList = scoreEntryList.ScoreEntryList;

            scoreTransformList = new List<Transform>();
            foreach (ScoreEntry score in scoreList)
            {
                DisplayScoreEntry(score, scoreContainerTransform, scoreTransformList);
            }
        }

        ////Debug.Log(PlayerPrefs.GetString("Leaderboard_temp"));
    }

    private void DisplayScoreEntry(ScoreEntry scoreEntry, Transform scoreContainer, List<Transform>transformList )
    {
        float height = scoreEntryTransform.GetComponent<RectTransform>().rect.height;
        Transform entryTransform = Instantiate(scoreEntryTransform, scoreContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

        entryRectTransform.anchoredPosition = new Vector2(0, -height * transformList.Count);
        entryRectTransform.gameObject.SetActive(true);
        
        entryRectTransform.Find("Rank").GetComponent<Text>().text = (transformList.Count+1).ToString();
        entryRectTransform.Find("Score").GetComponent<Text>().text = scoreEntry.Score.ToString();
        entryRectTransform.Find("Name").GetComponent<Text>().text = scoreEntry.Name;

        transformList.Add(entryTransform);
    }

    public void AddScoreEntry(int score, string name)
    {
        // get new entry for name and score
        ScoreEntry scoreEntry = new ScoreEntry { Score = score, Name = name };

        //add new entry into list
        string jsonString = PlayerPrefs.GetString(fileName);
        ScoreList scoreEntryList = JsonUtility.FromJson<ScoreList>(jsonString);

        //find duplicated name
        bool newEntry = true;
        bool duplicateName = false;
        int j = 0;
        foreach(ScoreEntry entry in scoreEntryList.ScoreEntryList)
        {
            if (string.Compare(name,entry.Name) == 0)
            {
                duplicateName = true;
                break;
            }
            j++;
        }

        //dealing  the  dupliicated name
        if(duplicateName)
        {
            if (score <= scoreEntryList.ScoreEntryList[j].Score)
            {
                newEntry = false;
            }
            else
            {
                scoreEntryList.ScoreEntryList.RemoveAt(j);
            }
        }
        
        //insert in order
        List<ScoreEntry> newList = new List<ScoreEntry>();
        bool firstTime = true;
        if (newEntry)
        {
            for(int i = 0; i< scoreEntryList.ScoreEntryList.Count; i++)
            {
                if (score > scoreEntryList.ScoreEntryList[i].Score && firstTime)
                {
                    newList.Add(scoreEntry);
                    firstTime = false;
                }
                newList.Add(scoreEntryList.ScoreEntryList[i]);
            }
            if (firstTime)
            {
                newList.Add(scoreEntry);
            }
            scoreEntryList.ScoreEntryList = newList;
        }

        //load new list into json file
        string json = JsonUtility.ToJson(scoreEntryList);
        PlayerPrefs.SetString(fileName, json);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class ScoreEntry
    {
        public int Score;
        public string Name;
    }

    private class ScoreList
    {
        public List<ScoreEntry> ScoreEntryList;
    }

}
