using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClicker : MonoBehaviour
{
    Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform != null)
                {
                    
                    if (hit.transform.name.Contains("Ragdoll"))
                    {
                        //Debug.Log(hit.transform.name);
                        hit.transform.GetComponent<Grunt>().TakeDamage(new Collider());
                    }
                    else if (hit.transform.tag == "TankGrunt")
                        hit.transform.GetComponent<TankGrunt>().TakeDamage();
                    else if (hit.transform.tag == "BatGrunt")
                        hit.transform.GetComponent<BatGrunt>().TakeDamage();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform != null)
                {
                    hit.transform.GetComponent<Enemy2>().Move();
                }
            }
        }

    }

}