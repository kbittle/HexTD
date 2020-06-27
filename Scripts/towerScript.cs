using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class CustomTile : Tile
{
    private float tileOffsetX = 0f;
    private float tileOffsetY = 0f;
    private bool invalidColorFlag = false;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        //tileData.color = Color.blue;

        if (invalidColorFlag)
        {
            Debug.Log("Get red color");
            tileData.color = Color.red;
        }

        // This disabled the color feature...
        //tileData.flags = TileFlags.LockTransform;

        // Tile transform at runtime
        Vector3 translation;
        translation.x = tileData.transform.m03 + tileOffsetX;
        translation.y = tileData.transform.m13 + tileOffsetY;
        translation.z = tileData.transform.m23;
        tileData.transform = Matrix4x4.TRS(translation, tileData.transform.rotation, Vector3.one);

        //Debug.Log("translation.y=" + translation.y);
    }

    //[CreateTileFromPalette]
    public static TileBase CreateCustomTile(Sprite sprite, float offsetX=0f, float offsetY=0f, bool colorFlag=false)
    {
        var customTile = ScriptableObject.CreateInstance<CustomTile>();
        customTile.sprite = sprite;
        customTile.name = sprite.name;
        customTile.tileOffsetX = offsetX;
        customTile.tileOffsetY = offsetY;
        customTile.invalidColorFlag = colorFlag;

        return customTile;
    }
}

public class towerScript : MonoBehaviour
{
    public Sprite tileSprite;
    public Sprite towerSprite;
    public Sprite[] towerSpriteUpgrades;
    public GameObject projectileGameObject;

    // Flag to disable all tower function
    public bool towerActive = false;
    public bool towerRangeMarkerVisible = true;
    public bool towerPlaced = false;

    // Tower attributes
    public string towerName = "Test Tower";
    public int towerCost = 300;
    public int towerUpgradeCost = 100;
    public float towerRange = 1.5f;
    public float towerRangeUpgrade = 0.2f;
    public float towerDamage = 0.5f;
    public float towerDamageUpgrade = 0.1f;
    public float towerSpeed = 0.5f;
    public float towerSpeedUpgrade = 0.1f;
    public int towerLevel = 1;
    public int towerLevelMax = 3;

    public Tilemap towerLevelMap;
    public Vector3Int currentCell;
    public Vector3 centerPos; // Center of cell
    
    // Range circle vars
    int segments = 30;
    public float xoffset = 5;
    public float yoffset = 5;
    LineRenderer line;

    GameObject[] enemys;
    projectileScript projectileInstance;
    float towerFireTimer = 0.5f;
    public bool projectileAlternating;
    int alternateCount = 0;
    int shotsFiredPerCycle = 0;

    // Tower building vars
    public bool towerBuildComplete = false;
    public float timeToBuildTower = 4f;
    float towerBuildTimer = 0f;
    Image buildProgressBar; 

    // Projectile start psoition tuning vars
    public float projectileXoffset = 0f;
    public float projectileYoffset = 0f;

    // Tower power vars
    public bool towerPowered = false;
    public float towerPowerConsumtionStatic = 0.5f;
    public float towerPowerConsumtionPerShot = 0.01f;
    public float towerPoweredSpeedBoost = 0.1f;
    public float towerPoweredDamageBoost = 0.5f;

    Button yourButton;
    TileBase prevTile;
    Sprite prevTileSprite;
    TileBase newTowerTile;
    
