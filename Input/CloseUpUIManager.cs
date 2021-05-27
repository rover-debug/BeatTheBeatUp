using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CloseUpUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject camRig;
    public bool isMenuOpen;
    GameObject menu;
    GameObject endMenu;
    List<GameObject> submenus = new List<GameObject>();
    int selectedIdx;
    public GameObject UIHelper;
    public bool m_isGameEnd = false;
    public static bool restartGame = false;
    public static bool hasRestarted = false;

    public bool SkipTutorial = false;

    public static int SkipTutRestarts = 0;

    // Start is called before the first frame update
    void Start()
    {
        menu = transform.parent.Find("CloseUpUI").transform.Find("MainMenu").gameObject;
        endMenu = transform.parent.Find("UIEnd").gameObject;

        // //Debug.Log("static variable is " + restartGame + " at " + Time.time);

        if (SkipTutorial && SkipTutRestarts == 0)
        {
            CloseUpUIManager.restartGame = true;
            CloseUpUIManager.hasRestarted = false;
            // SkipTutRestarts++;
            // restartGame = true;
            // hasRestarted = true;
            // RestartGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // display and hide menu
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (!isMenuOpen)
            {
                ShowMainMenu();
            }
            else
            {
                HideMainMenu();
            }
        }
        if (isMenuOpen)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
    }

    public void SetGameEndPointer()
    {
        ShowPointer(true);
        endMenu.SetActive(true);
        m_isGameEnd = true;
    }

    public void ShowPointer(bool active)
    {
        // pointer can't be hidden after game ends
        if(!m_isGameEnd)
        {
            UIHelper.GetComponent<HandedInputSelector>().enabled = active;
            UIHelper.transform.Find("LaserPointer").gameObject.SetActive(active);
            UIHelper.transform.Find("Sphere").gameObject.SetActive(active);
        }
        else
        {
            UIHelper.GetComponent<HandedInputSelector>().enabled = true;
            UIHelper.transform.Find("LaserPointer").gameObject.SetActive(true);
            UIHelper.transform.Find("Sphere").gameObject.SetActive(true);
        }
    }

    void ShowMainMenu()
    {
        isMenuOpen = true;
        menu.SetActive(true);
        ShowPointer(true);
    }

    void HideMainMenu()
    {
        isMenuOpen = false;
        menu.SetActive(false);
        ShowPointer(false);

        ControlUISlider slider = menu.transform.parent.Find("GlobalVolumeSlider").gameObject.GetComponent<ControlUISlider>();
        slider.SetUIActive(false);
        menu.transform.parent.Find("GlobalVolumeSlider").gameObject.SetActive(false);
    }

    public void OpenVolumeControl()
    {
        menu.SetActive(false);
        ControlUISlider slider = menu.transform.parent.Find("GlobalVolumeSlider").gameObject.GetComponent<ControlUISlider>();
        menu.transform.parent.Find("GlobalVolumeSlider").gameObject.SetActive(true);
        slider.SetUIActive(true);
    }

    public void AdjustCamera()
    {
        camRig.GetComponent<CameraAdjustion>().AdjustHeight();
        HideMainMenu();
    }

    public void RestartGame()
    {
        HideMainMenu();
        //Debug.Log("Restarting");
        restartGame = true;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        hasRestarted = false;
        StartCoroutine(GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOutLoadScene(SceneManager.GetActiveScene().name));
    }

    public void BackToMainMenu()
    {
        //SceneManager.LoadScene(0);
        GoToCredits();
    }

    public void QuitGame()
    {
        HideMainMenu();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
    
    public void GoToCredits()
    {
        HideMainMenu();
        InterSceneFlags.mNarrativeCreditsOn = true;
        // SceneManager.LoadScene(3);
        StartCoroutine(GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOutLoadScene("CreditScene"));
    }
}
