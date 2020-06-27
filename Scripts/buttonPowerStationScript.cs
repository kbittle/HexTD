using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class buttonPowerStationScript : MonoBehaviour, IPointerClickHandler
{
    int n = 0;

    public GameObject powerObject;
    powerStationScript powerStationInstance;
    Button uiButton;

    bool buttonPressed = false;
    Vector3 currentPos;
    Vector3Int prevCell;
    TileBase prevTile;
    //SpriteRenderer renderer;

    void Awake()
    {
        uiButton = gameObject.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        uiButton.onClick.AddListener(buttonActionHandler);
    }

    void handleMoveStationLogic()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 cursonPosition = ray.GetPoint(-ray.origin.z / ray.direction.z);
        //Debug.Log(string.Format("Displaying tower at [X: {0} Y: {0}]", cursonPosition.x, cursonPosition.y));

        // Keep moving tower where the cursor is
        if (currentPos != Input.mousePosition)
        {
            currentPos = Input.mousePosition;

            if (powerStationInstance != null)
                powerStationInstance.moveBasePosition(cursonPosition);
        }
    }

    void handlePlaceStationLogic()
    {
        // Once the user clicks a tile, the tower is final
        if (Input.GetMouseButtonDown(0))
        {        
            // Returns true if the tower can be/is placed
            if (powerStationInstance.placeBaseFinal() == false)
            {
                // Destroy instance of power station
                powerStationInstance.destroyPowerStation();
            }

            // reset tower building button
            unclickButton();
            prevCell = Vector3Int.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(buttonPressed)
        {
            handleMoveStationLogic();

            handlePlaceStationLogic();
        }
    }

    public void buttonActionHandler()
    {
        n++;
        //Debug.Log("Button clicked " + n + " times.");

        if (!buttonPressed)
        {
            buttonPressed = true;
            //uiButton.GetComponentInChildren<Text>().text = "Selected";

            // Grab cursor pos for initial tower
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 cursonPosition = ray.GetPoint(-ray.origin.z / ray.direction.z);

            // Create instance of tower
            GameObject powerStationGeneric = (GameObject)Instantiate(powerObject);
            powerStationInstance = powerStationGeneric.GetComponent<powerStationScript>();
            powerStationInstance.init(cursonPosition);

            resetOtherButtons();
        }
        else
        {
            buttonPressed = false;
        }
    }

    void resetOtherButtons()
    {
        GameObject[] buildButtons = GameObject.FindGameObjectsWithTag("buttons");

        foreach (GameObject buildButton in buildButtons)
        {
            if (buildButton.name == uiButton.name)
            {
                continue;
            }
            else if (buildButton.name.Contains("Tower"))
            {
                //Debug.Log("Unclick button name: " + towerButton.name);
                buttonScript other = buildButton.GetComponent(typeof(buttonScript)) as buttonScript;
                other.unclickButton();
            }
            else if (buildButton.name.Contains("Power"))
            {
                buttonPowerStationScript other = buildButton.GetComponent<buttonPowerStationScript>();
                other.unclickButton();
            }
        }
    }

    public void unclickButton()
    {
        buttonPressed = false;
    }

    public void OnPointerClick(PointerEventData data)
    {

    }
}
