using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceMove : MonoBehaviour
{
    [SerializeField]
    private GameObject[] pathObjects;
    int currentObject;
    [SerializeField]
    bool selfDestruct;
    [SerializeField]
    float destructTime;
    float sinceStart;
    void Start()
    {
        currentObject = -1;
        NextPathObject();
    }

    // Update is called once per frame
    void Update()
    {
        sinceStart += Time.deltaTime;
        if(sinceStart >= destructTime && selfDestruct)
        {
            Destroy(gameObject);
        }
    }

    public void NextPathObject()
    {
        ++currentObject;
        if (currentObject < pathObjects.Length)
        {
            pathObjects[currentObject].GetComponent<PathObjectBase>().EnableHit();
        }
        // finished path
        else if (currentObject == pathObjects.Length)
        {
            Destroy(gameObject);
        }
    }
}
