using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : PathObjectBase
{
    
    // Start is called before the first frame update
    void Start()
    {
        parentDanceMove = transform.parent.GetComponent<DanceMove>();
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canHit && !finished)
        {
            finished = true;
            GetComponent<Renderer>().material.color = Color.green;
            parentDanceMove.NextPathObject();
            DanceScore.instance.AddScore();
        }
    }
}
