using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MLSpace;
using System.ComponentModel;

public class Enemy2 : MonoBehaviour
{
    public GameObject enemy => this.transform.gameObject;

    public float attackRange;
    [HideInInspector]
    protected float attackTime;
    [HideInInspector]
    public Vector3 playerPos;
    public bool attacked;
    public float slowDownFactor = 0.5f;
    [HideInInspector]
    public BodyColliderScript bcs;
    [HideInInspector]
    public List<HitEvent> hitEvents;
    private Coroutine currHighlight;
    private EnemyManager enemyManager;
    private HitEvent currentEvent;
    private AudioSource EnemyAudioPlayer;
    private OVRGrabbable ovg;
    public AudioClip[] GrabSounds;
    private Vector3 startPos;
    [HideInInspector]
    public Vector3 destination;
    [HideInInspector]
    public Vector3 slowDownPos;
    private Vector3 hitDirection;
    private float offset;
    public float slowDownDistanceScalar;
    private float runTime;
    protected float hitWindow;
    private float runStart;
    private float sinceStart;
    public float AttackDelay = 1.05f;
    public bool reachDest = false;
    public bool slowDown = false;
    private int checkEventIdx = 0;
    private int HighlightIdx = 0;

    protected float firstHitEnd;

    public float runSpeed = 0.0f;

    float reachSlowdownTime = 0.0f;

    public float Time1 = Mathf.Infinity;
    public float Time2 = Mathf.Infinity;

    protected float HighlightStartTime = 0.0f;
    private float HighlightEndTime = 0.0f;
    protected float AttackStartTime = 0.0f;
    private Vector3 zeroY = new Vector3(1, 0, 1);
    private bool anticipated;

    [HideInInspector]
    public bool IsAttacking = false;
    public int Health;

    static int flag = 0;

    public float SpawnDistance = 15.0f;

    private bool ReachedSlowdownPhase = false;
    private Dictionary<string, Coroutine> HighlightsArray = new Dictionary<string, Coroutine>();

    int SpawnIdx = 0;

    const string Hitted_Right_Head = "RightHead";
    const string Hitted_Left_Stomach = "LeftStomach";
    const string Enemy_Run = "Run";
    const string Enemy_Attack = "Grunt Attack";

    private string currentState;

