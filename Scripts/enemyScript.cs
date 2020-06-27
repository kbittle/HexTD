using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class enemyScript : MonoBehaviour
{
    public Tilemap baseLevelMap;
    public float creepMaxHealth = 25.0f;
    private float creepCurrentHealth = 25.0f;
    public float creepSpeed = 1.0f;
    public int creepRewardValue = 100;

    public bool creepCanBeAttacked = false;
    public bool creepblocked = false;
    public bool creepCanMove = false;

    private bool startCreepDestroyCountdown = false;
    private float creepDestroyCounter = 1f;

    public GameObject[] waypoints;
    public GameObject wayPointObj;
    private int currentWaypoint = 0;
    private float lastWaypointSwitchTime;

    private GameObject healthBarObj;
    private healthBarScript hpScript;
    private GameObject currentSpriteObj;
    private GameObject coinAnimObj;

    void Awake()
    {
        //SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();

        // Get access to health bar
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            //Debug.Log(child.name);
            if (child.name == "healthBar")
            {
                healthBarObj = child.gameObject;
                healthBarObj.GetComponent<Image>().enabled = false;
                hpScript = healthBarObj.GetComponent<healthBarScript>();
            }

            if (child.name == "enemySprite")
            {
                //currentSpriteObj = GameObject.Find("enemySprite");
                currentSpriteObj = child.gameObject;
                currentSpriteObj.GetComponent<SpriteRenderer>().enabled = false;
            }

            if (child.name == "coinAnimation")
            {
                coinAnimObj = child.gameObject;
                coinAnimObj.GetComponent<Image>().enabled = false;
            }
        }
            
        /*
        for (int i = 0; i < gameObject.transform.childCount - 1; i++)
        {
            Debug.Log(gameObject.transform.GetChild(i).transform.name);
            if (gameObject.transform.GetChild(i).transform.name == "healthBar")
            {
                gameObject.transform.GetChild(i).transform.GetComponent<Image>().enabled = false;
                hpScript = gameObject.transform.GetChild(i).transform.GetComponent<healthBarScript>();
            }
        }*/

        //healthBarObj = GameObject.Find("healthBar");
        //healthBarObj.GetComponent<Image>().enabled = false;
        //hpScript = healthBarObj.GetComponent(typeof(healthBarScript)) as healthBarScript;

        // Get access to sprite
        //currentSpriteObj = GameObject.Find("enemySprite");
        //currentSpriteObj.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set inital position
        if (waypoints.Length > 1)
            transform.position = waypoints[0].transform.position;
    }

    public void initCreep()
    {
        creepCanBeAttacked = true;
        creepCanMove = true;

        if (currentSpriteObj != null)
            currentSpriteObj.GetComponent<SpriteRenderer>().enabled = true;
        if (healthBarObj != null)
            healthBarObj.GetComponent<Image>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (creepCanMove && (waypoints.Length > 0))
        {
            // Move enemy sprite along waypoints
            Vector3 dir = waypoints[currentWaypoint].transform.position - transform.position;
            transform.Translate(dir.normalized * creepSpeed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) <= 0.4f)
            {
                //Debug.Log("GetNextWayPoint");
                GetNextWayPoint();

                // Rotate to face the next position
                if (currentSpriteObj != null)
                    currentSpriteObj.transform.rotation = LookAt2D(waypoints[currentWaypoint].transform.position - transform.position);
            }
        }

        // Destory enemy object after hp reduced to 0
        if (creepCurrentHealth == 0f && startCreepDestroyCountdown == false)
        {
            // Add coins for killing creep
            GameObject.Find("coinManager").GetComponent<coinManagerScript>().addCoin(creepRewardValue);
            
            // Stop coin animation from moving after sprite disapears
            creepCanMove = false;

            // Start coin animation
            if (coinAnimObj != null)
            {
                coinAnimObj.GetComponent<Image>().enabled = true;
                coinAnimObj.GetComponent<imageAnimation>().shrintImageOverTime = true;
            }

            // Creep is dead, allow animation to finish before destroying game objects
            creepCanBeAttacked = false;

            // Remove sprite from GUI until it is destroyed
            if (currentSpriteObj != null)
                currentSpriteObj.GetComponent<SpriteRenderer>().enabled = false;

            // Start the timer to destroy creep obj
            startCreepDestroyCountdown = true;
        }

        if (startCreepDestroyCountdown)
        {
            // Countdown until gameobj is destroyed
            creepDestroyCounter -= Time.deltaTime;
            if (creepDestroyCounter <= 0)
            {
                destroyCreep();
            }
        }
    }

    void destroyCreep()
    {
        // might need to add lines to destroy children game objects
        for (int i = 0; i < waypoints.Length; i++)
            Destroy(waypoints[i]);

        Destroy(gameObject);
    }

    void GetNextWayPoint()
    {
        if (currentWaypoint >= waypoints.Length - 1)
        {
            // Subtract point value from health
            GameObject.Find("TopRightMenu").GetComponent<topButtonMenuScript>().subtractHealth(1);

            // Destroy creep objects
            destroyCreep();
            return;
        }
        else
        {
            currentWaypoint++;
        }
    }

    /// 
    /// This is a 2D version of Quaternion.LookAt; it returns a quaternion
    /// that makes the local +X axis point in the given forward direction.
    /// 
    /// forward direction
    /// Quaternion that rotates +X to align with forward
    static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - 90);
    }

    public void setWaypoints(Vector3Int[] pathIn)
    {
        float tilebaseXoffset = 0f;//-1.2f;
        float tilebaseYoffset = -0.1f;//-0.5f;
        Vector3 newPoint;

        waypoints = new GameObject[pathIn.Length];

        for(int i=0;i<pathIn.Length;i++)
        {
            newPoint = baseLevelMap.GetCellCenterLocal(pathIn[i]);
            newPoint.x += tilebaseXoffset;
            newPoint.y += tilebaseYoffset;
            waypoints[i] = GameObject.Instantiate(wayPointObj, newPoint, Quaternion.identity) as GameObject;
        }
    }

    public void takeDamage(float damageTaken)
    {
        // Subtract damage taken from total health
        creepCurrentHealth -= damageTaken;
        // Dont drop below 0hp
        if (creepCurrentHealth <= 0)
            creepCurrentHealth = 0f;

        float percentage = creepCurrentHealth / creepMaxHealth;
        // Update GUI
        if (hpScript != null)
            hpScript.setHealthBar(percentage);
    }

    public bool canTarget()
    {
        //Debug.Log("help" + creepCanBeAttacked);
        return creepCanBeAttacked;
    }

    public void pauseGame()
    {
        creepCanMove = false;
    }

    public void resumeGame()
    {
        creepCanMove = true;
    }
}
