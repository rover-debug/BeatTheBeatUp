using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplode : MonoBehaviour
{

    public GameObject BonusHit;
    public AudioClip[] explosionAudioClips = new AudioClip[5];
    private GameObject ExplosionObj;
    private AudioSource ExploisionAudio;

    private float radius = 10.0F;
    private float power = 750.0F;
    

    bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        ExplosionObj = GameObject.Find("Explosion");
        ExploisionAudio = ExplosionObj.GetComponent<AudioSource>();

        //BonusHit = transform.Find("score_pot/Bonus").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.root.gameObject.CompareTag("enemy"))
        {
            Explode();
        }
        else if(collision.transform.root.gameObject.CompareTag("ground"))
        {
            foreach (Transform child in this.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }
    }

    IEnumerator DelayGravity(Rigidbody rb)
    {
        yield return new WaitForSeconds(1.5f);
        rb.useGravity = true;
    }

    IEnumerator Disappear(GameObject gb)
    {
        yield return new WaitForSeconds(0.25f);
        gb.SetActive(false);
        BonusHit.SetActive(false);
    }

    private void randomSetAudioSource()
    {
        var number = Random.Range(0, 5);
        ExploisionAudio.clip = explosionAudioClips[number];
        ////Debug.Log("Explosion Audio Clip: " + ExploisionAudio.clip.name);
    }

    public void Explode()
    {
        if (!exploded)
        {

            ExplosionObj.transform.position = transform.position;
            exploded = true;
            foreach (ParticleSystem EffectObj in ExplosionObj.GetComponentsInChildren<ParticleSystem>())
            {
                EffectObj.Play();
            }
            randomSetAudioSource();
            ExploisionAudio.Play();
            BonusHit.SetActive(true);

            foreach (Transform child in this.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 ExplosionOrigin = transform.position;
                    ExplosionOrigin.y += 2.5f;
                    if (Random.value < 0.5f) 
                    {
                        ExplosionOrigin.x -= 2.5f*Random.value;
                    }
                    else
                    {
                        ExplosionOrigin.z -= 2.5f*Random.value;
                    }
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                    rb.AddExplosionForce(power, ExplosionOrigin, radius, 0f, ForceMode.Force);
                    StartCoroutine(Disappear(child.gameObject));
                }
                
                MeshRenderer mr = child.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.material.color = Color.black;
                }
                    
            }
            

            
        }
    }
}

