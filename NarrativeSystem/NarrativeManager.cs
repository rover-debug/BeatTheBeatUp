using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NarrativeManager : MonoBehaviour
{
    public TMPro.TMP_Text dialogueText;
    private Renderer m_Renderer;
    private Queue<string> sentences;
    private bool displaying;
    private GameObject curImg = null;

    private GameObject oldImg = null;
    [SerializeField] AudioSource audioSource;
    //public int SlidesCounter = 3;

    // Start is called before the first frame update
    public void Awake()
    {
        //Fetch the Renderer from the GameObject
        m_Renderer = GetComponent<Renderer>();
        sentences = new Queue<string>();
        displaying = false;
        curImg = null;
        //Debug.Log("Started: " + sentences);
    }

    // Update is called once per frame
    public void Update()
    {

        //DisplayImage();
        if (GameInForNar.fadeVolume)
        {
            //Debug.Log("vol" + GameInForNar.fadeVolume);
            audioSource.volume -= (Time.deltaTime*0.5f);
        }

    }

    public void StartDialogue (Slide slides)
    {
        
        //Debug.Log("Starting Conversation with " + slides.name);
        sentences.Clear();

        foreach (string sentence in slides.sentences)
        {
            sentences.Enqueue(sentence);
        }
        oldImg  = curImg;
        curImg = slides.image;
        DisplayImage();
        StartCoroutine(DisplayNextSentence(slides.secondBetweenSentences)); 
    }


    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        
    }

    IEnumerator DisplayNextSentence (int second)
    {
        while (sentences.Count != 0)
        {
            string sentence = sentences.Dequeue();
            ////Debug.Log(sentence);
            StartCoroutine(TypeSentence(sentence));
            yield return new WaitForSeconds(second);
        }
        EndDialogue();

        yield return null;
    }

    public void DisplayImage()
    {
        if (oldImg == this.gameObject)
        {
            m_Renderer.enabled = false;
        }
        else if (oldImg != null)
        {
            oldImg.SetActive(false);
        }

        curImg.SetActive(true);
        //m_Renderer.material.SetTexture("_BaseMap", image);
    }

    public void EndDialogue()
    {
        //SlidesCounter -= 1;
        //Debug.Log("End of dialogus!");
        dialogueText.text = "";
        /*
        if (SlidesCounter == 0)
        {
            if (SceneManager.GetActiveScene().name == "NarrativeScene")
                SceneManager.LoadScene("Punching");
        }
        */
    }
 

}


