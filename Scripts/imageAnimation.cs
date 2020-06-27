using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class imageAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public int spritePerFrame = 6;
    public bool loop = true;
    public bool destroyOnEnd = false;
    public bool shrintImageOverTime = false;

    private int index = 0;
    private Image image;
    private int frame = 0;
    private float scaleRate = -0.03f;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!loop && index == sprites.Length) return;
        frame++;
        if (frame < spritePerFrame) return;
        image.sprite = sprites[index];
        frame = 0;
        index++;
        if (shrintImageOverTime)
        {
            transform.localScale += new Vector3(scaleRate, scaleRate, 0);
        }
        if (index >= sprites.Length)
        {
            if (loop) index = 0;
            if (destroyOnEnd) Destroy(gameObject);
        }
    }
}
