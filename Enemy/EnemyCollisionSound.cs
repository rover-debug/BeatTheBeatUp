using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionSound : MonoBehaviour
{
    public AudioClip HitGround;
    public AudioSource hitSoundSource;
    private bool TriggerOnce = true;
    Enemy enemy;

    private void Start()
    {
        //hitSoundSource = GetComponent<AudioSource>();
        enemy = transform.root.GetComponent<Enemy>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if(TriggerOnce && collider.gameObject.tag == "ground" && enemy.IsDead)
        {
            hitSoundSource.clip = HitGround;
            hitSoundSource.volume = 0.25f;
            hitSoundSource.Play();
            TriggerOnce = false;
            GameObject NewDust = Instantiate(enemy.DustEffect);
            NewDust.transform.position = transform.position; 
            NewDust.GetComponent<DustEffect>().TriggerDustEffect();
            StartCoroutine(DisableEffectSound());

        }
    }
    IEnumerator DisableEffectSound()
    {
        yield return new WaitForSeconds(2);
        hitSoundSource.enabled = false;
    }
}
