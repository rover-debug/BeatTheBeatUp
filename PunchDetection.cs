using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchDetection : MonoBehaviour
{

    public OVRInput.Controller TouchController = OVRInput.Controller.RTouch;

    Vector3 PrevVelocity = Vector3.zero;

    public int AlignmentScore = 0;

    public float AlignmentThreshold = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        Vector3 CurrVelocity = OVRInput.GetLocalControllerVelocity(TouchController);
        ////Debug.Log(LVelocity);
        // if (CurrVelocity.magnitude > 0.01f)
        //     //Debug.Log(CurrVelocity);

        if (Vector3.Dot(CurrVelocity, PrevVelocity) > AlignmentThreshold)
        {
            AlignmentScore++;
        }
        else
        {
            AlignmentScore = 0;
        }


        PrevVelocity = CurrVelocity;
    }
}
