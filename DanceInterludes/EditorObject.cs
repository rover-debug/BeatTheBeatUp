using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorObject : MonoBehaviour
{
    public DIObject type;

    private bool used;
    private int order;
    private bool assigningOrder;
    private bool assigned;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(assigningOrder && !assigned && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            assigned = true;
            order = DIEditor.instance.GetNextIndex(gameObject);
            GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public void SetUsed(bool pUsed = true)
    {
        used = pUsed;
        if (used)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    public bool IsUsed()
    {
        return used;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DIEditor.instance.IsOrderMode() && other.CompareTag("hand"))
        {
            assigningOrder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (DIEditor.instance.IsOrderMode() && other.CompareTag("hand"))
        {
            assigningOrder = false;
        }
    }

    public int GetOrderIdx()
    {
        return order;
    }
}
