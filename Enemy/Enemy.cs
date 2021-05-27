using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MLSpace;
using System.ComponentModel;
using Oculus.Platform;
using System;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public GameObject enemy => this.transform.gameObject;

    public GameObject[] BodyParts;

    public Transform ScorePos;

    public float attackRange;
    [HideInInspector]
    protected float attackTime;
    [HideInInspector]
    public Vector3 playerPos;
    public bool attacked;
    [HideInInspector]
    public BodyColliderScript bcs;
    [HideInInspector]
    public List<HitEvent> hitEvents;

    public EnemyType thisEnemyType;

    private HitEvent currentEvent;

    public AudioClip[] SpawnSounds;
    public AudioClip RunSound;
    private Vector3 startPos;
    [HideInInspector]
    public Vector3 slowDownPos;
    [HideInInspector]
    public Vector3 destination;
    private string flyDestination;
    private Vector3 hitDirection;
    private float offset;
    private float runTime;
    protected float hitWindow;
    private float runStart;
    private float sinceStart;
    public float AttackDelay = 1.05f;
    public bool reachDest = false;
    public bool slowDown;
    protected bool isTutorial;
    protected bool[] tutorialHitted;
    protected int tutorialOffWindowHitCount;

    // variable moved to EnemySpawnHnadler
    public float SpawnDistance;
    public float slowDownFactor;
    // variable above all moved to EnemySpawnHnadler

    protected float firstHitEnd;



    protected float HighlightStartTime = 0.0f;
    protected float AttackStartTime = 0.0f;


    [HideInInspector]
    public bool IsAttacking = false;
    public int Health;

    private int ProperHits = 0;
    private int checkEventIdx;
    static int flag = 0;

    private bool ReachedSlowdownPhase = false;
    private Dictionary<string, Coroutine> HighlightsArray = new Dictionary<string, Coroutine>();

    int SpawnIdx = 0;

    public GameObject DustEffect;

    public bool TrailerMode = false;

    private float normalRunningTime;
    private float hitWindowTime;
    private float spawnTime;
    private float slowDownDistanceScalar;

    private bool RunningAway = false;

    private float RunTimer = 0.0f;

    public const int MaxHairChoices = 3;

    private Vector3 RunAwayDirection;

    private float GameTime;

    private TutorialManager m_TutManager = null;

    private BoneHighlighter EnemyBoneHighlighter;

    private Color[] colorArray = { new Color(0.16862745098f, 0.8f, 0.45098039215f), new Color(0.76862745098f, 0.31764705882f, 0.5725490196f), Color.yellow, new Color(0.95f, 0.42f, 0.06f), Color.red };    //white,green,yellow,orange,red
    private bool anticipated = false;
    public int currentEventIdx = 0;
    private float currentHLstartTime = 0.0f;
    private float nextHLstartTime = 0.0f;
    private Color RedColor = new Color(1.0f, 0, 0.0f); //new Color(0.76862745098f, 0.31764705882f, 0.5725490196f);
    //  new Color(1.0f, 0.41960784313f, 0.69411764705f);
    //private Color lightGreenColor = new Color(0.62f,1.0f,0.62f);
    private Color lightGreenColor = new Color(0.16862745098f, 0.8f, 0.45098039215f);

    public float HighlightLengthSeconds = 0.5f;

    private bool JustHit = false;

    GameObject FlyDestObj;

    public bool IsDead = false;

    GameObject hurtSphere;
    Renderer hurtSpehereRenderer;

    public bool IsFake = false;
    Color hurtColor;
    bool decreaseAlpha = false;
    bool increaseAlpha = false;

    public Transform PlayerTransform;

    public Material[] EnemyShirtColors;

    public Material[] EnemyHairColors;

    public Material[] EnemyPantColors;

    Animator animator;



    [SerializeField]
    private Material comboGruntBeard;
    /// <summary>
    /// set up variable before generating the enemy 
    /// </summary>
    /// <param name="pStartPos"> enemy position</param>
    /// <param name="pSpawnInfo"> EnemySpawnInformation</param>
    /// <param name="playerPosition">player position/destination, a fixed Vectors when game started</param>
    public void SetParameter(EnemySpawn pSpawnInfo, Vector3 playerPosition, Vector3 PlayerForward, 
        float normalrunningTime, float hitwindowTime, float slowdownFactor, float TimeSinceSpawnStart, float SlowDownDistanceScalar, bool isTrailer = false)
    {
        TrailerMode = isTrailer;
        isTutorial = pSpawnInfo.isTutorial;
        startPos = this.transform.position;
        SpawnIdx = pSpawnInfo.locationIdx;
        runStart = pSpawnInfo.runStart;
        hitEvents = pSpawnInfo.hits;
        normalRunningTime = normalrunningTime;
        hitWindowTime = hitwindowTime;
        spawnTime = pSpawnInfo.spawnTime;
        slowDownFactor = slowdownFactor;
        GameTime = TimeSinceSpawnStart;
        slowDownDistanceScalar = SlowDownDistanceScalar;
        thisEnemyType = pSpawnInfo.type;


        if (thisEnemyType == EnemyType.Grunt)
        {

            Transform HairOptions = transform.Find("model:geo/Hair");
            if (HairOptions != null)
            {
                List<int> HairChoices = new List<int>();
                int HairColor = UnityEngine.Random.Range(0, EnemyHairColors.Length);

                for (int i = 0; i < MaxHairChoices; i++)
                {
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        int HairChoice = UnityEngine.Random.Range(0, HairOptions.childCount);
                        if (!HairChoices.Contains(HairChoice))
                        {
                            HairChoices.Add(HairChoice);
                        }
                    }
                }

                int HairCount = 0;
                foreach (Transform HairOpt in HairOptions)
                {
                    if (HairChoices.Contains(HairCount))
                    {
                        HairOpt.gameObject.SetActive(false);
                        HairOpt.GetComponent<Renderer>().material = EnemyHairColors[HairColor];
                    }
                    
                    HairCount += 1;
                }
            }

            Transform ShirtOpt = transform.Find("model:geo/model:Pants_low");
            int PantColor = UnityEngine.Random.Range(0, EnemyPantColors.Length);
            ShirtOpt.GetComponent<Renderer>().material = EnemyPantColors[PantColor];

            Transform ShirtOption = transform.Find("model:geo/model:Shirt2_low");
            Transform DefaultShirt = transform.Find("model:geo/model:Shirt_low");
            

            if (UnityEngine.Random.value < 0.5f)
            {
                ShirtOption.gameObject.SetActive(true);
                DefaultShirt.gameObject.SetActive(false);
                GetComponent<BoneHighlighter>().SetBodyRenderer(ShirtOption.GetComponent<Renderer>());
            }
            else
            {
                int ShirtColor = UnityEngine.Random.Range(0, EnemyShirtColors.Length);
                DefaultShirt.gameObject.SetActive(true);
                ShirtOption.gameObject.SetActive(false);
                GetComponent<BoneHighlighter>().SetBodyRenderer(DefaultShirt.GetComponent<Renderer>());
                DefaultShirt.GetComponent<Renderer>().material = EnemyShirtColors[ShirtColor];
            }
        }

        if (hitEvents.Count > 1)
        {
            nextHLstartTime = hitEvents[1].start - spawnTime;
        }
        
        if(pSpawnInfo.flyTarget != null  && pSpawnInfo.flyTarget.Length > 0 && pSpawnInfo.flyTarget != "0")
        {
            flyDestination = pSpawnInfo.flyTarget;
        }
        else
        {
            flyDestination = "";
        }

        // retrieve information from hit event including first hit start, last hit end...
        currentEvent = hitEvents[0];
        Health = hitEvents.Count;
        float hitStart = pSpawnInfo.hits[0].start;
        float hitEnd = pSpawnInfo.hits[pSpawnInfo.hits.Count - 1].end;
        firstHitEnd = pSpawnInfo.hits[0].end;
        HighlightStartTime = hitStart;
        runTime = hitEnd - runStart;
        hitWindow = hitEnd - hitStart;
        tutorialHitted = new bool[hitEvents.Count];

        //slowDownDistanceScalar = 1.5f;
        offset = 0.3f;

        // get player position
        playerPos = playerPosition;
        // adjust forward direction to face player
        transform.LookAt(playerPos);

        //defalut enemy animation
        //ChangeAnimationState(Enemy_Run);
        enemy.GetComponent<Animator>().SetBool("run", true);
        enemy.GetComponent<Animator>().SetFloat("slowDownFactor", 1.0f);


        Vector3 towardsPlayer = playerPos - transform.position;
        Vector3 AwayFromPlayer = transform.position - playerPos;
        float AngleToPlayer = Vector3.Angle(PlayerForward, AwayFromPlayer);
        ////Debug.Log(Mathf.Abs(AngleToPlayer));
        if (Mathf.Abs(AngleToPlayer) > 30.0)
        {
            GetComponent<AudioSource>().clip = SpawnSounds[UnityEngine.Random.Range(0, SpawnSounds.Length)];
            GetComponent<AudioSource>().Play();
        }

        destination = transform.position + Vector3.Normalize(towardsPlayer) * (Vector3.Magnitude(towardsPlayer) - attackRange);
        destination.y = transform.position.y;
        slowDownPos = destination - (Vector3.Normalize(towardsPlayer) * slowDownDistanceScalar * Vector3.Magnitude(towardsPlayer));
    }

    public void Start()
    {
        if (GameObject.Find("TutorialManager"))
            m_TutManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        EnemyBoneHighlighter = GetComponent<BoneHighlighter>();
        hurtSphere = GameObject.Find("HurtSphere");
        hurtSpehereRenderer = hurtSphere.GetComponent<Renderer>();
        hurtColor = hurtSpehereRenderer.material.color;

        animator = GetComponent<Animator>();
        if (IsFake)
        {

            hitEvents = new List<HitEvent>();
            HitEvent hitEvent1 = new HitEvent();

            hitEvent1.hitLocation = "Head";
            hitEvent1.type = HitType.Punch;
            hitEvent1.start = 6.0f;
            hitEvent1.hitPerfect = 9f;
            hitEvent1.end = 10f;
            hitEvents.Add(hitEvent1);

            HitEvent hitEvent2 = new HitEvent();
            hitEvent2.hitLocation = "Chest";
            hitEvent2.type = HitType.Punch;
            hitEvent2.start = 8f;
            hitEvent2.hitPerfect = 11f;
            hitEvent2.end = 12f;
            hitEvents.Add(hitEvent2);

            spawnTime = 0f;
            runStart = 0f;

            isTutorial = true;

            Vector3 SpawnPosition = PlayerTransform.position + 9.0f * (Quaternion.AngleAxis(0, Vector3.up) * Vector3.right);
            SpawnPosition.y = 0.4f;
            transform.position = SpawnPosition;
            //transform.LookAt(PlayerTransform.position);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Movement
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    
    public void FixedUpdate()
    {
        //count time since enemy Spawn
        sinceStart += Time.fixedDeltaTime;
        if (hurtColor.a >= 0.3f)
        {
            decreaseAlpha = true;
            increaseAlpha = false;
        }
        if (increaseAlpha)
        {
            hurtColor.a += Time.deltaTime;
            hurtSpehereRenderer.material.color = hurtColor;
        }
        if (decreaseAlpha && hurtColor.a>0f)
        {
            hurtColor.a -= Time.deltaTime;
            hurtSpehereRenderer.material.color = hurtColor;
        }


        if (!IsDead && IsAttacking && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("GruntAttack") && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f)
        {
            //if (GameObject.Find("HurtEffect") != null)
            //{
            //    foreach (Transform child in GameObject.Find("HurtEffect").transform)
            //    {
            //        child.gameObject.SetActive(true);
            //    }
            //}
            increaseAlpha = true;
            decreaseAlpha = false;
            

        }

        if (!IsDead && !TrailerMode)
        {
            HighlightEvent();
        }

        if (reachDest && ((isTutorial && sinceStart > hitEvents[hitEvents.Count-1].end) || (animator.GetCurrentAnimatorStateInfo(0).IsName("GruntAttack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) || (animator.GetBool("run") && animator.GetCurrentAnimatorStateInfo(0).IsName("Run")))) // check if "bash" is playing...
        {
            Score.instance.ResetScoreStreak();

            if (!RunningAway)
            {
                // GetComponent<AudioSource>().clip = RunSound;
                // GetComponent<AudioSource>().loop = true;
                // GetComponent<AudioSource>().Play();

                //GetComponent<Animator>().ResetTrigger("gruntAttack");
                animator.SetBool("run", true);
                animator.SetFloat("slowDownFactor", 1.0f);
                 
                if (SpawnIdx == 1) 
                {
                    RunAwayDirection = -transform.right;
                    transform.Rotate(Vector3.up, -90);
                }
                else if (SpawnIdx == 2) 
                {
                    RunAwayDirection = (Quaternion.AngleAxis(-45, Vector3.up) * transform.forward);
                }
                else if (SpawnIdx == 3)
                {
                    RunAwayDirection = Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
                    transform.Rotate(Vector3.up, 90);
                    
                }
                
                RunAwayDirection.Normalize();
                RunAwayDirection = transform.position + 20*RunAwayDirection;
                RunningAway = true;
            }
            ////Debug.Log("Run away");
            RunAway();
        }

        if (!reachDest && !IsDead && !IsFake) // Sometimes player will die before reaching dest
        {
            // Enemy move to Player
            Move();
            // if (!GetComponent<AudioSource>().isPlaying)
            // {
            //     GetComponent<AudioSource>().clip = RunSound;
            //     GetComponent<AudioSource>().loop = true;
            //     GetComponent<AudioSource>().Play();
            // }
            
            //Enemy reach destination
            if (Vector3.Distance(transform.position.XZPlane(), destination.XZPlane()) <= attackRange)
            {
                reachDest = true;
                //ChangeAnimationState(Enemy_Attack);
                GetComponent<AudioSource>().loop = false;

                animator.SetBool("run", false);
                if (hitEvents.Count == 1)
                {
                    float DelaySeconds = (hitEvents[0].start + hitEvents[0].hitPerfect)/2 - sinceStart - spawnTime;
                    ////Debug.Log(DelaySeconds);
                    if (DelaySeconds < 0) DelaySeconds = 0;
                    StartCoroutine(DelayGruntAttack(DelaySeconds));
                }
            //StartCoroutine(Destry(1.0f));
            }
        }

    }

    IEnumerator DelayGruntAttack(float sec)
    {
        if (!isTutorial)
        {
            yield return new WaitForSeconds(sec);
            animator.SetTrigger("gruntAttack");
            IsAttacking = true;
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
        if (!slowDown)
        {
            transform.position = Vector3.Lerp(startPos, destination, sinceStart / normalRunningTime);
        }
        else
        {
            if (!ReachedSlowdownPhase)
            {
                transform.position = Vector3.Lerp(startPos, slowDownPos, sinceStart / normalRunningTime);
                if (sinceStart / normalRunningTime >= 1.0f)
                {
                    ReachedSlowdownPhase = true;
                    enemy.GetComponent<Animator>().SetFloat("slowDownFactor", slowDownFactor);
                }
            }
            else if ((sinceStart - normalRunningTime) / hitWindowTime <= 1.0f)
            {
                transform.position = Vector3.Lerp(slowDownPos, destination, (sinceStart - normalRunningTime) / hitWindowTime);
                ////Debug.Log("Still moving");
            }
        }
    }

    public void IncrementProperHit()
    {
        ProperHits++;
        
    }

    public int GetProperHits()
    {
        return ++ProperHits;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Body Hightlight
    ///////////////////////////////////////////////////////////////////////////////////////////////////////



    /// <summary>
    /// Call Hightlight Event for each fixed UPdate
    /// </summary>
    void HighlightEvent()
    {
        if (!anticipated)
        {
            // initial the highlight
            InitialHighlight();
            anticipated = true;
        }

        else if (currentEventIdx < hitEvents.Count)
        {
            // the next  hitEvets's  position is  learping color
            LearpingHightLight(hitEvents[currentEventIdx], currentHLstartTime);
            if (nextHLstartTime > 0.0f && currentEventIdx < hitEvents.Count - 1)
            {
                LearpingHightLight(hitEvents[currentEventIdx+1], nextHLstartTime);
            }
            // learping finish, new location is open to  be hit
            if ((sinceStart > hitEvents[currentEventIdx].end - spawnTime) || JustHit)
            {
                if(JustHit)
                {
                    tutorialHitted[currentEventIdx] = true;
                    tutorialOffWindowHitCount -= 1;
                }
                JustHit = false;

                //stop old highlighting
                StopCurrHighlight(hitEvents[currentEventIdx].hitLocation);
                //switch  to new event
                if (nextHLstartTime == 0.0f)
                {
                    currentHLstartTime = sinceStart;
                }
                else
                {
                    // If we are on third combo, then currentHLStartTime is when first combo finished.
                    currentHLstartTime = nextHLstartTime;
                }
                if (currentEventIdx < hitEvents.Count - 1)
                {
                    nextHLstartTime = hitEvents[currentEventIdx+1].hitPerfect - spawnTime; // This little offset is so that it appears dark green at start of highlight 
                }
                else
                {
                    nextHLstartTime = 0.0f;
                }
                currentEventIdx++;
                if (currentEventIdx < hitEvents.Count)
                {
                    currentEvent = hitEvents[currentEventIdx];
                }
                else
                {

                }
            }
        }

        // in tutorial mode, continue highlighting the parts if player fails to hit
        if (isTutorial && !IsFake)
        {
            int offWindowCount = 0;
            for(int i= 0; i< currentEventIdx; ++i)
            {
                if(tutorialHitted[i] != true && sinceStart > hitEvents[i].end - spawnTime)
                {
                    offWindowCount += 1;
                    if(offWindowCount > tutorialOffWindowHitCount)
                    {
                        UnconditionalHighlight(hitEvents[i]);
                    }
                    else
                    {
                        StopCurrHighlight(hitEvents[i].hitLocation);
                    }
                }
            }
        }

        //stop  last highlighting when hitWindow closed
        if(hitEvents.Count > 1 && sinceStart > hitEvents[hitEvents.Count - 1].end - spawnTime)
        {
            //StopCurrHighlight(hitEvents[hitEvents.Count - 1].hitLocation);
            if(!isTutorial)
            {
                GetComponent<Animator>().SetTrigger("gruntAttack");
                IsAttacking = true;
            }
        }


    }


    public void StopCurrHighlight(string HitLocation)
    {
        EnemyBoneHighlighter.DisableHighlight(HitLocation);

    }

    /// <summary>
    /// Hightlight all three hitlocation to light green
    /// </summary>
    void InitialHighlight()
    {
        int NumHitEvents = 0;
        foreach (HitEvent hitEvent in hitEvents)
        {
            if (NumHitEvents < 2)
            {
                EnemyBoneHighlighter.HighlightPart(hitEvent.hitLocation, RedColor, true);
                NumHitEvents++;
            }
            
        }

    }
    /// <summary>
    /// According to hitEvent's idex, learping the next body part from lightgreen to green
    /// </summary>
    void LearpingHightLight(HitEvent nextHitEvent, float HLStartTime)
    {
        Color LearpedColor = Color.Lerp(RedColor, lightGreenColor, (sinceStart-HLStartTime)/ (nextHitEvent.hitPerfect - spawnTime- HLStartTime));
        EnemyBoneHighlighter.HighlightPart(nextHitEvent.hitLocation, LearpedColor, true);
    }

    void UnconditionalHighlight(HitEvent e)
    {
        EnemyBoneHighlighter.HighlightPart(e.hitLocation, Color.gray, true);
    }


    void HighlightGreen()
    {
        SwitchHighlights(currentEventIdx, 1);
    }
    public void GreenHightEnd()
    {
        currentEventIdx++;
    }

    /// <summary>
    /// Switch first three highlights
    /// </summary>
    /// <param name="eventIndex"> the most recent hitted event  index</param>
    /// <param name="colorIndex"> the  first order of color</param>
    void SwitchHighlights(int eventIndex, int colorIndex)
    {
        while (eventIndex < hitEvents.Count)
        {
            SwitchHighlight(eventIndex++, colorIndex++);
        }
    }

    /// <summary>
    /// Switch boneHighligh at certain hitlocation to certain color
    /// </summary>
    /// <param name="eventIndex"></param>
    /// <param name="colorIndex"></param>
    void SwitchHighlight(int eventIndex, int colorIndex)
    {
        if (eventIndex >= 0 && eventIndex < hitEvents.Count)
        {
            EnemyBoneHighlighter.HighlightPart(hitEvents[eventIndex].hitLocation, colorArray[colorIndex],true);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Enemy Hit reaction 
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 

 
    /// <summary>
    /// Enemy been attacked, but not dead. 
    /// </summary>
    /// <param name="enemy"></param>
    public void Hitted(GameObject enemy)
    {
        JustHit = true;
        if(isTutorial)
        {
            tutorialOffWindowHitCount += 1;
        }

        if (!TrailerMode)
            Score.instance.MoveScoreText(ScorePos.position, transform.rotation);

    }

    private IEnumerator Despawn(float seconds = 12.0f)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }

    private IEnumerator StayAlive(float seconds = 12.0f)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(GetComponent<EnemyManager>());
        Destroy(GetComponent<BoneHighlighter>());
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<RagdollManagerHum>());
        Destroy(GetComponent<ragdoll>());
        Destroy(this);
    }


    /// <summary>
    /// Enemy dead and  fly away
    /// </summary>
    /// <param name="enemy"></param>
    public void Death(GameObject enemy, EnemyManager enemyManager)
    {
        IsDead = true;
        if (currentEventIdx >= hitEvents.Count)
        {
            currentEventIdx = hitEvents.Count - 1;
        }
        foreach(HitEvent he in hitEvents)
        {
            StopCurrHighlight(he.hitLocation);
        }
        // StopCurrHighlight(hitEvents[currentEventIdx].hitLocation);
        if (!TrailerMode)
            Score.instance.MoveScoreText(ScorePos.position, transform.rotation);
        
        if (flyDestination.Length == 0 || GameObject.Find("FlyDestination/" + flyDestination) == null)
        {
            //use ragdollManager to play fly away
            try
            {
                bcs = enemyManager.HittedPosition.GetComponent<BodyColliderScript>();

            }
            catch (Exception e)
            {
                //Debug.Log(enemyManager.HittedPosition);
            }
            hitDirection = enemyManager.HitDirection;
            int[] parts = new int[] { bcs.index };
            //if (hitDirection.y < 0) hitDirection.y *= -1;
            //hitDirection.y += 0.1f;
            GetComponent<ragdoll>().hit(hitDirection, enemyManager.HitForce);
            //bcs.ParentRagdollManager.startHitReaction(parts, hitDirection * 35f * enemyManager.HitForce);

            

            //change rigidbody to physics based
            Rigidbody rb = this.transform.root.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            CapsuleCollider collider = this.transform.root.GetComponent<CapsuleCollider>();
            collider.isTrigger = false;

            flag = 0;

            if (enemyManager.NormScore < 0.3f)
            {
                if (isTutorial)
                {
                    if(Punch1.missHitIndex % Punch1.TutorialCalloutSpan == 0)
                    {
                        m_TutManager.DelayEnemySpawn(4f);
                        StartCoroutine(Despawn(4f));
                    }
                    else
                    {
                        m_TutManager.DelayEnemySpawn(2.0f);
                        StartCoroutine(Despawn(2.0f));
                    }
                    
                }
                else
                {
                    StartCoroutine(Despawn());
                }
            }
            else if (enemyManager.NormScore < 0.8f)
            {
                if (isTutorial)
                {
                    m_TutManager.DelayEnemySpawn(2.0f);
                    StartCoroutine(Despawn(2.0f));
                }
                else
                {
                    StartCoroutine(Despawn());
                }
                
            }
            
            else
            {
                if (isTutorial)
                {
                     m_TutManager.DelayEnemySpawn(0.5f);
                     StartCoroutine(Despawn(2.0f));
                }
                else
                {
                    StartCoroutine(StayAlive());
                }   
                
            }

        }
        else
        { 
            try
            {
                bcs = enemyManager.HittedPosition.GetComponent<BodyColliderScript>();

            }
            catch (Exception e)
            {
                //Debug.Log(enemyManager.HittedPosition);
            }
            //change rigidbody to physics based
            Rigidbody rb = this.transform.root.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            bcs.ParentRagdollManager.gameObject.GetComponent<ragdoll>().grab();
            FlyDestObj = GameObject.Find("FlyDestination/" + flyDestination);
            FlyTowardCurve(FlyDestObj);
        }
        StartCoroutine(waitBeforeTurningOnKinematics());
 

        
    }

    IEnumerator waitBeforeTurningOnKinematics()
    {
        yield return new WaitForSeconds(7.0f);
        transform.GetComponent<Rigidbody>().isKinematic = true;
        ragdoll ragdoll = transform.GetComponent<ragdoll>();
        foreach (var bodyPart in ragdoll.ragdollList)
        {
            bodyPart.isStatic = true;
            var bodyRB = bodyPart.GetComponent<Rigidbody>();
            bodyRB.isKinematic = true;
            bodyRB.useGravity = false;

        }
        var model = this.transform.GetChild(1);
        if (model != null)
        {
            foreach (Transform child in model)
            {
                child.gameObject.isStatic = true;
                if (child.GetComponent<SkinnedMeshRenderer>() != null)
                    child.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            }
        }
        
    }

    void FlyTowardCurve(GameObject dest)
    {

        this.GetComponent<ParabolaController>().Target = dest;
        this.GetComponent<ParabolaController>().StartMove = true;

    }

    public float CalculateAttack(HitType type, string part)
    {

        bool IsCombo = currentEventIdx < (hitEvents.Count-1);

        float hitTime = Time.time - GameTime;
        float hitWindowSpan, frac;

        HitEvent AttackEvent = currentEvent;

        if (type == currentEvent.type)
        {
            AttackEvent = currentEvent;
            
        }
        else if (IsCombo && hitEvents[currentEventIdx + 1].type == type)
        {
            AttackEvent = hitEvents[currentEventIdx + 1];

        }
        else return -1;

        bool IsEarly = true;

        hitWindowSpan = (AttackEvent.hitPerfect - AttackEvent.start);
        if (hitTime > AttackEvent.hitPerfect)
        {
            IsEarly = false;
            hitWindowSpan = AttackEvent.end - AttackEvent.hitPerfect;
        }

        //Debug.Log("Hit time: " + hitTime + ", Hit window: " + AttackEvent.start + ", " + AttackEvent.hitPerfect + ", " + AttackEvent.end);
        frac = Mathf.Abs(hitTime - AttackEvent.hitPerfect) / hitWindowSpan;

        // If you hit way too late, score is 0
        if (frac > 1.0f)
        {
            frac = 0.0f;
        }
        // If you hit way too early, your score is 0
        else
        {
            
            if (hitTime < AttackEvent.hitPerfect && currentEventIdx > 0)
            {
                // Penalize hitting early less on a combo hit
                frac = 0.0f;
            }

            frac = 1.0f - frac;
        }


        float normalizedScore = Score.instance.NormalizeScore(frac);

        //Debug.Log(normalizedScore);

        if (normalizedScore < 0.3f)
        {
            if (IsEarly)
            {
                Score.instance.MainScoreText.SetText("Too early!");
            }
            else
            {
                Score.instance.MainScoreText.SetText("Too late!");
            }
            
        }

        //Debug.Log(currentEvent.hitLocation);
        //Debug.Log(part);


        if (currentEvent.hitLocation == "Head")
        {
            //StopCurrHighlight(currentEvent.hitLocation);
            
            ScorePos = BodyParts[0].transform;

            if ((part=="Head_M" || part == "mixamorig:Head"))
            {
                Score.instance.Add(type, HitLocation.Head, normalizedScore);
                return normalizedScore;
            }
            Score.instance.MainScoreText.SetText("Wrong part!");
            
        }
        else if (currentEvent.hitLocation == "Chest")
        {
            //StopCurrHighlight(currentEvent.hitLocation);
           
            ScorePos = BodyParts[1].transform;
            if ((part == "Chest" || part == "mixamorig:Spine1"))
            {
                Score.instance.Add(type, HitLocation.Chest, normalizedScore);
                return normalizedScore;
            }
            Score.instance.MainScoreText.SetText("Wrong part!");
            
        }
        else if (currentEvent.hitLocation == "Stomach")
        {
            //StopCurrHighlight(currentEvent.hitLocation);
            
            ScorePos = BodyParts[2].transform;
            if (part == "Stomach")
            {
                Score.instance.Add(type, HitLocation.Stomach, normalizedScore);
                return normalizedScore;
            }
            Score.instance.MainScoreText.SetText("Wrong part!");
            
        }

        return 0;
    }

   

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Other  function
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public void Destroy()
    {
        Destroy(enemy);
    }



    void RunAway()
    {
        //Score.instance.MoveScoreText(ScorePos.position, transform.rotation);

        if (RunTimer < 2.0f && Vector3.Distance(transform.position, RunAwayDirection) > 2.0f)
        {
            //if (RunTimer > 1.0f && GameObject.Find("HurtEffect") != null) 
            //{
            //    foreach (Transform child in GameObject.Find("HurtEffect").transform)
            //    {
            //        child.gameObject.SetActive(false);
            //    }
            //}
            RunTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, RunAwayDirection, 1-Mathf.Pow(1.2f, -Time.deltaTime * 1f));
        }
       
        else if (RunTimer > 2.0f) 
        {
            if(!IsDead && isTutorial && m_TutManager != null)
            {
                //m_TutManager.nextEnemy = true;
                m_TutManager.DelayEnemySpawn(3.0f);
                // tutorial: missed enemy
                m_TutManager.PlaySound(1);
            }
            Destroy();
        }
    }

    
    public void OnTriggerEnter(Collider col)
    {
        ////Debug.Log(col.gameObject.name);
        if (col.gameObject.name.Contains("Enemy") || col.gameObject.name.Contains("Grunt"))
        {
            ////Debug.Log(reachDest);
            ////Debug.Log(Vector3.Equals(startPos, col.gameObject.GetComponent<Grunt>().startPos));
            // if (reachDest && SpawnIdx == col.gameObject.GetComponent<Grunt>().SpawnIdx)
            // {
            //     // Destroy(this.gameObject);//.GetComponent<Enemy2>().Kill();
            // }
        }
        if (col.transform.CompareTag("interactable"))
        {
            col.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    

}
