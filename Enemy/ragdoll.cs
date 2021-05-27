using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdoll : MonoBehaviour
{
    [SerializeField] public GameObject[] ragdollList;
    [SerializeField] public GameObject[] attackableParts;
    private GameObject ragdollPart;
    public bool released;


    public void grab()
    {

        transform.gameObject.GetComponent<Animator>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        ragdollPart = ragdollList[0];
        foreach (var bodyPart in ragdollList)
        {
            bodyPart.GetComponent<Rigidbody>().isKinematic = false;
            bodyPart.GetComponent<Rigidbody>().useGravity = true;
        }

    }

    public void hit(Vector3 hitDirection,float hitForce)
    {
        transform.gameObject.GetComponent<Animator>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        foreach (var bodyPart in ragdollList)
        {
            var bodyRB = bodyPart.GetComponent<Rigidbody>();
            bodyRB.isKinematic = false;
            bodyRB.useGravity = true;
            bodyRB.AddForce(80*hitForce * hitDirection, ForceMode.Impulse);

        }
        transform.gameObject.GetComponent<Rigidbody>().AddForce(80*hitForce * hitDirection, ForceMode.Impulse);
        StartCoroutine(waitBeforeMakingTriggersFalse());
        

    }

    IEnumerator waitBeforeMakingTriggersFalse()
    {
        yield return new WaitForSeconds(0.25f);
        foreach (var attackablePart in attackableParts)
        {
            attackablePart.GetComponent<Collider>().isTrigger = false;
        }
    }


    public void release()
    {
        ragdollPart.GetComponent<Rigidbody>().isKinematic = false;
        transform.GetComponent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(destroyAfterReleased());
    }

    IEnumerator destroyAfterReleased()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
