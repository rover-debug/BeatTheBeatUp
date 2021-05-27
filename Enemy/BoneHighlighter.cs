using UnityEngine;
using System.Collections;

public class BoneHighlighter : MonoBehaviour
{
    Renderer bodyRenderer;
    Renderer headRenderer;

    public string modelName;
    public Material HitMaterialHead;
    public Material HitMaterialShirt;

    float bodyTime, headTime;

    bool hold;
    
    int LastIdx = -1;
    private void Start()
    {
        Transform HeadObj = transform.Find("model:geo/model:Head_low");
        if (HeadObj == null)
        {
            headRenderer = transform.Find(modelName + "_low/Body_low").gameObject.GetComponent<Renderer>();
        }
        else
        {
            headRenderer = HeadObj.GetComponent<Renderer>();
        }
        Transform Shirt1Obj = transform.Find("model:geo/model:Shirt_low");
        if (Shirt1Obj == null)
        {
             bodyRenderer = transform.Find(modelName + "_low/Shirt_low").gameObject.GetComponent<Renderer>();
        }
        // else
        // {
        //     GameObject Shirt2Obj = transform.Find("model:geo/model:Shirt2_low").gameObject;
        //     bodyRenderer = Shirt1Obj.GetComponent<Renderer>();
        //     int ShirtChoice = UnityEngine.Random.Range(0, 2);
        //     if (ShirtChoice == 1)
        //     {
        //         bodyRenderer = Shirt2Obj.GetComponent<Renderer>();
        //         Shirt1Obj.gameObject.SetActive(false);
        //     }
        //     else
        //     {
        //         Shirt2Obj.SetActive(false);
        //     }
        // }
        
    }

    public void SetBodyRenderer(Renderer renderer)
    {
        bodyRenderer = renderer;
    }

    private void Update()
    {
        bodyTime -= Time.deltaTime;
        if (bodyTime <= 0 && !hold && bodyRenderer)
        {
            bodyRenderer.material.SetInt("_PartIndex", 0);
            bodyTime = 0;
        }

        headTime -= Time.deltaTime;
        if (headTime <= 0 && !hold && headRenderer)
        {
            headRenderer.material.SetInt("_PartIndex", 0);
            headTime = 0;
        }
    }

    public void DisableHighlight(string part)
    {
        int idx = bodyRenderer.material.GetInt("_PartIndex");

        int hundreds = idx / 100;
        int tens = (idx - hundreds * 100) / 10;
        int ones = idx - hundreds * 100 - tens * 10;

        if (part.Contains("Head"))
        {
            ones = 0;
        }
        else if (part.Contains("Chest"))
        {
            tens = 0;
        }
        else if (part == "Stomach")
        {
            hundreds = 0;
        }

        idx = hundreds * 100 + tens * 10 + ones;

        headRenderer.material.SetInt("_PartIndex", idx);
        bodyRenderer.material.SetInt("_PartIndex", idx);

        // Color BaseColor = new Color(1f,1f,1f,1);
        // //BaseColor = Color.clear;
        // if(part.Contains("Head"))
        // {
        //     //headRenderer.material.SetColor("_HeadHLColor", BaseColor);
        //     headRenderer.material.SetColor("_HighLightColor", BaseColor);
        // }
        // else
        // {
        //     bodyRenderer.material.SetColor("_HighLightColor", BaseColor);
        // }

        // if(part.Contains("LeftStomach"))
        // {
        //     bodyRenderer.material.SetColor("_StomachHLColor", BaseColor);
        // }
        // else if(part.Contains("RightStomach"))
        // {
        //     bodyRenderer.material.SetColor("_StomachHLColor", BaseColor);
        // }
        // else if(part.Contains("Chest"))
        // {
        //     bodyRenderer.material.SetColor("_ChestHLColor", BaseColor);
        // }
        // else if (part == "Stomach")
        // {
        //     bodyRenderer.material.SetColor("_StomachHLColor", BaseColor);
        // }
        
    }

    /*-------------------
    ones : head
    tens : stomach
    hundreds : chest

    1 = 001 = head
    11 = 011 = stomach & head
    101 = chest & head
    000 = none
    -------------------*/

    public void HighlightPart(string part, float span, bool isHit = false)
    {
        int idx;
        bool head = false, body = false;
        if(part.Contains("Head"))
        {
            idx = 0;
            head = true;
            LastIdx = 0;
        }
        else if(part.Contains("LeftStomach"))
        {
            idx = 1;
            LastIdx = 1;
            body = true;
        }
        else if(part.Contains("RightStomach"))
        {
            idx = 2;
            LastIdx = 2;
            body = true;
        }
        else if(part.Contains("Chest"))
        {
            body = true;
            idx = 3;
        }
        else if (part == "Stomach")
        {
            idx = LastIdx;
            body = true;
        }
        else
        {
            idx = -1;
        }

        if (body)
        {
            bodyTime = span;
            bodyRenderer.material.SetInt("_PartIndex", idx);
            if (isHit && LastIdx > 0) 
            {
                //bodyRenderer.material = HitMaterialShirt;
                bodyRenderer.material.SetColor("_HighLightColor", Color.green);
            }
        }
        if (head)
        {
            headTime = span;
            headRenderer.material.SetInt("_PartIndex", idx);
            if (isHit && LastIdx >= 0) 
            {
                //headRenderer.material = HitMaterialHead;
                headRenderer.material.SetColor("_HighLightColor", Color.green);
            }
        }
    }

    public void HighlightPart(string part, Color color, bool pHold=false)
    {
        hold = pHold;
        int idx = bodyRenderer.material.GetInt("_PartIndex");
        Color head = headRenderer.material.GetColor("_HeadHLColor"),
              stomach = bodyRenderer.material.GetColor("_StomachHLColor"),
              chest = bodyRenderer.material.GetColor("_ChestHLColor");
        int hundreds = idx / 100;
        int tens = (idx - hundreds * 100) / 10;
        int ones = idx - hundreds * 100 - tens * 10;

        if (part.Contains("Head"))
        {
            ones = 1;
            head = color;
        }
        else if (part.Contains("Chest"))
        {
            tens = 1;
            chest = color;
        }
        else if (part == "Stomach")
        {
            hundreds = 1;
            stomach = color;
        }
        idx = hundreds * 100 + tens * 10 + ones;

        headRenderer.material.SetColor("_HeadHLColor", head);
        bodyRenderer.material.SetColor("_StomachHLColor", stomach);
        bodyRenderer.material.SetColor("_ChestHLColor", chest);

        headRenderer.material.SetInt("_PartIndex", idx);
        bodyRenderer.material.SetInt("_PartIndex", idx);
    }

}
