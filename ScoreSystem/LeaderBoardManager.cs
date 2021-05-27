using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;
using System;
using System.ComponentModel;

/// <summary>
/// 
/// </summary>
public class LeaderBoardManager
{
    private List<LeaderboardEntry> lbe;
    private LeaderBoardUI lbUI;
    private GameObject endUI;
    private GameObject UIhelper;
    private int limit = 10;

    /// check if oculus plateform  is initialized
    public void LeaderboardStart()
    {
        try
        {
            Core.AsyncInitialize();
            Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementsCallback);
        }
        catch (UnityException e)
        {
            //Debug.LogError("platform failed to initialize");
            //Debug.LogError(e);
            UnityEngine.Application.Quit();
        }

        lbUI = GameObject.Find("LeaderBoard").GetComponent<LeaderBoardUI>();
        lbUI.gameObject.SetActive(false);

        Users.GetLoggedInUser().OnComplete(m => {
            if (!m.IsError && m.Type == Message.MessageType.User_GetLoggedInUser)
            {
                //Debug.Log("Got Oculus username: " + m.GetUser().OculusID);
            }
        });

        endUI = GameObject.Find("UIEnd");
        endUI.SetActive(false);

        UIhelper = GameObject.Find("UIHelpers");
        UIhelper.SetActive(false);
    }
    void EntitlementsCallback(Message msg)
    {
        if (msg.IsError)
        {
            //Debug.LogError("you are not entitled to this application");
            //UnityEngine.Application.Quit();
        }
    }

    //submit the Score to the global leaderboard
    public void SubmitScore(string leaderboardName, int score)
    {
        if(score < 0)
        {
            //Debug.Log("Invalid value");
            return;
        }
        Leaderboards.WriteEntry(leaderboardName, score);
        //Debug.Log("data saved to leaderboards");
    }

    //Retrieve the Score from leaderboard
    public void GetLeaderboardData(string leaderboardName)
    {
        lbe = new List<LeaderboardEntry>();
        Leaderboards.GetEntries(leaderboardName,limit, LeaderboardFilterType.None,LeaderboardStartAt.Top).OnComplete(GetLeaderboardCallback);
        //Debug.Log("data retrieved from leaderboard");

        lbUI.gameObject.SetActive(true);
        lbUI.DisplayLeaderboard(lbe);

        endUI.SetActive(true);
        UIhelper.SetActive(true);
    }
    void GetLeaderboardCallback(Message<LeaderboardEntryList> msg)
    {
        if (!msg.IsError)
        {
            var entries = msg.Data;
            foreach (var entry in entries)
            {
                lbe.Add(entry);
            }
            //Debug.Log("Leaderboards data fetched succesfully");
        }
        else
        {
            //Debug.Log("Leaderboards data fetched failed");
        }

    }
}
