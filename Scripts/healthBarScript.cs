using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarScript : MonoBehaviour
{
    //Quaternion initRotation;
    private Image healthBarSlider;

    // Start is called before the first frame update
    void Start()
    {
        //initRotation = transform.rotation;

        healthBarSlider = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set health bar percentage value from 0-1
    public void setHealthBar(float percentage)
    {
        if ((percentage >= 0f) && (percentage <= 1f))
        {
            if (healthBarSlider.fillAmount != percentage)
                healthBarSlider.fillAmount = percentage;
        }
        else
        {
            healthBarSlider.fillAmount = 0f;
        }
    }
}
