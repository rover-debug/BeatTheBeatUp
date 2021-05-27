using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceManager : MonoBehaviour
{
    [SerializeField]
    GameObject pathPrefab;
    Vector3 origin;

    bool started;
    float sinceStart;
    bool instantiated;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position + transform.forward * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            if(OVRInput.Get(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.D))
            {
                started = true;
            }
        }
        else
        {
            sinceStart += Time.deltaTime;
            if (!instantiated)
            {
                instantiated = true;
                Instantiate(pathPrefab, origin, Quaternion.identity);
            }
        }
    }

    public void StartDanceInterlude()
    {
        started = true;
    }
}
