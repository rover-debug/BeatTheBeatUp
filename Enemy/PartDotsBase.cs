using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDotsBase : MonoBehaviour
{
    public Dictionary<string, GameObject> mDotDict;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void GetDots()
    {
    }

    public virtual void ActivateDotAtPart(string part, float span, string attack)
    {
        string partName = "";
        if (part.Contains("Head"))
        {
            partName = "Head";
        }
        else if (part.Contains("Left Stomach"))
        {
            partName = "Left Stomach";
        }
        else if (part.Contains("Right Stomach"))
        {
            partName = "Right Stomach";
        }
        else if (part.Contains("Chest"))
        {
            partName = "Chest";
        }
        if(partName.Length > 0)
        {
            mDotDict[partName].GetComponent<Dot1>().ActivateDot(span, attack);
        }
    }
}
