using System;

using System.Collections;

using System.Collections.Generic;

using System.Linq;
using UnityEngine;



public class MusicEventsParser

{

    
    public static bool Parse(ref Queue<EnemySpawn> spawns, string SongName, ref GameObject[] EnemyPrefabs, ref float SpawnCarTime, float normalRunningTime, float slowdownRunningTime)
    {
        //Debug.Log(string.Concat("music_events_" + SongName));
        TextAsset source = Resources.Load<TextAsset>(string.Concat("music_events_" + SongName));
        CsvReader csv = new CsvReader(source.text, true);

        EnemySpawn current = null;
        int windowEndPointer = 0, hitEventPointer = 0;
        int winStCt = 0, hitCt = 0, winEndCt = 0;
        float timing = 0;
        string currName = "";
        HashSet<string> loaded = new HashSet<string>();

        float EnemySpeed = 0.0f;
        float EnemyDistance = 0.0f;

        int LastCombo = 0;

        int NumSpawns = 0;

        while (csv.Read())
        {
            ////Debug.Log(csv.GetFieldOrEmpty("Timing"));
            int[] timeArray = csv.GetFieldOrEmpty("Timing").Split(':').Select(int.Parse).ToArray();
            // time[2] is assume milliseconds in 10ms increments
            
            
            float timefloat = timeArray[0] * 60 + timeArray[1] + (((float)timeArray[2]) / 1000.0f);
            string eventType = csv.GetFieldOrEmpty("HitType");
            // if(winStCt!=winEndCt || winStCt!=hitCt)
            // {
            //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Previous Enemy has unclosed hitWindow");
            //     return false;
            // }
            EnemySpawn newSpawn = new EnemySpawn
            {
                locationIdx = int.Parse(csv.GetFieldOrEmpty("EnemyPosition")),
            };
            string enemytype = csv.GetFieldOrEmpty("TargettedEnemy");
            if (enemytype.Contains("Grunt"))
            {
                newSpawn.type = EnemyType.Grunt;
                //EnemySpeed = EnemyPrefabs[0].GetComponent<Grunt>().runSpeed;
                //EnemyDistance = EnemyPrefabs[0].GetComponent<Grunt>().SpawnDistance;
            }
            else if (enemytype.Contains("Combo"))
            {
                newSpawn.type = EnemyType.Combo;
            }
            else if (enemytype.Contains("Boss"))
            {
                newSpawn.type = EnemyType.Boss;
            }
            else
            {
                //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Unkown enemy type " + enemytype);
                return false;
            }


            if (int.Parse(csv.GetFieldOrEmpty("Combo")) == 0)
            {
                current = newSpawn;
                current.hits.Add(new HitEvent
                {
                    start = timefloat - 0.5f,
                    hitPerfect = timefloat,
                    end = timefloat + 0.75f
                });

                newSpawn.spawnTime = current.hits[0].start - normalRunningTime;
                spawns.Enqueue(newSpawn);
                NumSpawns++;

                windowEndPointer = 0;
                hitEventPointer = 0;
            }
            else
            {
                current.hits.Add(new HitEvent
                {
                    // start = current.hits[hitEventPointer-1].hitPerfect,
                    // hitPerfect = current.hits[hitEventPointer-1].hitPerfect + 0.75f,
                    // end = current.hits[hitEventPointer-1].hitPerfect + 1.5f
                    start = timefloat - 0.9f,
                    hitPerfect = timefloat,
                    end = timefloat + 0.75f
                });
                if (current.type != EnemyType.Boss)
                {
                    current.type = EnemyType.Combo;
                }
                // 
            }
            /*
            float EnemyTime = EnemyDistance / EnemySpeed;

            if (int.Parse(csv.GetFieldOrEmpty("Combo")) == 0)
            {
                newSpawn.spawnTime = timefloat - EnemyTime;
                spawns.Enqueue(newSpawn);
                current = newSpawn;
                current.hits.Add(new HitEvent
                {
                    start = EnemyTime - 1.0f,
                    hitPerfect = EnemyTime,
                    end = EnemyTime + 1.0f
                });
                
                windowEndPointer = 0;
                hitEventPointer = 0;
            }
            else
            {
                if (LastCombo == 0)
                {
                    current.hits[0].start = EnemyTime;
                    current.hits[0].hitPerfect = EnemyTime+1.0f;
                    current.hits[0].end = EnemyTime+2.0f;
                }

                if (timefloat != 0.0f)
                {
                    current.hits.Add(new HitEvent
                    {
                        start = timefloat - current.spawnTime - 1.0f,
                        hitPerfect = timefloat - current.spawnTime,
                        end = timefloat - current.spawnTime + 1.0f
                    });
                }
                else
                {
                    current.hits.Add(new HitEvent
                    {
                        start = (current.hits[hitEventPointer-1].hitPerfect),
                        hitPerfect = (current.hits[hitEventPointer-1].hitPerfect+1.0f),
                        end = (current.hits[hitEventPointer-1].hitPerfect+2.0f)

                    });
                }
            }*/
            timing = timefloat;

            LastCombo = int.Parse(csv.GetFieldOrEmpty("Combo"));

            //loaded.Add(currName);
            currName = csv.GetFieldOrEmpty("TargettedEnemy");
            // if (loaded.Contains(currName))
            // {
            //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Repeated enemy name.");
            //     return false;
            // }
            // next line should be run-up
            // if (csv.Read())
            // {
            //     timeArray = csv.GetFieldOrEmpty("Timing").Split(':').Select(int.Parse).ToArray();
            //     timefloat = timeArray[0] * 60 + timeArray[1] + timeArray[2] / 100.0f;
                
            //     eventType = csv.GetFieldOrEmpty("HitType"); 
            //     // if (currName != csv.GetFieldOrEmpty("TargettedEnemy"))
            //     // {
            //     //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Enemy is not in order");
            //     //     return false;
            //     // }
            //     // if (eventType == "Run-Up")
            //     // {
            //     //     if(timefloat<timing)
            //     //     {
            //     //         //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Timing is not in order");
            //     //         return false;
            //     //     }
            //     //     timing = timefloat;
            //     //     current.runStart = timefloat - current.spawnTime;
            //     // }
            //     // else
            //     // {
            //     //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Spawn is not followed by Run-Up");
            //     //     return false;
            //     // }
            // }
            // else
            // {
            //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Spawn can't be the last line of a file");
            //     return false;
            // }
        

            //else if (eventType == "StartHitWindow")
            // {
            //     if (timefloat < timing)
            //     {
            //         //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Timing is not in order");
            //         return false;
            //     }
            //     if (currName != csv.GetFieldOrEmpty("TargettedEnemy"))
            //     {
            //         //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Enemy is not in order");
            //         return false;
            //     }
            //     timing = timefloat;
            //     current.hits.Add(new HitEvent
            //     {
            //         start = timefloat - current.spawnTime
            //     });
            //     ++winStCt;
            // }

        

            // else if (eventType == "EndHitWindow")
            // {
            //     if (timefloat < timing)
            //     {
            //         //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Timing is not in order");
            //         return false;
            //     }
            //     if (currName != csv.GetFieldOrEmpty("TargettedEnemy"))
            //     {
            //         //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Enemy is not in order");
            //         return false;
            //     }
            //     timing = timefloat;
            //     if (winStCt > winEndCt && hitCt > winEndCt)
            //     {
            //         // if program has read more StartHitWindow than EndHitWindow
            //         // && the window to end has read an event
            //         ++winEndCt;
            //     }
            //     else
            //     {
            //         //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Ending a hitWindow without hit event");
            //         return false;
            //     }
            //     current.hits[windowEndPointer].end = timefloat - current.spawnTime;
            //     ++windowEndPointer;
            // }
        
            if (eventType == "Punch" || eventType == "Block" ||
                    eventType == "Grab" || eventType == "Throw")
            {
                // if (timefloat < timing)
                // {
                //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Timing is not in order");
                //     return false;
                // }
                // if (currName != csv.GetFieldOrEmpty("TargettedEnemy"))
                // {
                //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Enemy is not in order");
                //     return false;
                // }
                // timing = timefloat;
                // if (winStCt>hitCt)
                // {
                //     ++hitCt;
                // }
                // else
                // {
                //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Hit event read before StartHitWindow");
                //     return false;
                // }
                if(eventType == "Punch" || eventType == "Grab")
                {
                    string loc = csv.GetFieldOrEmpty("HitLocation");
                    if(loc != "Stomach" && loc != "Head" && loc != "Chest")
                    {
                        //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Hit Location Format Error");
                        return false;
                    }
                }
                current.hits[hitEventPointer].type = (HitType)Enum.Parse(typeof(HitType), eventType);
                current.hits[hitEventPointer].hitLocation = csv.GetFieldOrEmpty("HitLocation");
                current.flyTarget = csv.GetFieldOrEmpty("FlyTarget");
                ////Debug.Log(current.flyTarget);
                if (current.flyTarget == "Car")
                {
                    if (timefloat >= 6.0f)
                        SpawnCarTime = timefloat - 6.0f;
                    else 
                        SpawnCarTime = -1.0f;
                }
                ++hitEventPointer;

            }
        
        }

        // else
        // {
        //     //Debug.Log("Parsing Error line " + csv.GetLineNumber() + ": Unexpected event type " + eventType);
        //     return false;
        // }
        
        //Debug.Log("Num Spawns: " +  NumSpawns);

        return true;
    }


}

