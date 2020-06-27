using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class topButtonMenuScript : MonoBehaviour
{
    // Wave number vars
    Text waveText;
    public int maxNumWaves = 20;
    int waveNumber = 0;
    bool updateWaveNumber = false;

    // Healthbar vars
    Text healthText;
    int healthValue = 100;
    bool updateHealthValue = false;

    // Wave Timer vars
    Text waveTimer;
    float waveTimerValue = 0f;
    bool updateWaveTimer = false;

    bool gamePaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "PlayButton")
            {
                child.gameObject.GetComponent<Button>().onClick.AddListener(playButtonActionHandler);
            }
            else if (child.name == "PlayButton")
            {
                child.gameObject.GetComponent<Button>().onClick.AddListener(stopButtonActionHandler);
            }
            else if (child.name == "WaveText")
            {
                waveText = child.gameObject.GetComponent<Text>();
            }
            else if (child.name == "HealthText")
            {
                healthText = child.gameObject.GetComponent<Text>();
            }
            else if (child.name == "WaveTimer")
            {
                waveTimer = child.gameObject.GetComponent<Text>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateWaveNumber)
        {
            updateWaveNumber = false;

            if (waveText != null)
                waveText.text = "Wave: " + waveNumber.ToString() + " / " + maxNumWaves.ToString();
        }

        if (updateHealthValue)
        {
            updateHealthValue = false;

            if (healthText != null)
                healthText.text = "Health: " + healthValue.ToString();
        }

        if (updateWaveTimer)
        {
            updateWaveTimer = false;

            if (waveTimer != null)
                waveTimer.text = "Next in: " + waveTimerValue.ToString("F1");
        }
    }

    void playButtonActionHandler()
    {
        if (gamePaused)
        {
            gamePaused = false;

            // Resume enemy spawner script
            GameObject.Find("enemySpawner").GetComponent<enemySpawnerScript>().resumeEnemySpawning();

            // Unfreeze all enemies
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject enemy in enemys)
            {
                if (enemy == null)
                    continue;

                enemyScript enemyInstance = enemy.GetComponent<enemyScript>();
                enemyInstance.resumeGame();
            }

            // Unfreeze all towers
            //TODO
        }
        else
        {
            // Start the next wave
            if (waveNumber < maxNumWaves)
            {
                waveNumber++;
                updateWaveNumber = true;
            }
        }
    }

    void stopButtonActionHandler()
    {
        // If the game is not already paused
        if (!gamePaused)
        {
            gamePaused = true;

            // Pause enemy spawner script
            GameObject.Find("enemySpawner").GetComponent<enemySpawnerScript>().pauseEnemySpawning();

            // Loop through all enemies and disable them
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject enemy in enemys)
            {
                if (enemy == null)
                    continue;

                enemyScript enemyInstance = enemy.GetComponent<enemyScript>();
                enemyInstance.pauseGame();
            }

            // Loop through all towers and disable them
            //TODO
        }
    }

    public int getWaveNumber()
    {
        return waveNumber;
    }

    public void setWaveNumber(int newWaveNumber)
    {
        waveNumber = newWaveNumber;
        updateWaveNumber = true;
    }

    public void subtractHealth(int value)
    {
        healthValue -= value;
        updateHealthValue = true;
    }

    public void setHealthValue(int health)
    {
        if (health > 0)
        {
            healthValue = health;
            updateHealthValue = true;
        }
    }

    public void setWaveTimer(float newTimer)
    {
        if (newTimer > 0)
        {
            Debug.Log("waveTimerValue=" + waveTimerValue);
            waveTimerValue = newTimer;
            updateWaveTimer = true;
        }
    }
}
