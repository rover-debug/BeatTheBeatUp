using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPower : MonoBehaviour
{
    #region Singleton
    public static SuperPower instance;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of SuperPower found!");
            return;
        }

        instance = this;
    }
    #endregion

    public TimeManager timeManager;
    void Update()
    {
        if (Input.GetKeyDown("1"))
            SlowDown();
    }
    public void SlowDown()
    {
        // TODO: change to slow motion without resetting
        timeManager.SlowMotion();
    }
    public void EndSlowDown()
    {
        // TODO: reset in this function instead, set timescale back to normal
    }
}
