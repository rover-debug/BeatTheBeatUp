using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaSignify : MonoBehaviour
{
    private bool signify;
    private Material mat;
    [SerializeField]
    GameObject warningText;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = Color.yellow;
        transform.position = GameObject.Find("OVRCameraRig (new)").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(signify)
        {
            float t_float = Time.time;
            int t_int = Mathf.FloorToInt(t_float);
            if(t_float - t_int < 0.5f)
            {
                mat.color = Color.white;
            }
            else
            {
                mat.color = Color.yellow;
            }
            warningText.SetActive(true);
        }
        else
        {
            mat.color = Color.yellow;
            warningText.SetActive(false);
        }
    }

    public void SetSignify(bool on)
    {
        signify = on;
    }
}
