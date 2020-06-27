using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawnerScript : MonoBehaviour
{
    private topButtonMenuScript playButtonInstance;

    private int currentWave = 0;
    private bool currentWaveFinished = true;
    private int numCreepsSpawned = 0;
    private float creepSpawnTimer = 0;
    private bool canSpawnCreeps = true;
    private float newWaveTimer = 0;

    private struct creepWaveData
    {
        public int creepCount;
        public float creepSpawnDelay;
        public GameObject creepObj;
        public float timeBeforeNextWave;
    };
    private creepWaveData[] waveData;

    // Start is called before the first frame update
    void Start()
    {
        playButtonInstance = GameObject.Find("TopRightMenu").GetComponent(typeof(topButtonMenuScript)) as topButtonMenuScript;

        initWaveData();
    }

    void initWaveData()
    {
        waveData = new creepWaveData[11];

        // This wave gets skipped
        waveData[0].creepCount = 0;
        waveData[0].timeBeforeNextWave = 2f;

        waveData[1].creepCount = 4;
        waveData[1].creepSpawnDelay = 1.2f;
        waveData[1].creepObj = GameObject.Find("enemySoldier");
        waveData[1].timeBeforeNextWave = 4f;

        waveData[2].creepCount = 8;
        waveData[2].creepSpawnDelay = 1.2f;
        waveData[2].creepObj = GameObject.Find("enemySoldier");
        waveData[2].timeBeforeNextWave = 3.5f;
        
        waveData[3].creepCount = 3;
        waveData[3].creepSpawnDelay = 1.2f;
        waveData[3].creepObj = GameObject.Find("enemyMiniTank");
        waveData[3].timeBeforeNextWave = 3.5f;

        waveData[4].creepCount = 6;
        waveData[4].creepSpawnDelay = 1.2f;
        waveData[4].creepObj = GameObject.Find("enemyMiniTank");
        waveData[4].timeBeforeNextWave = 5.5f;

        waveData[5].creepCount = 9;
        waveData[5].creepSpawnDelay = 1.2f;
        waveData[5].creepObj = GameObject.Find("enemyTank");
        waveData[5].timeBeforeNextWave = 2.5f;

        waveData[6].creepCount = 20;
        waveData[6].creepSpawnDelay = 0.6f;
        waveData[6].creepObj = GameObject.Find("enemyBuggy");
        waveData[6].timeBeforeNextWave = 2.5f;

        waveData[7].creepCount = 6;
        waveData[7].creepSpawnDelay = 0.8f;
        waveData[7].creepObj = GameObject.Find("enemyTank");
        waveData[7].timeBeforeNextWave = 2.5f;

        waveData[8].creepCount = 40;
        waveData[8].creepSpawnDelay = 0.4f;
        waveData[8].creepObj = GameObject.Find("enemySoldier");
        waveData[8].timeBeforeNextWave = 2.5f;

        waveData[9].creepCount = 50;
        waveData[9].creepSpawnDelay = 0.8f;
        waveData[9].creepObj = GameObject.Find("enemyTank");
        waveData[9].timeBeforeNextWave = 2.5f;

        waveData[10].creepCount = 100;
        waveData[10].creepSpawnDelay = 0.6f;
        waveData[10].creepObj = GameObject.Find("enemyBuggy");
        waveData[10].timeBeforeNextWave = 3.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playButtonInstance == null)
            return;

        // Play button currently kicks off the waves or skips to the next
        int waveNum = playButtonInstance.getWaveNumber();
        if ((waveNum != currentWave) && currentWaveFinished)
        {
            // Start new wave of creeps
            currentWave = waveNum;
            numCreepsSpawned = 0;
            currentWaveFinished = false;                       
        }

        if (!currentWaveFinished)
        {
            // Dont spawn creeps if the game is paused
            if (!canSpawnCreeps)
                return;

            // Dont spawn creeps that arent defined
            if (currentWave >= waveData.Length)
                return;

            creepSpawnTimer += Time.deltaTime;
            if (creepSpawnTimer >= waveData[currentWave].creepSpawnDelay)
            {
                //Debug.Log("currentWave=" + currentWave + " numCreepsSpawned=" + numCreepsSpawned);

                // Reset timer
                creepSpawnTimer = 0;

                Vector3Int[] genPath = GameObject.Find("pathGenerator").GetComponent<pathGeneratorScript>().getPathList();

                // Spawn creep
                GameObject newCreep = (GameObject)Instantiate(waveData[currentWave].creepObj);
                newCreep.GetComponent<enemyScript>().setWaypoints(genPath);
                newCreep.GetComponent<enemyScript>().initCreep();

                numCreepsSpawned++;
                if (numCreepsSpawned >= waveData[currentWave].creepCount)
                {
                    // finished spawning creeps for this wave
                    currentWaveFinished = true;                    
                }
            }
        }
        
        // If current wave complete, start counting to start the next wave
        if (currentWaveFinished && (currentWave != 0))
        {
            //Debug.Log("currentWave=" + currentWave + " newWaveTimer=" + newWaveTimer);
            newWaveTimer += Time.deltaTime;

            // Inform GUI about when the next wave is coming
            if (playButtonInstance != null)
                playButtonInstance.setWaveTimer(waveData[currentWave].timeBeforeNextWave - newWaveTimer);

            if (newWaveTimer >= waveData[currentWave].timeBeforeNextWave)
            {
                // reset timer
                newWaveTimer = 0;
                // Jump to next wave
                currentWave++;
                // Inform GUI that were on next wave
                if (playButtonInstance != null)
                    playButtonInstance.setWaveNumber(currentWave);
                // Start wave
                numCreepsSpawned = 0;
                currentWaveFinished = false;
            }
        }
    }

    public void pauseEnemySpawning()
    {
        canSpawnCreeps = false;
    }

    public void resumeEnemySpawning()
    {
        canSpawnCreeps = true;
    }
}
