using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustEffect : MonoBehaviour
{
    bool played = false;
    float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(played)
        {
            time += Time.deltaTime;
            if(time >= 2.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TriggerDustEffect()
    {
        if(!played)
        {
            GetComponent<ParticleSystem>().Play();
            played = true;
        }
    }
}
