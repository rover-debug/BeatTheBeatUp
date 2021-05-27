using Oculus.Platform.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    private Transform scoreContainerTransform;
    private Transform scoreEntryTransform;
    private float height;
    private void Awake()
    {
        scoreContainerTransform = transform.Find("ScoreContainer");
        scoreEntryTransform = scoreContainerTransform.Find("ScoreEntry");

        scoreEntryTransform.gameObject.SetActive(false);
        gameObject.SetActive(true);

        height = scoreEntryTransform.GetComponent<RectTransform>().rect.height;
    }

    //Display leaderboardEntry  on UI, size <= limit
    public void DisplayLeaderboard(List<LeaderboardEntry> leaderboardEntry)
    {
        int count = 0;
        foreach (LeaderboardEntry entry in leaderboardEntry)
        {
            DisplayScoreEntry(entry, scoreContainerTransform, count);
            count++;
        }
        DisplayFakeScoore(scoreContainerTransform, count);
    }

    private void DisplayScoreEntry(LeaderboardEntry entry, Transform scoreContainer, int count)
    {
        Transform entryTransform = Instantiate(scoreEntryTransform, scoreContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

        entryRectTransform.anchoredPosition = new Vector2(0, -height * count);
        entryRectTransform.gameObject.SetActive(true);

        entryRectTransform.Find("Rank").GetComponent<Text>().text = entry.Rank.ToString();
        entryRectTransform.Find("Score").GetComponent<Text>().text = entry.Score.ToString();
        entryRectTransform.Find("Name").GetComponent<Text>().text = entry.User.OculusID.ToString();
    }

    /// <summary>
    /// deleting in the future
    /// </summary>
    private void DisplayFakeScoore(Transform scoreContainer, int count) {
        List<ScoreEntry> scoreList = new List<ScoreEntry>() {
            new ScoreEntry{ Score = 200 , Name = "Rick"},
            new ScoreEntry{ Score =  180, Name  = "Tom"},
            new ScoreEntry { Score = 100, Name = "Rock"},
        };
        foreach (ScoreEntry entry in scoreList)
        {

            Transform entryTransform = Instantiate(scoreEntryTransform, scoreContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

            entryRectTransform.anchoredPosition = new Vector2(0, -height * count);
            entryRectTransform.gameObject.SetActive(true);

            count++;
            entryRectTransform.Find("Rank").GetComponent<Text>().text = count.ToString();
            entryRectTransform.Find("Score").GetComponent<Text>().text = entry.Score.ToString();
            entryRectTransform.Find("Name").GetComponent<Text>().text = entry.Name.ToString();

        }
    }
    private class ScoreEntry
    {
        public int Score;
        public string Name;
    }
}
