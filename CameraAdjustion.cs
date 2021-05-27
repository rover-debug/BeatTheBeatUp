using Oculus.Platform.Samples.VrHoops;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraAdjustion : MonoBehaviour
{
    public float distanceLimitX;
    public float distanceLimitZ;
    
    public float distanceLimitY;
    private float playerHeight;
    [SerializeField]
    private Volume warningEffect;
    [SerializeField]
    private Volume introEffect;
    [SerializeField]
    private GameObject playArea;

    public OVRScreenFade FaderScript;
    private bool warningOn;

    public bool IsIntro = false;

    public bool IsMainMenu = false;

    private Vector3 playerPos;

    private Transform Player;
    public float DesginedHeight;
    ///<Summary>
    ///  Keep the camera height at the same level as enemy when start the game
    ///  have delay for 2 seconds
    ///<Summary>
    void Start()
    {
        Player = this.transform.Find("TrackingSpace/CenterEyeAnchor");

        Invoke("AdjustHeight", 2.0f);
        // if (!OVRManager.boundary.GetConfigured())
        // {
        //     // correct position if no guardian, 
        //     // otherwise use original position to keep playArea inside guardian
        //     // if (IsMainMenu)
        //     // {
        //     //     //GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOut();
        //     //     Invoke("AdjustHeight", 1.0f);
        //     // }
        //     // else
        //     {
        //         Invoke("AdjustHeight", 2.0f);
        //     }
            
        // }
        if (IsIntro)
        {
            ToggleIntro(0.01f, true);
            //Invoke("ToggleWarning", 1.0f);
        }

    }

    IEnumerator DelayFilmGrain()
    {
        yield return new WaitForSeconds(0.2f);
        ToggleWarning(true);
    }

    public void AdjustHeight(bool OnlyY = false)
    {
        playerHeight = Player.transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y - playerHeight + DesginedHeight, transform.position.z);

        if (playArea && !OnlyY)
        {
            Vector3 areaCube = playArea.transform.position;              
            areaCube.y = transform.position.y - playerHeight + DesginedHeight;
            Player.transform.position = areaCube;    
        }

        if (OnlyY)
        {
            StartCoroutine(Player.GetComponent<OVRScreenFade>().Fade(1, 0));
        }
        

    }

    private void FadeToStart()
    {
        FaderScript.SetFadeLevel(1);
        StartCoroutine(FaderScript.Fade(1, 0));
        
    }

    private void FixedUpdate()
    {
        bool CheckZDuringPlay = !IsInPlayArea(true);
        if (!IsInPlayArea() || CheckZDuringPlay)
        {
            if (IsMainMenu)
            {
                AdjustHeight();
            }
            else if (CheckZDuringPlay)
            {
                // //Debug.Log("issue");
                AdjustHeight(true);
            }
            else
            {
                if (!warningOn)
                {
                    //Debug.Log("warning");
                    warningOn = true;
                    StartCoroutine(TurnOnWarning());
                    playArea.GetComponent<PlayAreaSignify>().SetSignify(true);
                }
            }
        }
        else
        {
            if(warningOn)
            {
                warningOn = false;
                StartCoroutine(TurnOffWarning());
                playArea.GetComponent<PlayAreaSignify>().SetSignify(false);
            }
        }
    }

    bool IsInPlayArea(bool OnlyY = false)
    {
        Vector3 eyePos = Player.transform.position;
        float diffX = Mathf.Abs(eyePos.x - playArea.transform.position.x);
        float diffZ = Mathf.Abs(eyePos.z - playArea.transform.position.z);
        float diffY = Mathf.Abs(eyePos.y - DesginedHeight);

        if (OnlyY)
        {
            if (diffY > distanceLimitY)
                return false;
            else
                return true;
        }

        if(diffX > distanceLimitX || diffZ > distanceLimitZ)
        {
            return false;
        }

        return true;
    }

    public void ToggleWarning(bool TurnOn = true)
    {
        if (TurnOn)
        {
            //Debug.Log("turning on");
            StartCoroutine(TurnOnWarning());
        }
        else
        {
            StartCoroutine(TurnOffWarning());
        }
    }

    IEnumerator TurnOnWarning()
    {
        float transition = 0.3f;
        for(float t=0; t<transition; t+=Time.deltaTime)
        {
            warningEffect.weight = Mathf.Lerp(0, 1, t / transition);
            yield return null;
        }
    }

    IEnumerator TurnOffWarning()
    {
        
        float transition = 0.3f;
        for (float t = 0; t < transition; t += Time.deltaTime)
        {
            warningEffect.weight = Mathf.Lerp(1, 0, t / transition);
            yield return null;
        }
    }

    public void ToggleIntro(float transition, bool TurnOn = true)
    {
        if (TurnOn)
        {
            TurnOnIntro();
        }
        else
        {
            StartCoroutine(TurnOffIntro(transition));
        }
    }

    void TurnOnIntro()
    {
        introEffect.weight = 1;
    }

    IEnumerator TurnOffIntro(float transition)
    {
        for (float t = 0; t < transition; t += Time.deltaTime)
        {
            introEffect.weight = Mathf.Lerp(1, 0, t / transition);
            yield return null;
        }
    }

}
