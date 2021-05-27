using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crackCollision : MonoBehaviour
{
    // Start is called before the first frame update
    private TutorialManager m_tutManager;
    bool cracked = false;
    private void Start()
    {
        m_tutManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (m_tutManager.tutorialFinished && other.CompareTag("Attackable") && !cracked)
        {
            GetComponent<MeshRenderer>().enabled = true;
            cracked = true;
            GetComponent<AudioSource>().Play();
        }
    }
}
