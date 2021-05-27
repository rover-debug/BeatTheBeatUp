using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    GameObject PlayAreaBox;

    public AudioSource TutorialAudio;

    private bool TriggeredTutorial = false;
    [SerializeField] AudioSource tutorialDholAudioSource;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(PlayAreaBox == null)
        {
            PlayAreaBox = GameObject.Find("TrackingSpace/CenterEyeAnchor");
        }
        
        {
            Vector3 pos = PlayAreaBox.transform.position;
            // pos.y += 1.5f;
            pos.x += 0.6f;
            transform.position = pos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayAreaBox == null)
        {
            PlayAreaBox = GameObject.Find("TrackingSpace/CenterEyeAnchor");
        }
        
        if (!TriggeredTutorial && Mathf.Abs(transform.position.y - PlayAreaBox.transform.position.y) > 0.25)
        {
            Vector3 pos = PlayAreaBox.transform.position;
            // pos.y += 1.5f;
            pos.x += 0.4f;
            transform.position = pos;
            StartCoroutine(PlayAreaBox.GetComponent<OVRScreenFade>().Fade(1, 0));
        }
    }

    IEnumerator DelayTutorialStart(TutorialManager manscript)
    {
        while (TutorialAudio.isPlaying)
        {
            yield return new WaitForSeconds(0.2f);
        }
        foreach (Transform ChildObj in transform)
        {
            ChildObj.gameObject.SetActive(false);
        }
        TutorialManager.instance.MarkTutMusicBegin(Time.time);
        tutorialDholAudioSource.Play();
        manscript.AddTutorialEnemy();
        Destroy(this.transform.gameObject);
        
        
    }

    public void TriggerTutorialCheck()
    {
        var manobj = GameObject.Find("TutorialManager");
        if (manobj != null)
        {
            var manscript = manobj.GetComponent<TutorialManager>();
            if(manscript != null && !TriggeredTutorial)
            {
                //manscript.tutorialFinished = true;
                TutorialAudio.Play();
                StartCoroutine(DelayTutorialStart(manscript));
                var gameInfor = GameObject.Find("GameManager");
                if (manobj != null)
                {
                    gameInfor.GetComponent<GameInfor>().GetPlayerPosition();
                }
                TriggeredTutorial = true;
                //GetComponent<MeshRenderer>().enabled = false;
                
                //transform.GetChild(0).gameObject.SetActive(false);
                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Contains("hands"))
        {
            
            TriggerTutorialCheck();
            
        }
    }
}
