using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHand : PathObjectBase
{
    [SerializeField]
    float requiredStayTime;
    float stayed;
    bool inTrigger;
    // Start is called before the first frame update
    void Start()
    {
        parentDanceMove = transform.parent.GetComponent<DanceMove>();
        GetComponent<Renderer>().material.color = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger)
        {
            stayed += Time.deltaTime;
            DanceScore.instance.DisplaySpecial(stayed);
            if (!finished && stayed >= requiredStayTime)
            {
                finished = true;
                GetComponent<Renderer>().material.color = Color.green;
                parentDanceMove.NextPathObject();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        inTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
        stayed = 0;
    }
}
