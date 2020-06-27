using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class towerMenuScript : MonoBehaviour
{
    GameObject upgradeButton, sellButton, exitButton;
    Image backgroundImage, poweredIconImage;
    Text towerName, levelLabel, damageLabel, speedLabel, rangeLabel, upgradeCostLabel;
    Text levelValue, damageValue, speedValue, rangeValue;
    Text levelValueUpgrade, damageValueUpgrade, speedValueUpgrade, rangeValueUpgrade, upgradeCostValue;
    Text damageValuePowered, speedValuePowered, rangeValuePowered;

    public bool menuVisible = false;
    towerScript towerObj;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage = gameObject.GetComponent<Image>();

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "upgradeButton")
            {
                upgradeButton = child.gameObject;
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
            else if (child.name == "towerName")             { towerName = child.gameObject.GetComponent<Text>();            }
            else if (child.name == "damageLabel")           { damageLabel = child.gameObject.GetComponent<Text>();          }
            else if (child.name == "speedLabel")            { speedLabel = child.gameObject.GetComponent<Text>();           }
            else if (child.name == "rangeLabel")            { rangeLabel = child.gameObject.GetComponent<Text>();           }
            else if (child.name == "levelLabel")            { levelLabel = child.gameObject.GetComponent<Text>();           }
            else if (child.name == "upgradeCostLabel")      { upgradeCostLabel = child.gameObject.GetComponent<Text>();     }
            else if (child.name == "damageValue")           { damageValue = child.gameObject.GetComponent<Text>();          }
            else if (child.name == "speedValue")            { speedValue = child.gameObject.GetComponent<Text>();           }
            else if (child.name == "rangeValue")            { rangeValue = child.gameObject.GetComponent<Text>();           }
            else if (child.name == "levelValue")            { levelValue = child.gameObject.GetComponent<Text>();           }
            else if (child.name == "levelValueUpgrade")     { levelValueUpgrade = child.gameObject.GetComponent<Text>();    }
            else if (child.name == "damageValueUpgrade")    { damageValueUpgrade = child.gameObject.GetComponent<Text>();   }
            else if (child.name == "speedValueUpgrade")     { speedValueUpgrade = child.gameObject.GetComponent<Text>();    }
            else if (child.name == "rangeValueUpgrade")     { rangeValueUpgrade = child.gameObject.GetComponent<Text>();    }
            else if (child.name == "upgradeCostValue")      { upgradeCostValue = child.gameObject.GetComponent<Text>();     }
            else if (child.name == "damageValuePowered")    { damageValuePowered = child.gameObject.GetComponent<Text>();   }
            else if (child.name == "speedValuePowered")     { speedValuePowered = child.gameObject.GetComponent<Text>();    }
            else if (child.name == "rangeValuePowered")     { rangeValuePowered = child.gameObject.GetComponent<Text>();    }
            else if (child.name == "poweredIcon")           { poweredIconImage = child.gameObject.GetComponent<Image>();    }
        }
        
        hideMenu();
    }

    public void showMenu()
    {
        // Buttons
        if (towerObj == null)
        {
            return;
        }

        // Only show upgrade button if not at max level
        if (towerObj.towerLevel < towerObj.towerLevelMax)
        {
            upgradeButton.GetComponent<Button>().enabled = true;
            upgradeButton.GetComponent<Image>().enabled = true;
            upgradeButton.GetComponentInChildren<Text>().enabled = true;
        }
        sellButton.GetComponent<Button>().enabled = true;
        sellButton.GetComponent<Image>().enabled = true;
        sellButton.GetComponentInChildren<Text>().enabled = true;
        exitButton.GetComponent<Button>().enabled = true;
        exitButton.GetComponent<Image>().enabled = true;

        // Text
        towerName.enabled = true;
        levelLabel.enabled = true;
        levelValue.enabled = true;
        levelValueUpgrade.enabled = true;
        damageLabel.enabled = true;
        damageValue.enabled = true;
        speedLabel.enabled = true;
        speedValue.enabled = true;
        rangeLabel.enabled = true;
        rangeValue.enabled = true;
        upgradeCostLabel.enabled = true;
        upgradeCostValue.enabled = true;
        if (towerObj.towerPowered)
        {
            damageValuePowered.enabled = true;
            speedValuePowered.enabled = true;
            rangeValuePowered.enabled = true;
        }

        // Images
        backgroundImage.enabled = true;
        if (towerObj.towerPowered)
            poweredIconImage.enabled = true;

        menuVisible = true;

        upgradeButtonUnfocus();
    }

    public void hideMenu()
    {
        // Buttons
        upgradeButton.GetComponent<Button>().enabled = false;
        upgradeButton.GetComponent<Image>().enabled = false;
        upgradeButton.GetComponentInChildren<Text>().enabled = false;
        sellButton.GetComponent<Button>().enabled = false;
        sellButton.GetComponent<Image>().enabled = false;
        sellButton.GetComponentInChildren<Text>().enabled = false;
        exitButton.GetComponent<Button>().enabled = false;
        exitButton.GetComponent<Image>().enabled = false;
        // Text
        towerName.enabled = false;
        levelLabel.enabled = false;
        levelValue.enabled = false;
        levelValueUpgrade.enabled = false;
        damageLabel.enabled = false;
        damageValue.enabled = false;
        damageValueUpgrade.enabled = false;
        speedLabel.enabled = false;
        speedValue.enabled = false;
        speedValueUpgrade.enabled = false;
        rangeLabel.enabled = false;
        rangeValue.enabled = false;
        rangeValueUpgrade.enabled = false;
        upgradeCostLabel.enabled = false;
        upgradeCostValue.enabled = false;
        damageValuePowered.enabled = false;
        speedValuePowered.enabled = false;
        rangeValuePowered.enabled = false;
        // Images
        backgroundImage.enabled = false;
        poweredIconImage.enabled = false;

        menuVisible = false;
    }

    public void UpdateTowerData(towerScript towerObj)
    {
        this.towerObj = towerObj;

        // Update text values with data from tower object
        towerName.text = towerObj.towerName;
        damageValue.text = towerObj.towerDamage.ToString();        
        speedValue.text = towerObj.towerSpeed.ToString();        
        rangeValue.text = towerObj.towerRange.ToString();
        levelValue.text = towerObj.towerLevel.ToString();

        if (towerObj.towerPowered)
        {
            damageValuePowered.text = "+" + towerObj.towerPoweredDamageBoost.ToString();
            speedValuePowered.text  = "-" + towerObj.towerPoweredSpeedBoost.ToString();
            rangeValuePowered.text  = "---";
        }

        upgradeCostValue.text = towerObj.towerUpgradeCost.ToString();
    }

    public void upgradeButtonFocus()
    {
        // Enable upgrade text fields
        levelValueUpgrade.enabled = true;
        damageValueUpgrade.enabled = true;
        damageValueUpgrade.text = "+" + towerObj.towerDamageUpgrade.ToString();
        speedValueUpgrade.enabled = true;
        speedValueUpgrade.text = "-" + towerObj.towerSpeedUpgrade.ToString();
        rangeValueUpgrade.enabled = true;
        rangeValueUpgrade.text = "+" + towerObj.towerRangeUpgrade.ToString();
    }

    public void upgradeButtonUnfocus()
    {
        // Disable upgrade text fields
        levelValueUpgrade.enabled = false;
        damageValueUpgrade.enabled = false;
        speedValueUpgrade.enabled = false;
        rangeValueUpgrade.enabled = false;
    }

    public void upgradeButtonPressed()
    {
        //Debug.Log("Upgrade button pushed");

        if (towerObj != null)
        {
            // Only upgrade tower if we have enough coin
            if (GameObject.Find("coinManager").GetComponent<coinManagerScript>().canBuild(towerObj.towerUpgradeCost))
            {
                towerObj.upgradeTower();

                // Hide tower menu
                hideMenu();
                // Remove range marker
                towerObj.towerRangeMarkerVisible = false;
            }
        }
    }

    void removeButtonActionHandler()
    {
        //Debug.Log("Remove button pushed");

        if (towerObj != null)
        {
            // Increase gold value by some determined amount, half tower amount?
            GameObject.Find("coinManager").GetComponent<coinManagerScript>().addCoin(towerObj.towerCost / 2);

            //Debug.Log("Calling destroy");
            towerObj.destroyTower();

            // Hide tower menu
            hideMenu();
        }
    }

    void exitButtonActionHandler()
    {
        if (towerObj != null)
            towerObj.towerRangeMarkerVisible = false;

        // Hide tower menu
        hideMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
