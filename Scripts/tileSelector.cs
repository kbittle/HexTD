using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
//using UnityEngine.CoreModule;

public class tileSelector : MonoBehaviour
{
    public Tilemap towerLevelMap;

    public GameObject towerMenu;
    public GameObject powerStationMenu;
    towerMenuScript towerMenuInstance;
    powerStationMenuScript stationMenuInstance;
    towerScript currentSelectedTower;

    // Start is called before the first frame update
    void Start()
    {
        towerMenuInstance = towerMenu.GetComponent<towerMenuScript>();
        stationMenuInstance = powerStationMenu.GetComponent<powerStationMenuScript>();
    }

    // Handles logic to launch submenuscript. If the user selects a tile, we check to see 
    // if a tower exists at that tile and enable the menu.
    void Update()
    {
        Vector2 localMousePosition;
        RectTransform rt;

        if (Input.GetMouseButtonDown(0))
        {
            // Hide menu anytime we click elsewhere
            if (towerMenuInstance.menuVisible)
            {
                localMousePosition = towerMenuInstance.transform.InverseTransformPoint(Input.mousePosition);
                rt = towerMenu.GetComponent(typeof(RectTransform)) as RectTransform;
                if (!rt.rect.Contains(localMousePosition))
                {
                    //Debug.Log("Hide sub menu.");
                    towerMenuInstance.hideMenu();

                    if (currentSelectedTower != null)
                    {
                        // Hide range circle
                        currentSelectedTower.towerRangeMarkerVisible = false;

                        currentSelectedTower = null;
                    }
                }

                // Break so we do not relocate the menu until use has clicked elsewhere
                return;
            }

            if (stationMenuInstance.menuVisible)
            {
                localMousePosition = stationMenuInstance.transform.InverseTransformPoint(Input.mousePosition);
                rt = powerStationMenu.GetComponent(typeof(RectTransform)) as RectTransform;
                if (!rt.rect.Contains(localMousePosition))
                {
                    //Debug.Log("Hide sub menu.");
                    stationMenuInstance.hideMenu();

                    //if (currentSelectedTower != null)
                    //{
                        // Hide range circle
                        //currentSelectedTower.towerRangeMarkerVisible = false;

                        //currentSelectedTower = null;
                    //}
                }

                // Break so we do not relocate the menu until use has clicked elsewhere
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 cursonPosition = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int cellSelected = towerLevelMap.WorldToCell(cursonPosition);

            // Check to see if selected cell has a tower built at that location
            GameObject[] towerList = GameObject.FindGameObjectsWithTag("tower");
            foreach (GameObject towerObj in towerList)
            {
                towerScript towerObjCode = towerObj.GetComponent<towerScript>();
                if (towerObjCode == null)
                    continue;

                if (towerObjCode.currentCell == cellSelected) 
                {
                    if (towerObjCode.towerBuildComplete)
                    {
                        //Debug.Log("Show sub menu.");
                        currentSelectedTower = towerObjCode;

                        // Update menu data
                        towerMenuInstance.UpdateTowerData(towerObjCode);                  
                        towerMenuInstance.showMenu();

                        // Move menu to selected tower
                        Vector3 newMenuPos = Camera.main.WorldToScreenPoint(towerLevelMap.GetCellCenterWorld(cellSelected));
                        newMenuPos.x += 120;
                        newMenuPos.y += 40;
                        towerMenuInstance.transform.position = newMenuPos;

                        // Show range circle
                        towerObjCode.towerRangeMarkerVisible = true;
                    }
                }                
            }

            // Check to see if selected cell has a power station built at that location
            GameObject[] stationList = GameObject.FindGameObjectsWithTag("station");
            foreach (GameObject powerStationObj in stationList)
            {
                powerStationScript powerStationObjCode = powerStationObj.GetComponent<powerStationScript>();
                if (powerStationObjCode == null)
                    continue;

                if (powerStationObjCode.currentCell == cellSelected)
                {
                    if (powerStationObjCode.stationRunning)
                    {
                        //Debug.Log("Show sub menu.");
                        //currentSelectedTower = towerObjCode;

                        // Update menu data
                        //towerMenuInstance.UpdateTowerData(towerObjCode);
                        stationMenuInstance.showMenu();

                        // Move menu to selected tower
                        Vector3 newMenuPos = Camera.main.WorldToScreenPoint(towerLevelMap.GetCellCenterWorld(cellSelected));
                        newMenuPos.x += 120;
                        newMenuPos.y += 40;
                        stationMenuInstance.transform.position = newMenuPos;

                        // Show range circle
                        //towerObjCode.towerRangeMarkerVisible = true;
                    }
                }
            }
        }
    }
}
