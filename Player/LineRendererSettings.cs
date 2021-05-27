using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererSettings : MonoBehaviour
{
    [SerializeField] LineRenderer rend;

    Vector3[] points;

    public LayerMask layerMask;

    public GameObject panel;
    public Button btn;
    public Image img;
    // public Button QuitBtn;
    // public Image QuitBtnImg;
    // public Button EscapeBtn;
    // public Image EscapeBtnImg;
    // public Button RestartBtn;
    // public Image RestartBtnImg;

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<LineRenderer>();

        points = new Vector3[2];

        points[0] = Vector3.zero;

        points[1] = transform.position + new Vector3(0, 0, 20);

        rend.SetPositions(points);
        rend.enabled = false;

        img = panel.GetComponent<Image>();
        
        
    }

    public void EnableLineRenderer(bool disable = true)
    {
        rend.enabled = disable;
    }

    public bool AlignLineRenderer(LineRenderer rend)
    {
        bool hitBtn = false;
        Ray ray;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        points[1] = transform.forward + new Vector3(0, 0, 20);
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            if (hit.collider.name.Contains("Button"))
            {
                //points[1] = transform.forward + new Vector3(0, 0, hit.distance);
                rend.startColor = Color.red;
                rend.endColor = Color.red;
                btn = hit.collider.gameObject.GetComponent<Button>();
                hitBtn = true;
            }
        }
        else
        {
            
            rend.startColor = Color.green;
            rend.endColor = Color.green;
            hitBtn = false;
        }

        rend.SetPositions(points);
        return hitBtn;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        if (rend.enabled)
        {
            if (AlignLineRenderer(rend) && OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                btn.onClick.Invoke();
                rend.enabled = false;
            }
            rend.material.color = rend.startColor;
        }
    }
}
