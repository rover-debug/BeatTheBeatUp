using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class Dot1 : MonoBehaviour

{

    private float activeTime;

    // Start is called before the first frame update

    void Start()

    {

        

    }



    // Update is called once per frame

    void Update()

    {

        if (activeTime > 0)
        {
            activeTime -= Time.deltaTime;

            if (activeTime <= 0)
            {
                gameObject.SetActive(false);
                activeTime = 0;
            }
        }

    }
//Debug.Log


    public void ActivateDot(float spanSec, string attack)
    {
        switch (attack)
        {
            case "Slap":
                gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case "Punch":
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                break;
            case "Grab":
                gameObject.GetComponent<Renderer>().material.color = Color.green;
                break;
        }

        gameObject.SetActive(true);
        //Debug.Log("Setting ActiveTime to " + spanSec);
        activeTime = spanSec;
    }

}