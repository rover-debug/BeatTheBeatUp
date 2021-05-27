using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceGrab : MonoBehaviour
{
    public GameObject dest;
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    void Update()
    {
    }

    /// <summary>
    /// check  collision between grabable object and its destination
    /// destroy both after few seconds
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == dest)
        {
            GetComponent<Renderer>().material.color = Color.green;
            other.gameObject.SetActive(false);

        }

    }

}
