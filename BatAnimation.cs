using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAnimation : MonoBehaviour
{
    private float RotYLimit = 200.0f;
    private float RotSpeed = 3.0f;
    private float RotationTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.parent.gameObject.GetComponent<Animator>().SetTrigger("gruntAttack");
        }

        //test for attack and block System
        if (transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Grunt Attack"))
        {
            float direction = Mathf.Sin(RotationTime * RotSpeed);
            transform.Rotate(RotYLimit * direction * Time.deltaTime, 0f, 0f);
            RotationTime += Time.deltaTime;
            
        }
        else
        {
            RotationTime = 0f;
            this.transform.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
    }
}
