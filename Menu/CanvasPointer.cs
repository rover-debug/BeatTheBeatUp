using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CanvasPointer : MonoBehaviour
{
    public float defaultLength = 3.0f;

    public EventSystem eventSystem = null;

    public StandaloneInputModule inputModule = null;

    private LineRenderer lineRenderer = null;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateLength();
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, GetEnd());
    }

    private Vector3 GetEnd()
    {
        float distance = GetCanvasDistance();
        Vector3 endPosition = DefaultEnd(defaultLength);

        if(distance != 0.0f)
        {
            endPosition = DefaultEnd(distance);
        }

        return endPosition;
    }

    private float GetCanvasDistance()
    {   PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputModule.inputOverride.mousePosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        
        eventSystem.RaycastAll(eventData, results);
        
        RaycastResult closestResult = FindFirstRaycast(results);
        
        float distance = closestResult.distance;
        
        //Clamp distance
        distance = Mathf.Clamp(distance, 0.0f, defaultLength);

        return distance;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
       foreach(RaycastResult result in results)
       {
           if(!result.gameObject)
           {
               continue; 
           }

           return result;
       }

       return new RaycastResult();
    }
    private Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }
}
