using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class EnemySpawnHandler : MonoBehaviour

{ 
    #region Singleton
    public static EnemySpawnHandler instance;

    //public GameObject[] SpawnHistory = new GameObject[4];

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of EnemySpawnHandler found!");
            return;
        }

        instance = this;
    }
    #endregion

    [SerializeField]
    public int SpawnAngle = 30;

    [SerializeField]

    GameObject spawnLocationsContainer;

    [SerializeField]

    GameObject[] EnemyPrefabs;
    List<Transform> spawnLocations;

    Queue<EnemySpawn> spawns;

    bool isSpawning;

    public float timeSpawningStart;

    private GameObject enemyPrefab;
    private GameObject enemy1;
    private GameObject enemy2;
    private GameObject enemy3;
    private GameObject enemy4;
    
    public bool isTrailer = false;
    private Transform Player;
    
    public Vector3 PlayerPosition;
    private float hitWindowTime;                        // predefined hit Window time (during slow down)
    public float normalRunningTime;                  // time cost from spawn to start slowing down
    private float SpawnDistance = 9.0f;                // distance from enemy to player
    [Header("Enemy Running Parameter")]
    [Range(0f, 1f)]
    [Tooltip("Scalar of slow down distanace to the total distance")]
    public float slowDownDistanceScalar = 0.1f;       // ratio of slowdown distance to total distance
    [Range(0f,1f)]
    [Tooltip("Slow Down factor in slow motion 0 is not move, 1 is normal speed")]
    public float slowDownFactor = 0.1f;               // slowDown factor
    [Tooltip("Speed when running with normal speed")]
    public float normalRunningSpeed = 9.0f;

    private bool StartSpawning = false;

    private float SpawnCarTime = -1.0f;

    public float MusicTime;
    
    Vector3 CarInitialPosition;

    Vector3 CarDesiredPosition;

    GameObject FlyDestObj;

    float CarStartMovingTime = -1.0f;

    void Start()

    {
        #region VarInitialization



        spawns = new Queue<EnemySpawn>();
        spawnLocations = new List<Transform>();


        #endregion

        GameObject[] players = GameObject.FindGameObjectsWithTag("MainCamera");
        if (players.Length > 0)
        {
            Player = players[0].transform;
        }
        else
        {
            Player = null;
        }

        CalculateRunningTime();
    }
    public void CalculateRunningTime()
    {
        // Calculate the time for each enemy reach player's position
        normalRunningTime = ((1 - slowDownDistanceScalar) * SpawnDistance) / normalRunningSpeed;
        hitWindowTime = (slowDownDistanceScalar * SpawnDistance) / (normalRunningSpeed * slowDownFactor);
    }
    public bool SpawnHandler(string songName, float gameStartTime)
    {
        // Parse Beatmap
        bool success = MusicEventsParser.Parse(ref spawns, songName, ref EnemyPrefabs, ref SpawnCarTime, normalRunningTime, hitWindowTime);

        // if (SpawnCarTime >= 0.0f)
        // {
        //     FlyDestObj = GameObject.Find("FlyDestination/Car");
        //     CarInitialPosition = FlyDestObj.transform.position;
        //     CarDesiredPosition = CarInitialPosition + FlyDestObj.transform.right*20.0f;
        //     //CarDesiredPosition.y = 0.5f;
            
        // }

        if (!success)
        {
            //Debug.Log("Parse music events failed.");
        }

        PassSpawnInfor();

        // spawn enemy according to beatmap
        timeSpawningStart = gameStartTime;
        if (spawns.Count > 0)
        {
            StartSpawning = true;
            return true;
        }
            
        return false;
            //StartCoroutine(SpawnEnemies());

    }

    public void Update()
    {
        if (StartSpawning)
        {       
            // start spawning
            if (spawns.Count > 0)
            {
                EnemySpawn e = spawns.Peek();

                if (e.spawnTime < (Time.time - timeSpawningStart))
                {
                    SpawnEnemy(spawns.Dequeue());
                }
            }
            else
            {
                StartSpawning = false;
            }
        }

        // if (SpawnCarTime >= 0.0f)
        // {
        //     if (SpawnCarTime < (Time.time - timeSpawningStart))
        //     {
        //         // Move Car
        //         if (CarStartMovingTime == -1.0f) CarStartMovingTime = Time.time - timeSpawningStart;
        //         if (FlyDestObj != null && Vector3.Distance(CarDesiredPosition, FlyDestObj.transform.position) > 0.1f)
        //         {
        //             FlyDestObj.transform.position = Vector3.Lerp(CarInitialPosition, CarDesiredPosition, (Time.time - timeSpawningStart - CarStartMovingTime) / 5.0f);
        //         }
        //         else
        //         {
        //             SpawnCarTime = -1.0f;
        //         }
        //         ////Debug.Log(FlyDestObj.transform.position);
        //     }
        // }

    }

    public void EnqueueSpawnEvent(EnemySpawn es)
    {
        spawns.Enqueue(es);
    }

    // public IEnumerator SpawnEnemies()
    // {
    //     float deltaTimeStart = Time.time - timeSpawningStart;
    //     yield return new WaitForSeconds(spawns.Peek().spawnTime - deltaTimeStart);
    //     // start spawning
    //     // bool playAllUpToCurrent = false;
    //     while (/*!playAllUpToCurrent &&*/ spawns.Count > 0)
    //     {
    //         deltaTimeStart = Time.time - timeSpawningStart;
    //         EnemySpawn e = spawns.Peek();
    //         // if (e.spawnTime > deltaTimeStart)
    //         // {
    //         //     playAllUpToCurrent = true;
    //         // }
    //         // else
    //         if (e.spawnTime > deltaTimeStart)
    //         {
    //             ////Debug.Log(deltaTimeStart);
    //             SpawnEnemy(e);
    //             spawns.Dequeue();
    //         }
    //         //yield return new WaitForSeconds(spawns.Peek().spawnTime - deltaTimeStart);
    //     }
    //     // if (spawns.Count > 0)
    //     //     StartCoroutine(SpawnEnemies());
    // }

    //void GetPlayerPosition()
    //{
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("MainCamera");
    //    if (players.Length > 0)
    //    {
    //        PlayerPosition = players[0].transform.position;
    //    }
    //    else
    //    {
    //        //Debug.LogError("Player Not Found");
    //    }
    //}

    public void SpawnEnemy(EnemySpawn es)
    {
        switch (es.type)
        {
            case EnemyType.Grunt:
                enemyPrefab = EnemyPrefabs[0];
                break;
            case EnemyType.Combo:
                enemyPrefab = EnemyPrefabs[1];
                break;
            case EnemyType.Boss:
                enemyPrefab = EnemyPrefabs[2];
                break;
            default:
                break;
        }
        //GetPlayerPosition();
        float sinceSpawn = Time.time - timeSpawningStart;
        Vector3 SpawnPosition = Player.position + SpawnDistance * (Quaternion.AngleAxis(-1 * SpawnAngle * (es.locationIdx - 2), Vector3.up) * Vector3.right);
        //SpawnPosition.y = -1.3f;
        SpawnPosition.y = 0.5f;
        ////Debug.Log(sinceSpawn + " vs " + es.spawnTime + " spawning enemy at location " + SpawnPosition);
        GameObject newEnemy = Instantiate(enemyPrefab, SpawnPosition, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().SetParameter(es, Player.position, Player.forward, normalRunningTime, hitWindowTime, slowDownFactor, timeSpawningStart, slowDownDistanceScalar, isTrailer);

        switch (es.locationIdx)
        {
            case 1:
                //if (enemy1 != null && es.hits.Count > 1) Destroy(enemy1);
                enemy1 = newEnemy;
                break;
            case 2:
                //if (enemy2 != null && es.hits.Count > 1) Destroy(enemy2);
                enemy2 = newEnemy;
                break;
            case 3:
                //if (enemy3 != null && es.hits.Count > 1) Destroy(enemy3);
                enemy3 = newEnemy;
                break;
            case 4:
                //if (enemy4 != null && es.hits.Count > 1) Destroy(enemy4);
                enemy4 = newEnemy;
                break;
        }
    }

    public GameObject GetEnemyByIndex(int idx)
    {
        switch (idx)
        {
            case 1:
                return enemy1;
            case 2:
                return enemy2;
            case 3:
                return enemy3;
            case 4:
                return enemy4;
            default:
                return null;
        }
    }

    private void PassSpawnInfor()
    {
        //Count total punches
        foreach (EnemySpawn enemy in spawns)
        {
            Score.instance.totalEnemy += enemy.hits.Count();
            MusicTime = enemy.hits[enemy.hits.Count()-1].hitPerfect;
        }

    }

}



public enum EnemyType
{
    Grunt,
    Combo,
    Boss
}



public class EnemySpawn
{
    public float spawnTime;
    public EnemyType type;
    public int locationIdx;
    public float runStart;
    public List<HitEvent> hits = new List<HitEvent>();
    public string flyTarget;
    public bool isTutorial = false;
}

public class HitEvent
{
    public HitType type;
    public float start;
    public float end;
    public string hitLocation;
    public float hitPerfect;
}

public enum HitType
{
    Punch,
    Grab,
    Throw,
    Block
}

public enum HitLocation
{
    Head,
    Chest,
    Stomach,
    Default
}

