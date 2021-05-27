using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region Singleton
    public static TutorialManager instance;

    //public GameObject[] SpawnHistory = new GameObject[4];

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of TutorialManager found!");
            return;
        }

        instance = this;
    }
    #endregion

    public GameObject SpawnPoint;
    public static Queue<EnemySpawn> TutorialEnemyContainer;
    public bool nextEnemy = false;          // release an enemy

    [HideInInspector] public bool[] newEnemy;           // next new enemy in Queue
    public bool tutorialFinished = false;
    private float TutorialStartTime = -1.0f;
    public static int firstComboEnemy = 0;

    public List<int> missHitIndex;

    public GameObject ComboEnemyForShow;

    // for synching tutorial enemy with tutorial music
    float tutMusicBegin;
    float secPerBeat;

    // 0-2: success, wrong, miss
    public AudioClip[] sounds;
    public AudioSource tutAudio;
    [SerializeField] AudioSource dholAudioSource;

    public EnemyType CurrentEnemy = EnemyType.Grunt;
    
    void Start()
    {
        TutorialEnemyContainer = new Queue<EnemySpawn>();
        missHitIndex = new List<int>(sounds.Length);

        newEnemy = new bool[2];

        newEnemy[0] = false;
        newEnemy[1] = false;

        secPerBeat = 60.0f /175.0f * 4;   // "dhol" around 175bpm, *4 for only counting the first heavy beat
    }

    public void PlaySound(int idx)
    {
        if (idx >= sounds.Length) return;
        //tutAudio.clip = sounds[idx];
        tutAudio.PlayOneShot(sounds[idx]);
    }

    public void AddTutorialEnemy()
    {
        SetupEnemy(2, "Head");
        SetupEnemy(2, "Stomach");
        SetupEnemy(2, "Chest");
        SetupEnemy(2, "Head", "Chest");
        SetupEnemy(1, "Head", "Stomach");
        SetupEnemy(1, "Chest", "Stomach");
        SetupEnemy(1, "Chest");

        TutorialStartTime = Time.time;
    }

    private void Update()
    {
        if (TutorialEnemyContainer.Count > 0 && nextEnemy && !CloseUpUIManager.restartGame)
        {
            //UnityEngine.//Debug.Log(TutorialEnemyContainer.Count);
            nextEnemy = false;
            if (newEnemy[0] && newEnemy[1])
            {
                TutorialEnemyContainer.Dequeue();
            }
            newEnemy[0] = false;
            newEnemy[1] = false;
            
            if (TutorialEnemyContainer.Count > 0)
            {
  
                EnemySpawn enemy = TutorialEnemyContainer.Peek();
                CurrentEnemy = enemy.type;
                if (firstComboEnemy == 0 && enemy.type == EnemyType.Combo)
                {
                    firstComboEnemy = 1;
                    StartCoroutine(delayForFirstComboEnemy(enemy));
                    //spawnedEnemy.GetComponent<Enemy>().firstComboEnemyHighlightDelayFactor = 0.001f;
                    //spawnedEnemy.GetComponent<Animator>().SetFloat("slowDownFactor", 0.1f);
                    PlaySound(6);

                }
                else
                {
                    StartCoroutine(AddEnemyParameter(enemy));
                    SpawnPoint.GetComponent<EnemySpawnHandler>().SpawnEnemy(enemy);
                }
                
            

            }
            else
            {
                dholAudioSource.Stop();
                dholAudioSource.clip = null;
                PlaySound(7);
                StartCoroutine(delayAfterTutorialFinished());
            }
            
        }

        if (CloseUpUIManager.restartGame && !CloseUpUIManager.hasRestarted)
        {
            CloseUpUIManager.hasRestarted = true;
            StartCoroutine(delayAfterTutorialFinished());
        }
        // if (TutorialStartTime > 0 && newEnemy && TutorialEnemyContainer.Count == 0)
        // {
            
        // }
            
    }

    public void DelayEnemySpawn(float seconds)
    {
        StartCoroutine(DelayNextEnemy(seconds));
    }
    IEnumerator DelayNextEnemy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        nextEnemy = true;
    }

    IEnumerator delayForFirstComboEnemy(EnemySpawn enemy)
    {
        //yield return new WaitForSeconds(2.0f);
        ComboEnemyForShow.SetActive(true);
        yield return new WaitForSeconds(sounds[6].length + 0.5f);
        StartCoroutine(AddEnemyParameter(enemy));
        Destroy(ComboEnemyForShow);
        SpawnPoint.GetComponent<EnemySpawnHandler>().SpawnEnemy(enemy);
    }

    IEnumerator delayAfterTutorialFinished()
    {
        OVRScreenFade FadeScript = GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>();
        FadeScript.FadeOut();
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("GameManager").GetComponent<GameInfor>().ActivateLevelObjs();
        Score.instance.ResetScoreStreak();
        FadeScript.StartCoroutine(FadeScript.Fade(1, 0));
        yield return new WaitForSeconds(4.5f);
        tutorialFinished = true;
    }


    IEnumerator AddEnemyParameter(EnemySpawn enemy)
    {
        float runningTime = SpawnPoint.GetComponent<EnemySpawnHandler>().normalRunningTime;
        // original start time with no sync
        float rawSpawn = Time.time;     
        // original hitPerfect time with no sync
        float perfect = rawSpawn + runningTime + 0.5f;
        // push hitPerfect to next on-beat time, and calculate offset
        float offset = Mathf.Ceil((perfect - tutMusicBegin) / secPerBeat) * secPerBeat + tutMusicBegin - perfect;
        // calculate hitPerfect with offset, then compute start/end based on perfect
        perfect += offset;

        //Debug.Log(rawSpawn + ", " + offset);
        SpawnPoint.GetComponent<EnemySpawnHandler>().timeSpawningStart = -offset;

        enemy.spawnTime = rawSpawn + offset;
        enemy.hits[0].start = perfect - 0.5f;
        enemy.hits[0].hitPerfect = perfect;
        enemy.hits[0].end = perfect + 0.75f;
        if(enemy.type == EnemyType.Combo)
        {
            // push hitPerfect to next on-beat time, and calculate offset
            offset = Mathf.Ceil(0.9f / secPerBeat) * secPerBeat - 0.9f; // 0.9: diff betw 1st and 2nd start/hitPerfect/end
            perfect = perfect + 0.9f + offset;
            enemy.hits[1].hitPerfect = perfect;
            enemy.hits[1].start = perfect - 0.5f;
            enemy.hits[1].end = perfect + 0.75f;
        }

        yield return new WaitForSeconds(enemy.spawnTime - Time.time);
    }

    /// <summary>
    /// Set Up enemy spawen location and hit location before spawning
    /// </summary>
    /// <param name="enemySpawn"></param>
    /// <param name="locationIdx"></param>
    /// <param name="hitLocation"></param>
    private void SetupEnemy(int locationIdx, string hitLocation)
    {
        EnemySpawn enemySpawn = new EnemySpawn();

        enemySpawn.locationIdx = locationIdx;
        HitEvent hitEvent = new HitEvent();
        hitEvent.hitLocation = hitLocation;
        hitEvent.type = HitType.Punch;
        hitEvent.start = 0.0f;
        hitEvent.hitPerfect = 3f;
        hitEvent.end = 4f;
        enemySpawn.hits.Add(hitEvent);
        enemySpawn.type = EnemyType.Grunt;
        enemySpawn.spawnTime = 0f;
        enemySpawn.runStart = 0f;
        enemySpawn.flyTarget = null;
        enemySpawn.isTutorial = true;
        TutorialEnemyContainer.Enqueue(enemySpawn);
    }
    private void SetupEnemy(int locationIdx, string hitLocation1, string hitLocation2)
    {
        EnemySpawn enemySpawn = new EnemySpawn();

        enemySpawn.locationIdx = locationIdx;
        HitEvent hitEvent1 = new HitEvent();

        hitEvent1.hitLocation = hitLocation1;
        hitEvent1.type = HitType.Punch;
        hitEvent1.start = 0.0f;
        hitEvent1.hitPerfect = 3f;
        hitEvent1.end = 4f;
        enemySpawn.hits.Add(hitEvent1);

        HitEvent hitEvent2 = new HitEvent();
        hitEvent2.hitLocation = hitLocation2;
        hitEvent2.type = HitType.Punch;
        hitEvent2.start = 4f;
        hitEvent2.hitPerfect = 6f;
        hitEvent2.end = 7f;
        enemySpawn.hits.Add(hitEvent2);

        enemySpawn.type = EnemyType.Combo;
        enemySpawn.spawnTime = 0f;
        enemySpawn.runStart = 0f;
        enemySpawn.flyTarget = null;
        enemySpawn.isTutorial = true;

        TutorialEnemyContainer.Enqueue(enemySpawn);
    }

    public void MarkTutMusicBegin(float begin)
    {
        tutMusicBegin = begin;
    }
}
