using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slide
{
    public string name;

    [TextArea(2,25)]
    public string[] sentences;

    public GameObject image;

    public int slideDuration;

    public int secondBetweenSentences;
}

