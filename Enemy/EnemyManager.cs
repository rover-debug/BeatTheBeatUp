using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public UnityEvent AttackEvent;
    public Transform HittedPosition;
    public Vector3 HitDirection;
    public string PunchHandName;
    public string HittedPart;

    public float NormScore;

    public float HitForce;

}
