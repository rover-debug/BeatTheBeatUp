using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float RotYLimit = 100.0f;
    public float RotSpeed = 3.0f;
    private float RotationTime = 0f;

    public bool isBlocking = false;

    public SphereCollider LeftHand;
    public SphereCollider RightHand;

    private bool BlockedEnemy = false;

    public Animator EnemyAnimator;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        LeftHand = GameObject.Find("LeftHandAnchor").gameObject.GetComponent<SphereCollider>();
        RightHand = GameObject.Find("RightHandAnchor").gameObject.GetComponent<SphereCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        /*float direction = Mathf.Sin(Time.time * RotSpeed);
         if (direction < 0.0f)
         {
             GetComponent<Renderer>().enabled = false;
             gameObject.GetComponent<Renderer>().material.color = Color.white;
             //transform.Rotate(0f, 0f, RotYLimit * direction);
         }
         else
         {
              GetComponent<Renderer>().enabled = true;
         }
         transform.Rotate(RotYLimit * direction * Time.deltaTime, 0f, 0f);
         */
        if (Input.GetKeyDown(KeyCode.U))
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            EnemyAnimator.speed = -2.0f;
            
        }

        if (isBlocking && BlockedEnemy)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;

            //this.transform.parent.GetComponent<BatGrunt>().Blocked();
            //this.transform.parent.transform.Find("Enemy_low").GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
            this.transform.parent.tag = "Attackable";
        }
        else
        {
            //this.transform.parent.GetComponent<Animator>().SetBool("blocked", false);
            //this.transform.parent.transform.Find("Enemy_low").GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            this.transform.parent.tag = "Untagged";
        }
    }

    public void ToggleBlocking(bool isBlock)
    {
        if (isBlock && !isBlocking && this.transform.parent.gameObject.GetComponent<Grunt>().reachDest)
        {
            ////Debug.Log("Block received");
            // LeftHand.radius *= 3;
            // RightHand.radius *= 3;
            isBlocking = true;

            // calculate score
            gameObject.GetComponentInParent<Enemy2>().CalculateBlock();
        }
        else if (!isBlock && isBlocking)
        {
            // LeftHand.radius /= 3;
            // RightHand.radius /= 3;
            isBlocking = false;
        }
        
    }
    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log(collider.gameObject.name);
        if (collider.gameObject.name.Contains("Hand") && isBlocking)
        {
            EnemyAnimator.ResetTrigger("gruntAttack");
            
            BlockedEnemy = true;
            

            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            EnemyAnimator.speed = -2.0f;

            StartCoroutine(BlockRumble());
            //this.transform.parent.GetComponent<BatGrunt>().Blocked();
            //this.transform.parent.transform.Find("Enemy_low").GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
        }
        else if (collider.gameObject.name.Contains("Enemy"))
        {
            Destroy(collider.gameObject);
        }
    }

    private IEnumerator BlockRumble()
    {
        // OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        // OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);
        yield return new WaitForSeconds(0.2f);
        // OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        // OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        // Score.instance.SetScore(500);
        // Destroy(transform.root.gameObject);
    }
}
