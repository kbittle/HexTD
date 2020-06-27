using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class powerStationScript : MonoBehaviour
{
    public Sprite tileSprite;
    public Tilemap towerLevelMap;
    public Vector3Int currentCell;
    public Vector3 centerPos; // Center of cell
    private TileBase prevTile;
    private Sprite prevTileSprite;
    private Image energyLevelBar;
    private GameObject energyLevel;

    // Flag to disable all tower function
    public bool stationActive = false;
    public bool stationRangeMarkerVisible = true;
    public bool stationPlaced = false;
    public bool stationRunning = false;

    // Range circle vars
    int segments = 30;
    public float xoffset = 5;
    public float yoffset = 5;
    LineRenderer line;

    // Power line vars
    public Color lineColor;
    public Material lineMaterial;
    public float lineWidth = 0.05f;
    public int lineSortOrder = 3;

    // Power Station attributes
    public string name = "Basic Power";
    public int cost = 300;
    public int upgradeCost = 100;
    public float range = 3f;
    public float upgradeRange = 0.2f;
    public float powerGeneratedPerSec = 2.5f;
    public  float powerConsumedPerSec = 0f;
    private float powerStoredCurrent = 5f;
    public float powerStorageMax = 10.0f;

    private class towerObject
    {
        public GameObject tower;
        public GameObject powerLine;
        public bool powerLineDrawn;
    };
    private List<towerObject> towersConnected = new List<towerObject>();

    // Timing vars
    public float powerConsumptionCheckTimeout = 1f;
    private float powerConsumptionCheckTimer = 0f;

    void Awake()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            //Debug.Log(child.name);
            if (child.name == "energylevel")
            {
                energyLevelBar = child.gameObject.GetComponent<Image>();
            }

            if (child.name == "powerCanvas")
            {
                energyLevel = child.gameObject;
            }
        }
    }

    public void init(Vector3 cursorPosition)
    {
        // Enable all functionality for tower
        stationActive = true;
        stationRangeMarkerVisible = true;

        currentCell = towerLevelMap.WorldToCell(cursorPosition);
        centerPos = towerLevelMap.GetCellCenterLocal(currentCell);

        // Save previous tile, just incase
        prevTile = towerLevelMap.GetTile(currentCell);
        prevTileSprite = towerLevelMap.GetSprite(currentCell);

        // Create new tile for sprite
        //newTowerTile = CustomTile.CreateCustomTile(tileSprite, 0, 0);
        towerLevelMap.SetTile(currentCell, CustomTile.CreateCustomTile(tileSprite, 0, 0));

        // Draw red range marker
        if (stationRangeMarkerVisible)
        {
            DrawCircle();
        }
    }

    public void moveBasePosition(Vector3 newCursorPosition)
    {
        //Debug.Log("moveTowerPosition");

        // Set prevtile to its original state
        if (stationActive)
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
            towerLevelMap.SetTile(currentCell, CustomTile.CreateCustomTile(tileSprite));
        }

        // Draw red range marker
        if (stationRangeMarkerVisible)
        {
            DrawCircle();
        }
    }

    public bool placeBaseFinal()
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

        // Do the same check for power stations




        // Only place tower if we have the coin
        Text coinValue = GameObject.Find("coinValue").GetComponent<Text>();
        int coinRemaining = (int.Parse(coinValue.text) - cost);
        if (coinRemaining > 0)
        {
            coinValue.text = coinRemaining.ToString();
        }
        else
        {
            return false;
        }

        // Flag that the tower has been placed
        stationPlaced = true;

        // remove the range marker
        stationRangeMarkerVisible = false;

        // Tell path generator to update path if the tower is placed on the path
        GameObject.Find("pathGenerator").GetComponent<pathGeneratorScript>().newTowerLocation(currentCell);

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
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * (range);
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * (range);

            x += centerPos.x + xoffset;
            y += centerPos.y + yoffset;

            line.SetPosition(i, new Vector3(x, y, 0));

            // Skipping X deg for tower space
            angle += (360f / segments);
        }
        //Debug.Log(string.Format("Placing ring around [X: {0} Y: {0}]", centerPos.x, centerPos.y));
    }

    void drawLine(towerObject selectedTower, Vector3 startPos, Vector3 stopPos)
    {
        //Debug.Log("drawline" + startPos);
        selectedTower.powerLine = new GameObject("Line");
        LineRenderer lineInst = selectedTower.powerLine.AddComponent<LineRenderer>();

        lineInst.positionCount = 2;
        lineInst.startWidth = lineWidth;
        lineInst.endWidth = lineWidth;
        lineInst.enabled = true;
        lineInst.startColor = lineColor;
        lineInst.material = lineMaterial;
        lineInst.sortingOrder = lineSortOrder;
        lineInst.SetPosition(0, startPos);
        lineInst.SetPosition(1, stopPos);
    }

    void powerUpTower(towerObject selectedTower)
    {
        if (!selectedTower.powerLineDrawn)
        {
            // Tell tower that it is powered up
            selectedTower.tower.GetComponent<towerScript>().setPowerState(true);
            // Draw power line to tower
            drawLine(selectedTower, centerPos, selectedTower.tower.GetComponent<towerScript>().centerPos);
            // Record that power line is active
            selectedTower.powerLineDrawn = true;
        }
    }

    void powerDownTower(towerObject selectedTower)
    {
        if (selectedTower.powerLineDrawn)
        {
            // Tell tower that it is NOT powered up
            selectedTower.tower.GetComponent<towerScript>().setPowerState(false);
            // Remove power line graphic
            Destroy(selectedTower.powerLine);
            // Record that power line is not active anymore
            selectedTower.powerLineDrawn = false;
        }
    }

    public void setupPowerStation()
    {
        towerObject newTowerObj;

        // Reset conencted tower list
        //towersConnected.Clear(); //doesnt work yet

        GameObject[] towerList = GameObject.FindGameObjectsWithTag("tower");
        foreach (GameObject towerObj in towerList)
        {
            // If the tower is placed and not already powered
            towerScript towerObjCode = towerObj.GetComponent<towerScript>();
            if (towerObjCode.towerPlaced && !towerObjCode.towerPowered)
            {
                // If the tower is within range
                float distanceToEnemy = Vector2.Distance(centerPos, towerObjCode.centerPos);
                if (distanceToEnemy < range)
                {
                    // Store connected tower
                    newTowerObj = new towerObject();
                    newTowerObj.tower = towerObj;
                    towersConnected.Add(newTowerObj);
                    //Debug.Log("Add(newTowerObj)");
                }
            }
        }
    }

    public void updatePowerStation()
    {
        // new function for towers to call instead of the setup??

        // this should only try to add 1 station at a time, maybe
    }

    public void destroyPowerStation()
    {
        Destroy(gameObject);
    }

    void handlePowerConsumtionLogic()
    {
        // Generate power
        if (powerStoredCurrent <= powerStorageMax)
            powerStoredCurrent += powerGeneratedPerSec;

        // If batteries are connected, expand storage

        // Calculate consumed power
        powerConsumedPerSec = 0;
        foreach (towerObject towerObj in towersConnected)
        {
            // Increment power consumed from tower
            powerConsumedPerSec += towerObj.tower.GetComponent<towerScript>().getPowerConsumed();
        }

        // Consume power
        if ((powerStoredCurrent - powerConsumedPerSec) > 0)
            powerStoredCurrent -= powerConsumedPerSec;
        else
            powerStoredCurrent = 0;

        // Set energy level bar, temp? maybe change speed on a turbine graphic?
        if (energyLevelBar != null)
        {
            if (energyLevel)
                energyLevel.transform.position = centerPos;

            energyLevelBar.enabled = true;
            energyLevelBar.fillAmount = (powerStoredCurrent/ powerStorageMax);
        }

        // Enable / disable towers power state
        if (powerStoredCurrent > 0)
        {
            Debug.Log("setTowerPower(true)");

            foreach (towerObject towerObj in towersConnected)
            {
                powerUpTower(towerObj);
            }
        }
        else if (powerStoredCurrent == 0)
        {
            //foreach (towerObject towerObj in towersConnected)
            //{
            //    powerDownTower(towerObj);
            //}

            Debug.Log("Poping a tower");

            // Power off last tower
            powerDownTower(towersConnected[towersConnected.Count - 1]);
            towersConnected.RemoveAt(towersConnected.Count - 1);
        }
    }

    // Funtion to cleanup local tower vars
    void connectedTowerCleanup()
    {
        for (int i=0;i<towersConnected.Count;i++)
        {
            // If the tower has been destroyed remove it from out list
            if (towersConnected[i].tower == null)
            {
                // Remove powerline
                Destroy(towersConnected[i].powerLine);
                // Remove from list
                towersConnected.RemoveAt(i);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Dont do anything if the tower is not active
        if (!stationActive)
            return;

        // Update the range marker visibility
        if (stationRangeMarkerVisible)
        {
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }

        // Init power station after being placed
        if (stationPlaced && !stationRunning)
        {
            setupPowerStation();
            stationRunning = true;
        }

        // Power consumption logic
        if (stationRunning)
        {
            powerConsumptionCheckTimer += Time.deltaTime;
            if (powerConsumptionCheckTimer >= powerConsumptionCheckTimeout)
            {
                powerConsumptionCheckTimer = 0;

                connectedTowerCleanup();
                handlePowerConsumtionLogic();
            }
        }
    }
}
