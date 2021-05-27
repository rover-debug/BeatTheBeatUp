using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankGrunt : Enemy2
{
    // public int Health;
    //private float attackRange;
    public TankGrunt()
    {

    }
    public void Start()
    {
        attackRange = 0.3f;
        //base.attackTime = 2.0f;
        base.slowDownFactor = 0.25f;

        Vector3 towardsPlayer = playerPos - transform.position;
        base.destination = transform.position + Vector3.Normalize(towardsPlayer) * (Vector3.Magnitude(towardsPlayer) - attackRange);
        base.destination.y = transform.position.y;
        base.slowDownPos = destination - Vector3.Normalize(towardsPlayer) * slowDownDistanceScalar;

    }
    public void TakeDamage()
    {
        Health = base.TakeDamage(Health);

        if (Health <= 0)
        {
            base.Death(this.gameObject);

            //Debug.Log("deadf");
        }
        else
        {
            base.Hitted(this.gameObject);
            //Debug.Log("Hitted");
        }
    }
}