using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAnim : MonoBehaviour
{
    float time = 0.0f;
    bool runTriggered = false;

    [Range(0, 2)]
    public int hitCount;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            GetComponent<Animator>().SetTrigger("gruntAttack");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetComponent<Animator>().SetTrigger("midHead");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetComponent<Animator>().SetTrigger("chest");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetComponent<Animator>().SetTrigger("midStomach");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            bool run = GetComponent<Animator>().GetBool("run");
            GetComponent<Animator>().SetBool("run", !run);
            GetComponent<Animator>().SetFloat("slowDownFactor", 1.0f);
        }

        GetComponent<Animator>().SetInteger("hitCount", hitCount);
    }
}
