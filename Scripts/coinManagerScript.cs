using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coinManagerScript : MonoBehaviour
{
    public int startingCoinAmount = 1000;
    private Text coinValueText;
    private int coinAmount = 0;
    private bool refreshCoinText = true;

    // Function that returns true if the requested coin amount is available.
    public bool canBuild(int requestedValue)
    {
        if ((coinAmount - requestedValue) > 0)
        {
            coinAmount -= requestedValue;
            refreshCoinText = true;
            return true;
        }

        return false;
    }

    // Function to add coins to the pot
    public void addCoin(int addValue)
    {
        coinAmount += addValue;
        refreshCoinText = true;
    }

    void Awake()
    {
        // Get access to health bar
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "coinValue")
            {
                coinValueText = child.gameObject.GetComponent<Text>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        coinAmount = startingCoinAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (refreshCoinText)
        {
            coinValueText.text = coinAmount.ToString();
            refreshCoinText = false;
        }
    }
}
