using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{   [SerializeField]
    public Panel currentpanel = null;

    private List<Panel> panelHistory = new List<Panel>(); // Needs to know previous UI state.

    private void Start()
    {
        SetUpPanels();
    }

    private void SetUpPanels()
    {
        Panel[] panels = GetComponentsInChildren<Panel>();
        
        foreach(Panel panel in panels)
        {
            panel.SetUp(this);
        }

        currentpanel.Show(); // Pick up for part 2 https://www.youtube.com/watch?v=0otP3ww-auE&t=19s
    }

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            GoToPreviouis();
        }
    }

    public void GoToPreviouis()
    {
        if(panelHistory.Count == 0)
        {
            OVRManager.PlatformUIConfirmQuit(); // Quits through Oculus UI
            return;
        }

        int  lastIndex = panelHistory.Count - 1;
        SetCurrent(panelHistory[lastIndex]);
        panelHistory.RemoveAt(lastIndex);
    }


    public void PlayGame()
    {
        //SceneManager.LoadScene("Punching");
        StartCoroutine(GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOutLoadScene("Punching"));
    }
    public void SetCurrentWithHistory(Panel newPanel)
    {
        panelHistory.Add(currentpanel); 
        SetCurrent(newPanel);
    }

    private void SetCurrent(Panel newPanel)
    {
        currentpanel.Hide();
        currentpanel = newPanel;
        currentpanel.Show();
    }

}
