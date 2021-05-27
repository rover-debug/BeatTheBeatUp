using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    private Canvas canvas = null;
    private MenuManager menuManager = null;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }   

    public void SetUp(MenuManager menuManager)
    {
        this.menuManager = menuManager;
        Hide();
    } 

    public void Show()
    {
        canvas.enabled = true;
    }

    public void Hide()
    {   
        canvas.enabled = false;
        //Good for canvas groups both show and hide (Look up canvas group - for dom)
    }

}
