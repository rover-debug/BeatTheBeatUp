using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideTrigger : MonoBehaviour
{
    //public int secondsBetweenSlides;
    public Slide[] slides;
    private NarrativeManager nm;
   
    // Start is called before the first frame update
    void Start()
    {
        nm = FindObjectOfType<NarrativeManager>();

        StartCoroutine(MyCoroutine());


    }


    IEnumerator MyCoroutine()
    {
        nm.StartDialogue(slides[0]);

        yield return new WaitForSeconds(slides[0].slideDuration);

        nm.StartDialogue(slides[1]);

        yield return new WaitForSeconds(slides[1].slideDuration);

        nm.StartDialogue(slides[2]);

        yield return new WaitForSeconds(slides[2].slideDuration);

        if (SceneManager.GetActiveScene().name == "NarrativeScene")
            // SceneManager.LoadSceneAsync("Punching");
            StartCoroutine(GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOutLoadScene("Punching"));
    }
}