    [SerializeField]
    private Material comboGruntBeard;
    /// <summary>
    /// set up variable before generating the enemy 
    /// </summary>
    /// <param name="pStartPos"> enemy position</param>
    /// <param name="pSpawnInfo"> EnemySpawnInformation</param>
    /// <param name="playerPosition">player position/destination, a fixed Vectors when game started</param>
    public void SetParameter(Vector3 pStartPos, EnemySpawn pSpawnInfo, Vector3 playerPosition)
    {
        startPos = pStartPos;
        SpawnIdx = pSpawnInfo.locationIdx;
        runStart = pSpawnInfo.runStart;
        hitEvents = pSpawnInfo.hits;

        // retrieve information from hit event including first hit start, last hit end...
        currentEvent = hitEvents[0];
        Health = hitEvents.Count;
        float hitStart = pSpawnInfo.hits[0].start;
        float hitEnd = pSpawnInfo.hits[pSpawnInfo.hits.Count - 1].end;
        firstHitEnd = pSpawnInfo.hits[0].end;
        HighlightStartTime = hitStart;
        runTime = hitEnd - runStart;
        hitWindow = hitEnd - hitStart;

        //slowDownDistanceScalar = 1.5f;
        offset = 0.3f;

        // get player position
        playerPos = playerPosition;
        // adjust forward direction to face player
        transform.LookAt(playerPos);

        //defalut enemy animation
        ChangeAnimationState(Enemy_Run);
        enemy.GetComponent<Animator>().SetFloat("slowDownFactor", 1.0f);

        // differentiate combo enemies
        if(pSpawnInfo.hits.Count > 1)
        {
            transform.Find("model:geo/model:Hair_low").gameObject.SetActive(false);
            transform.Find("model:geo/model:Beard_low").gameObject.GetComponent<Renderer>().material = comboGruntBeard;
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Movement
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void FixedUpdate()
    {
        //count time since enemy Spawn
        sinceStart += Time.fixedDeltaTime;

        if (!anticipated && hitEvents.Count == 1)
        {
            //highlight first hit location red
            //Debug.Log("anticipated");
            AnticipateHit(0);
            anticipated = true;
        }

        RunMusicEvent();

        if (!reachDest)
        {
            // Enemy move to Player
            Move();
            //Enemy reach destination
            if (Vector3.Distance(transform.position.XZPlane(), destination.XZPlane()) <= attackRange)
            {
                reachDest = true;
                ChangeAnimationState(Enemy_Attack);
                GetComponent<Animator>().SetBool("run", false);

                if (hitEvents.Count > 1)
                {
                    //StartCoroutine(DelayGruntAttack());
                    currHighlight = StartCoroutine(Highlight(currentEvent, 3));
                }
                else
                {
                    //Debug.Log("In Update");
                    //GetComponent<Animator>().SetTrigger("gruntAttack");
                }
                StartCoroutine(Destry(1.0f));
            }
        }
    }

    /// <summary>
    /// Control Enemy's translation by time
    /// </summary>
    public void Move()
    {
        Debug.DrawLine(transform.position + Vector3.up * 0.3f, destination + Vector3.up * 0.3f, Color.red, Time.deltaTime);
        // Lerp speed should be constant
        // Time to reach player IS

        if (!ReachedSlowdownPhase)
        {
            transform.position = Vector3.Lerp(startPos, slowDownPos, sinceStart / (Time1));
            if (sinceStart / (Time1) >= 1.0f)
            {
                ReachedSlowdownPhase = true;
                enemy.GetComponent<Animator>().SetFloat("slowDownFactor", slowDownFactor);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(slowDownPos, destination, (sinceStart - Time1) / (Time2));
        }
    }

    

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Animation Controller
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Change Animation call which replace the animation as new state
    /// </summary>
    /// <param name="newState"></param>
    void ChangeAnimationState(string newState)
    {
        //stop the same animation from interrupting itself
        if (currentState == newState) return;

        // play the animation
        enemy.GetComponent<Animator>().Play(newState);

        //reassign the current state
        currentState = newState;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Body Hightlight
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    
    void StopCurrHighlight(string HitLocation)
    {
        if (HighlightsArray.ContainsKey(HitLocation))
        {
            StopCoroutine(HighlightsArray[HitLocation]);
        }
    }
    void StopCurrHighlight()
    {
        if (currHighlight != null) StopCoroutine(currHighlight);
    }

    void RunMusicEvent()
    {
        //disabled since check wrong hit in other place.
        /*
        if (checkEventIdx >= hitEvents.Count)
        {
            if (checkEventIdx > hitEvents.Count && Health > 0 && !IsAttacking)
            {
                //GetComponent<Animator>().SetTrigger("gruntAttack");
                IsAttacking = true;
            }
            return;
        }*/

        // //Debug.Log((float)HighlightIdx / (float)hitEvents.Count);
        // //Debug.Log(hitEvents[0].hitPerfect);
        // //Debug.Log((hitEvents[0].start * (float)((float)HighlightIdx / (float)hitEvents.Count)));
        // take the start divide by count.
        ////Debug.Log(sinceStart);
        
        //displace hightlight in sequence
        if (hitEvents.Count > 1 && HighlightIdx < hitEvents.Count && sinceStart > (hitEvents[0].start * ((float)HighlightIdx / (float)hitEvents.Count)))
        {
            HighlightsArray.Add(hitEvents[HighlightIdx].hitLocation, StartCoroutine(Highlight(hitEvents[HighlightIdx], HighlightIdx)));
            HighlightIdx++;
        }

        //AttackStartTime = hitEvents[checkEventIdx].start;
        ////Debug.Log(sinceStart-HighlightStartTime);
        if (currentEvent != null && sinceStart > currentEvent.end)
        {
            //GetComponent<Animator>().SetTrigger("gruntAttack");
            IsAttacking = true;
        }
        // if (sinceStart >= HighlightStartTime)
        // {
        //     ////Debug.Log(hitEvents.Count);
        //     if ((reachDest && hitEvents.Count > 1))
        //     {
        //         if ((hitEvents.Count-checkEventIdx) == Health)
        //         {
        //             //if (Vector3.Distance(Vector3.Scale(transform.position, zeroY), Vector3.Scale(player.transform.position, zeroY)) < attackRange)
        //             {
        //                 //currentEvent = hitEvents[checkEventIdx];
        //                 //++checkEventIdx;


        //             }
        //         }
        //     }
        // }
        // if (sinceStart >= HighlightStartTime)
        // {
        //     ////Debug.Log(hitEvents.Count);
        //     if ((reachDest && hitEvents.Count > 1)|| hitEvents.Count == 1)
        //     {
        //         if ((hitEvents.Count-checkEventIdx) == Health)
        //         {
        //             //if (Vector3.Distance(Vector3.Scale(transform.position, zeroY), Vector3.Scale(player.transform.position, zeroY)) < attackRange)
        //             {
        //                 currentEvent = hitEvents[checkEventIdx];
        //                 //++checkEventIdx;
        //                 // currHighlight = StartCoroutine(Highlight(currentEvent));
        //                 if (checkEventIdx < hitEvents.Count)
        //                 {
        //                     HighlightStartTime = hitEvents[checkEventIdx].start;

        //                 }

        //             }
        //         }
        //         else if (!IsAttacking)
        //         {
        //             //Debug.Log("In Music Event");
        //             GetComponent<Animator>().SetTrigger("gruntAttack");
        //             IsAttacking = true;
        //         }
        //     }
        // }
    }

    IEnumerator Highlight(HitEvent e, int FinalColor)
    {
        //Debug.Log(FinalColor);
        float span = 1.0f;
        float seg = span / 10;
        float time1 = sinceStart;
        float time2 = time1 + span;
        Color LerpColor = Color.red;
        if (FinalColor == 1) LerpColor = Color.yellow;
        else if (FinalColor == 2) LerpColor = Color.green;
        else if (FinalColor == 3) LerpColor = Color.blue;
        
        //float time2 = e.start + seg * 3;
        //float time3 = e.end;
        // highlight body part for textured enemies
        if (GetComponent<BoneHighlighter>() != null)
        {
            // for (float t = sinceStart; t <= time1+seg*4; t = sinceStart)
            // {
            //     //Debug.Log((t - time1) / span);
            //     Color c = Color.Lerp(Color.red, Color.yellow, Score.instance.GetColorFrac((t - time1) / span, 0.0f, 0.4f));
            //     GetComponent<BoneHighlighter>().HighlightPart(e.hitLocation, c);
            //     yield return null;
            // }
            // for (float t = sinceStart; t <= time1+seg*6; t = sinceStart)
            // {
            //     Color c = Color.Lerp(Color.yellow, Color.green, Score.instance.GetColorFrac((t - time1+seg*4) / span, 0.4f, 0.6f));
            //     GetComponent<BoneHighlighter>().HighlightPart(e.hitLocation, c);
            //     yield return null;
            // }
            for (float t = sinceStart; t <= time2; t = sinceStart)
            {
                ////Debug.Log((t - time1) / span);
                Color c = Color.Lerp(Color.red, LerpColor, Score.instance.GetColorFrac((t - time1) / span, 0.0f, 1.0f));
                //Debug.Log(e.hitLocation);
                GetComponent<BoneHighlighter>().HighlightPart(e.hitLocation, c);
                yield return null;
            }
            // GetComponent<BoneHighlighter>().HighlightPart(e.hitLocation, FinalColor, true); 
            // if gets here, player failed to hit
            //hitEvents.Clear();
            //currentEvent = null;
            //StopCurrHighlight();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Hit reaction 
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 

    /// <summary>
    /// the attack cost enemy health -1
    /// </summary>
    /// <param name="health"></param>
    /// <returns>remaining health</returns>
    public virtual int TakeDamage(int health)
    {
        health = health - 1;

        return health;
    }

    /// <summary>
    /// Enemy been attacked, but not dead. 
    /// </summary>
    /// <param name="enemy"></param>
    public void Hitted(GameObject enemy)
    {
        //play the animation depending on attack hand and  hitted position.
        if (enemyManager.PunchHandName == "RightHandAnchor") //&& flag == 0)
        {
            //enemy.transform.root.GetComponent<Animator>().SetTrigger("leftStomach");
            //flag = 1;
            ChangeAnimationState(Hitted_Right_Head);

        }
        else if (enemyManager.PunchHandName == "LeftHandAnchor")//&& flag == 1)
        {
            ChangeAnimationState(Hitted_Left_Stomach);
            //enemy.transform.root.GetComponent<Animator>().SetTrigger("rightStomach");
            //flag = 0;
        }

        
        ++checkEventIdx;
        //if (currHighlight != null)  GetComponent<BoneHighlighter>().DisableHighlight();
        if (checkEventIdx < hitEvents.Count)
        {
            currentEvent = hitEvents[checkEventIdx];
            HighlightStartTime = hitEvents[checkEventIdx].start;
            currHighlight = StartCoroutine(Highlight(currentEvent, 3));
        }
    }

    /// <summary>
    /// Enemy dead and  fly away
    /// </summary>
    /// <param name="enemy"></param>
    public void Death(GameObject enemy)
    {
        //use ragdollManager to play fly away
        bcs = enemyManager.HittedPosition.GetComponent<BodyColliderScript>();
        hitDirection = enemyManager.HitDirection;
        int[] parts = new int[] { bcs.index };
        bcs.ParentRagdollManager.startHitReaction(parts, hitDirection * 35f);

        //change rigidbody to physics based
        Rigidbody rb = this.transform.root.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        flag = 0;
    }

    public float CalculateAttack(HitType type, string part)
    {

        ////Debug.Log(Vector3.Distance(transform.position, player.transform.position));

        if (type != currentEvent.type)
        {
            // wrong hit type
            return -1;
        }
        float hitTime = sinceStart;
        float hitWindowSpan = currentEvent.end - currentEvent.start;
        float frac = (hitTime - currentEvent.start) / hitWindowSpan;
        float normalizedScore = Score.instance.NormalizeScore(frac);
        //Debug.Log("HitWindow " + currentEvent.start + "-" + hitTime + "-" + currentEvent.end + ", frac = " + frac + ", normalized score = " + normalizedScore);
        if (part == "Head_M" && currentEvent.hitLocation == "Head")
        {
            //StopCurrHighlight(currentEvent.hitLocation);
            Score.instance.Add(type, HitLocation.Head, normalizedScore);
            return normalizedScore;
        }
        else if (part == "Root_M" && currentEvent.hitLocation == "Stomach")
        {
            //StopCurrHighlight(currentEvent.hitLocation);
            Score.instance.Add(type, HitLocation.Stomach, normalizedScore);
            return normalizedScore;
        }
        else if (part == "Spine1_M" && currentEvent.hitLocation == "Chest")
        {
            //StopCurrHighlight(currentEvent.hitLocation);
            Score.instance.Add(type, HitLocation.Chest, normalizedScore);
            return normalizedScore;
        }


        return 0;
    }

    public void Attacked()
    {
        enemyManager = GetComponent<EnemyManager>();
        //Debug.Log("current Hit Location   " + enemyManager.HittedPart + "\ncurrent need  to be hiitted locatiioon  " + currentEvent.hitLocation);

        // if health = 1, hit the right position --> death
        // if health = 1, hit the wrong position --> play hitted animation --> attack player
        // if health > 1, hit the right position --> play hitted animation --> run toward player
        // if health > 1, hit the wrong position --> player hitted animation --> attack player
        if (Health == 1)
        {
            if (CalculateAttack(HitType.Punch, enemyManager.HittedPart) > 0)
            {
                //correct hit an enemy with 1 health
                Death(this.gameObject);
                //Debug.Log("Hit result : enemy death");
            }
            else
            {
                //worng hit an enemy with 1 health
                Hitted(this.gameObject);
                //stop highlight
                StopCurrHighlight(currentEvent.hitLocation);
                //attack player
                //Debug.Log("Hit result : wrong hit");
            }
        }
        else
        {
            if (CalculateAttack(HitType.Punch, enemyManager.HittedPart) > 0)
            {
                //correct hit an enemy with multi-health
                Hitted(this.gameObject);
                // health -- 
                Health = TakeDamage(Health);
                //keep running and play next highlight

                //Debug.Log("Hit result : enemy -1 health");
            }
            else
            {
                //worng hit an enemy with 1 health
                Hitted(this.gameObject);
                //stop highlight
                StopCurrHighlight(currentEvent.hitLocation);
                //attack player
                //Debug.Log("Hit result : wrong hit");
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Other  function
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public void Destroy()
    {
        // //Debug.Log("Destroying " + sinceStart + " vs " + hitEvents[hitEvents.Count - 1].end);
        Destroy(enemy);
    }

    IEnumerator Destry(float delay = 1.0f)
    {
        //When palyer in attacking range, hit the player
        //enemy.GetComponent<Animator>().SetTrigger("attack");
        enemy.GetComponent<Animator>().SetBool("run", false);

        // transform.Find("Enemy_low").GetComponent<SkinnedMeshRenderer>().material.color = Color.red;

        //yield return new WaitForSeconds(3.5f);
        //if(attacked)
        //    yield return new WaitForSeconds(2f);


        //StopCurrHighlight();

        // if(hitEvents.Count > 0)
        // {
        //     yield return new WaitForSeconds(delay * AttackDelay * (hitEvents[hitEvents.Count - 1].end - hitEvents[hitEvents.Count - 1].hitPerfect));
        // }
        // else
        {
            yield return new WaitForSeconds(delay * AttackDelay);
        }
        
        // if(attacked)
        // {
        //     Destroy();
        // }
        // else
        // if (!attacked)
        // {
        //     enemy.GetComponent<Animator>().SetTrigger("taunt2");
        // }
    }

    public void GotAttacked(float delay = 1.0f)
    {
        reachDest = true;
        // EnemyAudioPlayer.Play();
        // StartCoroutine(Destry(0.1f));
    }

    

    public void CalculateBlock()
    {
        if (currentEvent.type != HitType.Block)
        {
            // wrong hit type
            return;
        }
        float hitTime = sinceStart;
        float hitWindowSpan = currentEvent.end - currentEvent.start;
        float diffPerfect = hitTime - currentEvent.hitPerfect;
        float frac = Mathf.Lerp(1, 0.1f, Mathf.Abs(diffPerfect) / hitWindowSpan);

        Score.instance.Add(HitType.Block, HitLocation.Default, frac);
    }

    void AnticipateHit(int idx = 0)
    {
        Color c = Color.red;
        GetComponent<BoneHighlighter>().HighlightPart(hitEvents[idx].hitLocation, c, true);
    }

   
    /*
    public void OnTriggerEnter(Collider col)
    {
        ////Debug.Log(col.gameObject.name);
        if (col.gameObject.name.Contains("Enemy") || col.gameObject.name.Contains("Grunt"))
        {
            ////Debug.Log(reachDest);
            ////Debug.Log(Vector3.Equals(startPos, col.gameObject.GetComponent<Grunt>().startPos));
            if (reachDest && SpawnIdx == col.gameObject.GetComponent<Grunt>().SpawnIdx)
            {
                //Destroy(this.gameObject);//.GetComponent<Enemy2>().Kill();
            }
        }
    }
    */

}