    void Awake()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            //Debug.Log(child.name);
            if (child.name == "progressBar")
            {
                buildProgressBar = child.gameObject.GetComponent<Image>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Disbale sprite from rendering
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void init(Vector3 cursorPosition)
    {
        // Enable all functionality for tower
        towerActive = true;
        towerRangeMarkerVisible = true;

        currentCell = towerLevelMap.WorldToCell(cursorPosition);
        centerPos = towerLevelMap.GetCellCenterLocal(currentCell);

        // Save previous tile, just incase
        prevTile = towerLevelMap.GetTile(currentCell);
        prevTileSprite = towerLevelMap.GetSprite(currentCell);

        // Create new tile for sprite
        newTowerTile = CustomTile.CreateCustomTile(tileSprite, 0, 0);
        towerLevelMap.SetTile(currentCell, newTowerTile);

        // Draw red range marker
        if (towerRangeMarkerVisible)
        {
            DrawCircle();
        }
    }

    public void moveTowerPosition(Vector3 newCursorPosition)
    {
        //Debug.Log("moveTowerPosition");

        // Set prevtile to its original state
        if (towerActive)
        {
            if (prevTile != null)
            {
                // Debug.Log(prevTile.name);
                if (prevTileSprite.name != null)
                {
                    prevTile = CustomTile.CreateCustomTile(prevTileSprite, 0f, 0f);
                }
                else
                {
                    // Unities tilebase is such a POS, have to create a new tile of the old tiles existing sprite for the hex pattern to match up.
                    prevTile = CustomTile.CreateCustomTile(prevTileSprite);
                }
            }
            towerLevelMap.SetTile(currentCell, prevTile);
        }
        // Update the new current cell position vars
        currentCell = towerLevelMap.WorldToCell(newCursorPosition);
        centerPos = towerLevelMap.GetCellCenterLocal(currentCell);
        //Debug.Log("CenterPos=" + centerPos);

        // save prev tile
        prevTile = towerLevelMap.GetTile(currentCell);
        prevTileSprite = towerLevelMap.GetSprite(currentCell);

        // Only set tower tile if a tower does not already exist there
        if (prevTileSprite == null)
        {
            towerLevelMap.SetTile(currentCell, newTowerTile);
        }

        // Draw red range marker
        if (towerRangeMarkerVisible)
        {
            DrawCircle();
        }
    }

    /*
     * Function to handle final checks for building a tower.
     * 
     * return: true if tower can be placed, false otherwise
     */
    public bool placeTowerFinal()
    {
        // Check to see if selected cell has a tower already built at that location
        GameObject[] towerList = GameObject.FindGameObjectsWithTag("tower");
        foreach (GameObject towerObj in towerList)
        {
            towerScript towerObjCode = towerObj.GetComponent<towerScript>();
            if ((towerObjCode.currentCell == currentCell) && (towerObjCode.towerPlaced))
            {
                // Replace existing tower tile
                //Debug.Log(prevTile.name);
                prevTile = CustomTile.CreateCustomTile(prevTileSprite);
                towerLevelMap.SetTile(currentCell, prevTile);
                return false;
            }
        }

        // Check to see if selected cell has a power station already built at that location
        GameObject[] stationList = GameObject.FindGameObjectsWithTag("station");
        foreach (GameObject stationObj in stationList)
        {
            powerStationScript stationObjCode = stationObj.GetComponent<powerStationScript>();
            if ((stationObjCode.currentCell == currentCell) && (stationObjCode.stationPlaced))
            {
                // Replace existing tower tile
                //Debug.Log(prevTile.name);
                prevTile = CustomTile.CreateCustomTile(prevTileSprite);
                towerLevelMap.SetTile(currentCell, prevTile);
                return false;
            }
        }

        // Only place tower if we have the coin
        if (!GameObject.Find("coinManager").GetComponent<coinManagerScript>().canBuild(towerCost))
            return false;

        // Flag that the tower has been placed
        towerPlaced = true;

        // remove the range marker
        towerRangeMarkerVisible = false;

        // Start the build anim
        if (buildProgressBar != null)
        {
            //Debug.Log("Start build");
            towerBuildTimer = timeToBuildTower;

            buildProgressBar.enabled = true;
            buildProgressBar.fillAmount = 0f;                
        }

        // Tell path generator to update path if the tower is placed on the path
        GameObject.Find("pathGenerator").GetComponent<pathGeneratorScript>().newTowerLocation(currentCell);

        // Tell any power stations within range to update/redraw their power lines
        stationList = GameObject.FindGameObjectsWithTag("station");
        foreach (GameObject stationObj in stationList)
        {
            powerStationScript stationObjCode = stationObj.GetComponent<powerStationScript>();
            if (stationObjCode.stationPlaced)
            {
                // If the tower is within range
                float distanceToEnemy = Vector2.Distance(centerPos, stationObjCode.centerPos);
                if (distanceToEnemy < stationObjCode.range)
                {
                    stationObjCode.setupPowerStation();
                }
            }
        }

        // Place tower sprite onto tower tile
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = towerSprite;
        gameObject.transform.position = centerPos;

        return true;
        
    }

    void DrawCircle()
    {
        float x;
        float y;
        float angle = 0f;

        line = gameObject.GetComponent<LineRenderer>();
        //line.SetVertexCount(segments + 1); Obsolete
        line.positionCount = segments + 1;
        //line.SetWidth(0.02F, 0.02F); Obsolete
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        //line.useWorldSpace = false;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * (towerRange);
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * (towerRange);

            x += centerPos.x + xoffset;
            y += centerPos.y + yoffset;

            line.SetPosition(i, new Vector3(x, y, 0));

            // Skipping X deg for tower space
            angle += (360f / segments);
        }
        //Debug.Log(string.Format("Placing ring around [X: {0} Y: {0}]", centerPos.x, centerPos.y));
    }

