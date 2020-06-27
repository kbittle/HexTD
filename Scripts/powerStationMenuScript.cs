using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class powerStationMenuScript : MonoBehaviour
{
    GameObject setupButton, sellButton, exitButton;
    Image backgroundImage;
    Text powerStationName, consumptionLabel, generationLabel, storedLabel;
    Text consumptionValue, generationValue, storedValue, storedPercentage;

    public bool menuVisible = false;
    powerStationScript powerObj;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage = gameObject.GetComponent<Image>();

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "setupButton")
            {
                setupButton = child.gameObject;
            }
            else if (child.name == "sellButton")
            {
                sellButton = child.gameObject;
                sellButton.GetComponent<Button>().onClick.AddListener(removeButtonActionHandler);
            }
            else if (child.name == "exitButton")
            {
                exitButton = child.gameObject;
                exitButton.GetComponent<Button>().onClick.AddListener(exitButtonActionHandler);
            }
            else if (child.name == "powerStationName") { powerStationName = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "consumptionLabel") { consumptionLabel = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "consumptionValue") { consumptionValue = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "generatedLabel") { generationLabel = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "generatedValue") { generationValue = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "storedLabel") { storedLabel = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "storedValue") { storedValue = child.gameObject.GetComponent<Text>(); }
            else if (child.name == "storedPercentage") { storedPercentage = child.gameObject.GetComponent<Text>(); }
        }

        hideMenu();
    }

    public void showMenu()
    {
        // Buttons
        setupButton.GetComponent<Button>().enabled = true;
        setupButton.GetComponent<Image>().enabled = true;
        setupButton.GetComponentInChildren<Text>().enabled = true;
        sellButton.GetComponent<Button>().enabled = true;
        sellButton.GetComponent<Image>().enabled = true;
        sellButton.GetComponentInChildren<Text>().enabled = true;
        exitButton.GetComponent<Button>().enabled = true;
        exitButton.GetComponent<Image>().enabled = true;
        // Text
        powerStationName.enabled = true;
        consumptionLabel.enabled = true;
        consumptionValue.enabled = true;
        generationLabel.enabled = true;
        generationValue.enabled = true;
        storedLabel.enabled = true;
        storedValue.enabled = true;
        storedPercentage.enabled = true;
        // Images
        backgroundImage.enabled = true;

        menuVisible = true;
    }

    public void hideMenu()
    {
        // Buttons
        setupButton.GetComponent<Button>().enabled = false;
        setupButton.GetComponent<Image>().enabled = false;
        setupButton.GetComponentInChildren<Text>().enabled = false;
        sellButton.GetComponent<Button>().enabled = false;
        sellButton.GetComponent<Image>().enabled = false;
        sellButton.GetComponentInChildren<Text>().enabled = false;
        exitButton.GetComponent<Button>().enabled = false;
        exitButton.GetComponent<Image>().enabled = false;
        // Text
        powerStationName.enabled = false;
        powerStationName.enabled = false;
        consumptionLabel.enabled = false;
        consumptionValue.enabled = false;
        generationLabel.enabled = false;
        generationValue.enabled = false;
        storedLabel.enabled = false;
        storedValue.enabled = false;
        storedPercentage.enabled = false;
        // Images
        backgroundImage.enabled = false;

        menuVisible = false;
    }

    void removeButtonActionHandler()
    {
        //Debug.Log("Remove button pushed");

        if (powerObj != null)
        {
            // Increase gold value by some determined amount, half tower amount?
            GameObject.Find("coinManager").GetComponent<coinManagerScript>().addCoin(powerObj.cost / 2);

            //Debug.Log("Calling destroy");
            powerObj.destroyPowerStation();

            // Hide tower menu
            hideMenu();
        }
    }

    void exitButtonActionHandler()
    {
        //if (powerObj != null)
        //    powerObj.towerRangeMarkerVisible = false;

        // Hide tower menu
        hideMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
