using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private OVRInput.Controller LTouch_Controller = OVRInput.Controller.LTouch;
    private OVRInput.Controller RTouch_Controller = OVRInput.Controller.RTouch;

    private float ControllerGap = 0.2f;
    private float ControllerDegreeGap = 60.0f;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        Vector3 LPos = OVRInput.GetLocalControllerPosition(LTouch_Controller);
        Vector3 RPos = OVRInput.GetLocalControllerPosition(RTouch_Controller);
        
        Quaternion lRot = OVRInput.GetLocalControllerRotation(LTouch_Controller);
        Quaternion rRot = OVRInput.GetLocalControllerRotation(RTouch_Controller);
        
        Vector3 GamePos = transform.TransformPoint(LPos);

        // if (OVRInput.Get(OVRInput.Button.Two))
        // {
        //     //Debug.Log(Mathf.Abs(Vector3.Distance(LPos, RPos)));
        //     //Debug.Log(Mathf.Abs(Quaternion.Angle(lRot, rRot)));
        // }

        bool isControllersClose = Mathf.Abs(Vector3.Distance(LPos, RPos) - ControllerGap) < 0.2f;
        bool isControllersAligned = Mathf.Abs(Quaternion.Angle(lRot, rRot) - ControllerDegreeGap) < 20.0f;

        GameObject[]weapons = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (GameObject weapon in weapons)
        {
            if (isControllersClose && isControllersAligned)
            {
                ////Debug.Log("Blocked");
                weapon.GetComponent<Sword>().ToggleBlocking(true);
            }
            else
            {
                weapon.GetComponent<Sword>().ToggleBlocking(false);
            }

            if (Input.GetKey(KeyCode.B))
            {
               ////Debug.Log("Blocked");
                weapon.GetComponent<Sword>().ToggleBlocking(true);
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                weapon.GetComponent<Sword>().ToggleBlocking(false);
            }
        }
    }
}
