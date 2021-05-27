using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathObjectBase : MonoBehaviour
{
    protected DanceMove parentDanceMove;
    protected bool canHit, finished;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableHit(bool pCanHit = true)
    {
        canHit = pCanHit;
    }
}
