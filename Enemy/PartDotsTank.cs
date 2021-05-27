using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDotsTank : PartDotsBase
{
    // inherited member
    // protected Dictionary<string, GameObject> mDotDict;

    // Start is called before the first frame update
    void Start()
    {
        mDotDict = new Dictionary<string, GameObject>();
        GetDots();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override protected void GetDots()
    {
        Transform headNode = transform.Find("Hips/Spine/Spine1/Spine2/Neck/Head");
        mDotDict.Add("Head", headNode.Find("Dot_head").gameObject);
        mDotDict.Add("Chest", headNode.Find("Dot_chest").gameObject);
        mDotDict.Add("Left Stomach", headNode.Find("Dot_leftstomach").gameObject);
        mDotDict.Add("Right Stomach", headNode.Find("Dot_rightstomach").gameObject);
    }
}
