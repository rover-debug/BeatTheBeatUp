using UnityEngine;


public class Grunt : Enemy
{

    //protected float attackRange;
    [SerializeField] Transform hand;
    Punch1 punchScript;
    public Grunt()
    {
        
     
    }
    public void Start()
    {
        punchScript = GetComponent<Punch1>();
        base.Start();
    }

    public void TakeDamage(Collider collider)
    {
        //punchScript.Attacked(collider);
    }


}