    public void upgradeTower()
    {
        towerDamage += towerDamageUpgrade;
        towerRange += towerRangeUpgrade;
        towerSpeed -= towerSpeedUpgrade;

        towerLevel += 1;
        // Upgrade tower tilebase sprites here

        if ((towerName == "Red Tower") ||
            (towerName == "Yellow Tower") ||
            (towerName == "Teal Tower") ||
            (towerName == "Light Blue Tower") ||
            (towerName == "Blue Tower"))
        {
            if (towerLevel > towerSpriteUpgrades.Length)
                gameObject.GetComponent<SpriteRenderer>().sprite = towerSpriteUpgrades[towerSpriteUpgrades.Length - 1];
            else
                gameObject.GetComponent<SpriteRenderer>().sprite = towerSpriteUpgrades[towerLevel-2]; // Sub 1 for base tower
        }
    }

    public void setPowerState(bool newPowerState)
    {
        towerPowered = newPowerState;
    }

    public float getPowerConsumed()
    {
        // Calculate power consumtion for this tower
        float powerConsumptionThisCycle = (towerPowerConsumtionStatic + (towerPowerConsumtionPerShot * shotsFiredPerCycle));
        //Debug.Log("Power consumption=" + powerConsumptionThisCycle + ", shots fired=" + shotsFiredPerCycle);

        // Reset shot counter
        shotsFiredPerCycle = 0;
        
        // Return calculated value
        return powerConsumptionThisCycle;
    }

    public void destroyTower(bool clearTile=true)
    {
        //Debug.Log("Destroying tower");

        if (clearTile)
            towerLevelMap.SetTile(currentCell, null);

        Destroy(gameObject);
    }

    void handleTowerBuildingLogic(float timeDelta)
    {
        if (!towerBuildComplete && towerPlaced)
        {
            towerBuildTimer -= timeDelta;

            if (buildProgressBar != null)
                buildProgressBar.fillAmount = towerBuildTimer / timeToBuildTower;

            if (towerBuildTimer <= 0)
            {
                if (buildProgressBar != null)
                    buildProgressBar.enabled = false;

                towerBuildComplete = true;
            }
        }
    }

    void changeTowerDirectionLogic(GameObject target)
    {
        Vector2 targetPos = target.transform.position - gameObject.transform.position;
        
        gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90);
    }

    void handleShootingLogic(float timeDelta)
    {
        bool enemyInRange = false;
        GameObject targetedEnemy = null;
        GameObject closestEnemy = null;
        float closestDistance = 100f;
        float projectileDamage = 0;

        if (towerBuildComplete && towerPlaced)
        {
            // Search through all the enemies to find one in range
            enemys = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject enemy in enemys)
            {
                if (enemy == null)
                    continue;

                // See if enemy is allowed to be targeted  
                enemyScript test = enemy.GetComponent<enemyScript>();
                if (test == null)
                    continue;
                if (test.canTarget() == false)
                    continue;

                float distanceToEnemy = Vector2.Distance(centerPos, enemy.transform.position);
                // If enemy is within shooting distance
                if (distanceToEnemy < towerRange)
                {
                    enemyInRange = true;
                    targetedEnemy = enemy;
                    break;
                }
                else if (distanceToEnemy < closestDistance)
                {
                    // Determine closest enemy that is not within shooting distance
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy;
                }
            }

            // If there is an enemy out there, point tower at it
            if (targetedEnemy != null)
                changeTowerDirectionLogic(targetedEnemy);
            else if (closestEnemy != null)
                changeTowerDirectionLogic(closestEnemy);

            if (enemyInRange)
            {
                towerFireTimer -= timeDelta;
                if (towerFireTimer <= 0)
                {
                    // shoot
                    GameObject projectile = (GameObject)Instantiate(projectileGameObject);
                    projectile.GetComponent<Renderer>().enabled = true;
                    projectileInstance = projectile.GetComponent<projectileScript>();

                    // Calc starting pos
                    Vector3 startingProjectilePos = centerPos;
                    startingProjectilePos.x += projectileXoffset;
                    startingProjectilePos.y += projectileYoffset;

                    // Calc damage, including powered up bonuses
                    projectileDamage = towerDamage + (towerPowered ? towerPoweredDamageBoost : 0);

                    // Init projectile
                    projectileInstance.init(startingProjectilePos, targetedEnemy, projectileDamage, alternateCount==1);

                    // Tells projectile to flip image, simulating projectile comming from 2 diff barrels
                    if (projectileAlternating)
                    {
                        alternateCount = (alternateCount + 1) & 1;
                    }

                    // reset timer
                    towerFireTimer = towerSpeed - (towerPowered ? towerPoweredSpeedBoost : 0);

                    // Increment shot counter use for power consumption
                    if (towerPowered)
                        shotsFiredPerCycle++;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Dont do anything if the tower is not active
        if (!towerActive)
            return;

        // Update the range marker visibility
        if (towerRangeMarkerVisible)
        {
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }

        // Handle game logic below
        handleTowerBuildingLogic(Time.deltaTime);
        handleShootingLogic(Time.deltaTime);
    }
}
