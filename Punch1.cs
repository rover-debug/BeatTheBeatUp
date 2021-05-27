using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Punch1 : MonoBehaviour
{
    private const int MaxAlignmentScore = 20;
    private const float AlignmentThreshold = 0.5f;

    private const int FramesPrior = 4;

    private const float ForceThreshold = 0.15f;

    private const float VelocityCutoff = 1.5f;
    public float HitForce;
    public AudioClip[] BackgroundClips;
    public AudioClip[] PunchPerfectSounds;
    public AudioClip[] PunchNormalSounds;
    public AudioClip[] PunchMissSounds;

    public AudioSource punchIndicator;
    public ParticleSystem[] PunchEffects;
    public MusicAdjuster MusicCutoffFilter;
    public OVRInput.Controller RTouchController = OVRInput.Controller.RTouch;
    public OVRInput.Controller LTouchController = OVRInput.Controller.LTouch;

    [HideInInspector]
    private int LeftAlignmentScore = 0;
    private int RightAlignmentScore = 0;

    private Vector3 PrevLVelocity = Vector3.zero;
    private Vector3 PrevRVelocity = Vector3.zero;

    // public Material NormHand;
    // public Material FireHand;

    public GameObject LNormHand;
    public GameObject LFireHand;
    public GameObject RNormHand;
    public GameObject RFireHand;

    private float PrevLVelo = 0.0f;
    private float PrevRVelo = 0.0f;
    private EnemyManager enemyManager;

    private float waitTimeVal = 1.0f;

    private float TimeSinceLastTrigger = 0.0f;

    private const float TimeWaitForClear = 0.5f;
    private float waitTime = 0;
    private string hitLocation;
    private static Grunt g;
    private static Animator animator;

    //public GameObject HurtEffect;

    private TutorialManager m_TutManager;
    static int hitSoundIndex = 0;
    public static int missHitIndex;

    public bool IsMainMenu = false;



    public static int TutorialCalloutSpan = 3;

    public enum Hand // your custom enumeration
    {
        Left,
        Right
    };

    public Hand WhichHand;

    private float NormalPunchVolume = 1.0f;
    private float PerfectPunchVolume = 0.85f;

    public void Start()
    {
        waitTime = waitTimeVal;
        if (GameObject.Find("TutorialManager"))
        {
            m_TutManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        }
        
        InvokeRepeating("CheckFireHand", 10.0f, 2.0f);
        //StartCoroutine(FadeToFire());
        
    }

    private float Clamp(float val, float min, float max)
    {
        if (val < min) val = min;
        if (val > max) val = max;
        return val;
    }
    public void Attacked(Collider collider, int PunchForce)
    {
        g = collider.transform.root.GetComponent<Grunt>();

        

        if (g.IsDead) return;

        if (collider.transform.name == "Head_M" || collider.transform.name == "mixamorig:Head")
        {
            hitLocation = "Head";
        }
        else if (collider.transform.name == "Stomach")
        {
            hitLocation = "Stomach";
            
        }
        else if (collider.transform.name == "Chest" || collider.transform.name == "mixamorig:Spine1")
        {
            hitLocation = "Chest";
        }

        //if (g.currentEventIdx > 0 && g.hitEvents[g.currentEventIdx-1].hitLocation == hitLocation) return;

        enemyManager = collider.transform.root.GetComponent<EnemyManager>();

        enemyManager.PunchHandName = transform.parent.name;
        enemyManager.HittedPosition = collider.transform;
        enemyManager.HitDirection = transform.forward;
        enemyManager.HittedPart = collider.transform.name;

        if (hitLocation == "Stomach" || hitLocation == "Chest")
        {
            enemyManager.HittedPosition = collider.transform.root.GetComponent<MLSpace.RagdollManagerHum>().RagdollBones[(int)(MLSpace.BodyParts.Chest)];
        }
        
        g.attacked = true;
        //HurtEffect.transform.GetChild(0).gameObject.SetActive(false);

        if ((g.Health <= 1) || (g.thisEnemyType == EnemyType.Boss && g.currentEventIdx == g.hitEvents.Count-1 && g.Health <= 10) )
        {
            float score = g.CalculateAttack(HitType.Punch, enemyManager.HittedPart);
            PunchIndicatorSound(score);
            enemyManager.NormScore = score;

            // Calculate Hit Force
            float ScoreRange = (float)MaxAlignmentScore - (float)FramesPrior;
            float PunchForceNormalized = (float)(PunchForce - FramesPrior);
            PunchForceNormalized /= ScoreRange;
            // Divide it further to reduce max force
            PunchForceNormalized /= 3f;
            PunchForceNormalized += 1.0f;

            // Add juice based on last controller velocity
            Vector3 CurrVeloVector;

            if (WhichHand == Hand.Right)
            {
                CurrVeloVector = OVRInput.GetLocalControllerVelocity(RTouchController);
            }
            else
            {
                CurrVeloVector = OVRInput.GetLocalControllerVelocity(LTouchController);
            }

            float VeloMagnitude = CurrVeloVector.magnitude;

            float CurrVelocity = Clamp((VeloMagnitude - VelocityCutoff) / (5.0f - VelocityCutoff), 0f, 0.3f);

            enemyManager.HitForce = PunchForceNormalized + CurrVelocity;

            // //Debug.Log(CurrVeloVector.magnitude);

            g.Death(collider.transform.root.gameObject, enemyManager);

            
            if (m_TutManager != null && !m_TutManager.tutorialFinished)
            {
                // m_TutManager.nextEnemy = true;
                ////Debug.Log(score);
                if (score > 0.3f && g.GetProperHits() == g.hitEvents.Count)
                {
                    // tutorial: success
                    
                    if (TutorialManager.TutorialEnemyContainer.Count != 1)
                    {
                        m_TutManager.PlaySound(2 + hitSoundIndex % 3);
                    }
                    m_TutManager.newEnemy[1] = true;
                    if (g.thisEnemyType == EnemyType.Grunt)
                    {
                        m_TutManager.newEnemy[0] = true;
                    }

                    hitSoundIndex++;

                }
                else
                {
                    // tutorial: wrong part/time
                    m_TutManager.PlaySound(0);
                    
                    
                    if(missHitIndex % TutorialCalloutSpan == 0)
                    {
                        m_TutManager.PlaySound(5);
                    }
                    missHitIndex++;
                    m_TutManager.newEnemy[1] = false;
                }
            }
        }
        else
        {

            PlayAnimation(collider);
            float score = g.CalculateAttack(HitType.Punch, enemyManager.HittedPart);
            PunchIndicatorSound(score);
            g.Hitted(collider.transform.root.gameObject);
            
            if (score > 0.3f)
            {
                //g.GreenHightEnd();
                g.IncrementProperHit();
                // tutorial: success
                if (m_TutManager != null && !m_TutManager.tutorialFinished)
                {
                    m_TutManager.newEnemy[0] = true;
                    
                }
                //Debug.Log("Hit result : enemy -1 health");
            }
            else
            {
                //g.StopCurrHighlight(hitLocation);
                // tutorial: wrong part/time
                if (m_TutManager != null && !m_TutManager.tutorialFinished)
                {
                    m_TutManager.PlaySound(0);
                    missHitIndex++;
                }
                //Debug.Log("Hit result : wrong hit");
            }
            
            g.Health--;
            if (g.thisEnemyType == EnemyType.Boss)
            {
                if (g.Health > (g.hitEvents.Count - g.currentEventIdx))
                    g.Health -= 2;
                //Debug.Log(g.Health);
            }
        }
        
        g.StopCurrHighlight(hitLocation);

        
    }


    // private IEnumerator FadeToFire()
    // {
    //     while (true)
    //     {
    //         if (g != null)
    //         {
                
    //         }
            
    //         yield return new WaitForSeconds(0.5f);
    //     }
    // }

    private void CheckFireHand()
    {
        if (IsMainMenu) return;
        
        if (Score.instance.GetScoreStreak() > Score.instance.perfectHitLimit && m_TutManager.tutorialFinished)
        {
            LFireHand.SetActive(true);
            LNormHand.SetActive(false);
            RFireHand.SetActive(true);
            RNormHand.SetActive(false);
            if (!LFireHand.GetComponent<AudioSource>().isPlaying)
            {
                LFireHand.GetComponent<AudioSource>().Play();
                LFireHand.GetComponent<AudioSource>().loop = true;
            }
            
        }
        else
        {
            LFireHand.SetActive(false);
            LNormHand.SetActive(true);
            RFireHand.SetActive(false);
            RNormHand.SetActive(true);
            LFireHand.GetComponent<AudioSource>().Stop();
        }
    }

    void PlayAnimation(Collider collider)
    {
        animator = collider.transform.root.GetComponent<Animator>();
        if (collider.transform.name == "Head_M" || collider.transform.name == "mixamorig:Head")
        {

            if (Vector3.Dot(transform.forward, -1 * collider.transform.forward) >= 0)
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("LeftHead"))
                {
                    animator.SetTrigger("chin");
                }
                else
                {
                    animator.SetTrigger("leftHead");
                }
            }
            else
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("RightHead"))
                {
                    animator.SetTrigger("chin");
                }
                else
                {
                    animator.SetTrigger("rightHead");
                }
            }
            
     
        }
        else if(collider.transform.name == "Stomach")
        {
            // if (Vector3.Dot(transform.forward, -1 * collider.transform.forward) >= 0)
            // {
            //     if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("LeftStomach"))
            //     {
            //         animator.SetTrigger("hardHitStomach");
            //     }
            //     else
            //     {
            //         animator.SetTrigger("leftStomach");
            //     }
            // }
            // else
            // {
            //     if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("RightStomach"))
            //     {
            //         animator.SetTrigger("hardHitStomach");
            //     }
            //     else
            //     {
            //         animator.SetTrigger("rightStomach");
            //     }
            // }
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Chest"))
            {
                animator.SetTrigger("hardHitStomach");
            }
            else
            {
                animator.SetTrigger("chest");

            }

        }
        else if (collider.transform.name == "Chest" || collider.transform.name == "mixamorig:Spine1")
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Chest"))
            {
                animator.SetTrigger("hardHitStomach");
            }
            else
            {
                animator.SetTrigger("chest");

            }
        }
       

        if (animator.GetBool("run") == true)
            animator.SetBool("run", false);
    }

    /// <summary>
    ///  Player's hand  have contact  with enemy's body with trigger
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        // Vector3 EnemyToPlayer = collider.transform.position - transform.position;
        // EnemyToPlayer.Normalize();
        ////Debug.Log(collider.name + ": " + collider.CompareTag("Attackable") + ": " + PunchValidationCheck() + ": " + waitTime);
        
        // if (!PunchValidationCheck())
        // {
        //     //Debug.Log(LeftAlignmentScore + "-" + RightAlignmentScore);
        // }
        if (collider.CompareTag("Attackable")) 
        {
            TimeSinceLastTrigger = TimeWaitForClear;
            int Validation = PunchValidationCheck();
            if (Validation > 0 && waitTime <= 0f)
            {
                Attacked(collider, Validation);
                waitTime = waitTimeVal;
            }
        }
        
    
    }
   

    /// <summary>
    /// Check the punch speed is a valid punch by adding alignment
    /// </summary>
    private void PunchAlignment()
    {
        Vector3 RCurrVeloVector = OVRInput.GetLocalControllerVelocity(RTouchController);
        Vector3 LCurrVeloVector = OVRInput.GetLocalControllerVelocity(LTouchController);
        float RCurrVelocity = RCurrVeloVector.magnitude;
        float LCurrVelocity = LCurrVeloVector.magnitude;
        
        // if (LCurrVelocity > 0.01 || RCurrVelocity > 0.01)
        // {
        //     //Debug.Log(LCurrVelocity + "-" + RCurrVelocity);
        //     //Debug.Log( Vector3.Dot(PrevRVelocity, RCurrVeloVector) + "-" + Vector3.Dot(PrevLVelocity, LCurrVeloVector));
        // }
        
       if (RCurrVelocity > ForceThreshold && Vector3.Dot(PrevRVelocity, RCurrVeloVector) > AlignmentThreshold)
        {
            RightAlignmentScore++;
        }
        else
        {
            RightAlignmentScore = 0;
        }
       
            // Is Left in a streak?
        if (LCurrVelocity > ForceThreshold && Vector3.Dot(PrevLVelocity, LCurrVeloVector) > AlignmentThreshold)
        {
            LeftAlignmentScore++;
        }
        else
        {
            LeftAlignmentScore = 0;
        }

        PrevLVelo = LCurrVelocity;
        PrevRVelo = RCurrVelocity;
        PrevLVelocity = LCurrVeloVector;
        PrevRVelocity = RCurrVeloVector;
    }

    private int PunchValidationCheck()
    {
        // Vector3 RVelocityWorld = PrevRVelocity;
        // Vector3 LVelocityWorld = PrevLVelocity;
        // RVelocityWorld.Normalize();
        // LVelocityWorld.Normalize();
        // EnemyForwardWorld.Normalize();
        ////Debug.Log(RVelocityWorld + " Enemy: " + EnemyForwardWorld);
        ////Debug.Log("Right: " + Vector3.Dot(RVelocityWorld, -EnemyForwardWorld) + " and Score: " + RightAlignmentScore + ", Left: " + Vector3.Dot(LVelocityWorld, -EnemyForwardWorld) + ". Score: " + LeftAlignmentScore);
        
        if (WhichHand == Hand.Right && (RightAlignmentScore > FramesPrior))
        {
            return RightAlignmentScore;
        }
        else if (WhichHand == Hand.Left && LeftAlignmentScore > FramesPrior)
        {
            return LeftAlignmentScore;
        }
        
        return 0;

        // return (/*(Vector3.Dot(RVelocityWorld, -EnemyForwardWorld) > AlignmentThreshold &&*/ (RightAlignmentScore > FramesPrior) || /*(Vector3.Dot(LVelocityWorld, -EnemyForwardWorld) > AlignmentThreshold &&*/ (LeftAlignmentScore > FramesPrior));
    }

    /// <summary>
    /// punch indicator sound,this might be move outise punch.cs in the future.
    /// </summary>
    private void PunchIndicatorSound(float normalizedscore)
    {


        //scoreIndicator = GetComponent<AudioSource>();
        
        if (normalizedscore >= 0.3f)
        {
            PunchEffects[0].Play();
            if (normalizedscore < 0.8f)
            {
                int RandomPunchSound = Random.Range(0, PunchNormalSounds.Length);
                punchIndicator.volume = NormalPunchVolume;
                punchIndicator.clip = PunchNormalSounds[RandomPunchSound];
                Score.instance.MainScoreText.SetText("Good!");
                if (normalizedscore < 0.3f)
                    Score.instance.ResetScoreStreak();
            }
            else
            {
                int RandomPunchSound = Random.Range(0, PunchPerfectSounds.Length);
                punchIndicator.volume = PerfectPunchVolume;
                punchIndicator.clip = PunchPerfectSounds[RandomPunchSound];
                Score.instance.MainScoreText.SetText("Perfect!");
            }     
        }
        else
        {
            StartCoroutine(MusicCutoffFilter.AdjustMusicFreq(0.5f, 5000));
            int RandomPunchMissSound = Random.Range(0, PunchMissSounds.Length);
            punchIndicator.clip = PunchMissSounds[RandomPunchMissSound];
            Score.instance.ResetScoreStreak();
            PunchEffects[0].Play();
        }

        Score.instance.MainScoreText.SetStreak(Score.instance.GetScoreStreak());


        CheckFireHand();

        StartCoroutine(VibrateController(normalizedscore));
        punchIndicator.Play();
        //PunchEffects[0].Play();
    }


    private IEnumerator VibrateController(float Score)
    {
        float ActivationPotential = Score;
        if (ActivationPotential > 1.0f) ActivationPotential = 1.0f;
        if (WhichHand == Hand.Left) OVRInput.SetControllerVibration(ActivationPotential, ActivationPotential, LTouchController);
        else OVRInput.SetControllerVibration(ActivationPotential, ActivationPotential, RTouchController);
        yield return new WaitForSeconds(ActivationPotential / 3);
        if (WhichHand == Hand.Left) OVRInput.SetControllerVibration(0, 0, LTouchController); 
        else OVRInput.SetControllerVibration(0, 0, RTouchController); 
    }


    private void Update()
    {
        
        PunchAlignment();
        //PunchIntervalCheck();
        if (TimeSinceLastTrigger > 0)
        {
            TimeSinceLastTrigger -= Time.deltaTime;
        }
        
        if (waitTime > 0)

            if (TimeSinceLastTrigger < 0.0f)
            {
                waitTime = 0.0f;
                TimeSinceLastTrigger = 0.0f;
            }

            // If player is moving, then the waitTime should be decremented twice as fast
            // The punch cooldown is primarily for the case where the player doesn't move their
            // hand and the enemy accidentally triggers it
            // if  (PunchValidationCheck())
            // {
            //     waitTime -= 2*Time.deltaTime;
            // }

            waitTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.C))
            PunchEffects[1].Play();

    }

}