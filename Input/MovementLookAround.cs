using UnityEngine;
using System.Collections;

public class MovementLookAround : MonoBehaviour {

    public float MouseSensitivity = 1;

    public float PanSpeed = 20.0f;

    float yVelocity = 0.0f;
    float xVelocity = 0.0f;

    public float upDownRange = 1.0f;

    public bool DisableVR = false;
    public GameObject[] VRObjs;

    void Start()
    {
        if (DisableVR)
        {
            for (int i = 0; i < VRObjs.Length; i++)
            {
                VRObjs[i].SetActive(false);
            }
        }
    }

    public void DisableVRMode()
    {
        if (DisableVR)
        {
            for (int i = 0; i < VRObjs.Length; i++)
            {
                VRObjs[i].SetActive(false);
            }
        }
    }
    void Update()
    {
        xVelocity += Input.GetAxis("Mouse X") * MouseSensitivity;
        yVelocity -= Input.GetAxis("Mouse Y") * MouseSensitivity;

        //xVelocity = Mathf.Lerp(transform.rotation., rotX, snappiness * Time.deltaTime);
        // yVelocity = Mathf.Lerp(yVelocity, rotY, snappiness * Time.deltaTime);

        ////Debug.Log(xVelocity);
        ////Debug.Log(yVelocity);
 
        //RotY
        yVelocity = Mathf.Clamp(yVelocity, -upDownRange, upDownRange);
        //transform.localRotation = Quaternion.Euler(yVelocity, transform.localRotation.y, transform.localRotation.z);

        //RotX
        // transform.Rotate(rotY, rotX, 0);
   
        transform.eulerAngles = new Vector3(yVelocity, xVelocity, 0.0f);

        Vector3 PlayerPos = transform.position;
        Vector3 CamForward = Camera.main.transform.forward;
        Vector3 CamRight = Camera.main.transform.right;

        CamForward.y = 0;
        CamRight.y = 0;

        if (Input.GetKey("w"))
        {
            PlayerPos += PanSpeed * CamForward * Time.deltaTime;
        }
        else  if (Input.GetKey("a"))
        {
            PlayerPos -= PanSpeed * CamRight * Time.deltaTime; 
        }
        else  if (Input.GetKey("s"))
        {
            PlayerPos -= PanSpeed * CamForward * Time.deltaTime;
        }
        else  if (Input.GetKey("d"))
        {
            PlayerPos += PanSpeed * CamRight * Time.deltaTime;
        }

        transform.position = PlayerPos;

    }
        
}