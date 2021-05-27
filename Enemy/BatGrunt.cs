using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatGrunt : Enemy2
{
    public int Health;
    private float attackRange;
    public BatGrunt()
    {

    }
    public void Start()
    {
        attackRange = 0.6f;
        //base.attackTime = 3.0f;
        base.slowDownFactor = 0.25f;

        Vector3 towardsPlayer = playerPos - transform.position;
        base.destination = transform.position + Vector3.Normalize(towardsPlayer) * (Vector3.Magnitude(towardsPlayer) - attackRange);
        base.destination.y = transform.position.y;
        base.slowDownPos = destination - Vector3.Normalize(towardsPlayer) * slowDownDistanceScalar;
    }
    public void Blocked()
    {
        this.GetComponent<Animator>().SetBool("blocked", true);
    }
    public void TakeDamage()
    {
        Health = base.TakeDamage(Health);

        if (Health <= 0)
        {
            base.Death(this.gameObject);
        }
        else
        {
            base.Hitted(this.gameObject);
        }
    }
}