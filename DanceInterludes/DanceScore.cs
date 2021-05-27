using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceScore : MonoBehaviour
{
    #region Singleton
    public static DanceScore instance;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of DanceScore found!");
            return;
        }

        instance = this;
    }
    #endregion

    Text text;
    int score;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }


    public void AddScore(int add = 1)
    {
        score += add;
        text.text = "Dance Interlude: " + score.ToString();
    }

    public void DisplaySpecial(float s)
    {
        text.text = "Dance Interlude: " + s.ToString();
    }
}
