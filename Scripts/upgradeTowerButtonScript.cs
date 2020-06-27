using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class upgradeTowerButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject towerMenuObj;
    towerMenuScript towerMenu;

    void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(upgradeButtonActionHandler);
    }

    // Start is called before the first frame update
    void Start()
    {
        towerMenu = towerMenuObj.GetComponent<towerMenuScript>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        towerMenu.upgradeButtonFocus();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        towerMenu.upgradeButtonUnfocus();
    }

    void upgradeButtonActionHandler()
    {
        towerMenu.upgradeButtonPressed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
