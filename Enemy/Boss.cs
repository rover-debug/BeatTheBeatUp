using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy2
{
    //public int Health;
    public Boss()
    {

    }
    public void Start()
    {
        base.attackRange = 0.005f;
        base.attackTime = 0.6f;
        base.slowDownFactor = 0.25f;
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