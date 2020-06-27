using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraEffects : MonoBehaviour
{
    // All code here will be within the class

    /* Variables */
    //public Transform target;
    Vector3 moveToPosition; // This is where the camera will move after the start
    public float speed = 2f; // this is the speed at which the camera moves
    //bool started = false; // stops the movement until we want it

    private Vector3 mouseOrigin;
    private bool isZooming;

    public float zoomSensitivity = 0.002f;
    float lastDistance = 0f;

    public float mouseSensitivity = 0.05f;
    private Vector3 lastPosition;

    /* functions */
    void Start()
    {
        // Since this object as an child the (0, 0, 0) position will be the same as the players. So we can just add to the zero vector and it will be position correctly. 

        //moveToPosition = new Vector3(-10, 10, -0.01f); // 2 meters above/ 0.01 meters behind
                                                    // If you are allowed to rotate your camera the change the -0.01 to 0

        // The following function decides how long to stare at the player before moving
        //LookAtPlayerFor(1f); // waits for 3.5 seconds then starts 
    }

    void zoomIn(float zMovement)
    {
        if (transform.position.z < -2)
            transform.position += new Vector3(0, 0, zMovement);
    }

    void zoomOut(float zMovement)
    {
        if (transform.position.z > -30)
            transform.position -= new Vector3(0, 0, zMovement);
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        /*
        // Scroll wheel zooming
        if (Input.GetMouseButtonDown(0))
        {
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }

        if (!Input.GetMouseButton(0))
            isZooming = false;

        if (isZooming)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 move = pos.y * speed * transform.forward;
            transform.Translate(move, Space.World);
        }
        */

        // Handle 2 touch zooming
        if ((Input.touchCount >= 2) && (Input.GetTouch(0).phase == TouchPhase.Moved) && (Input.GetTouch(1).phase == TouchPhase.Moved))
        {
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);
            //Debug.Log(distance + "," + lastDistance);

            if (lastDistance != 0)
            {
                if ((distance - lastDistance) > 0)
                    zoomIn(distance * zoomSensitivity);
                else
                    zoomOut(distance * zoomSensitivity);
            }

            lastDistance = distance;
        }


        // Handle click and drag
        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
        }


        // +/- zoom key
        if (Input.GetKey(KeyCode.Minus))
        {
            zoomOut(speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            zoomIn(speed * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
        }

        /*
        // so we only want the movement to start when we decide
        //if (!started)
        //    return;

        // Move the camera into position
        transform.position = Vector3.Lerp(transform.position, target.position, speed);

        // Ensure the camera always looks at the player
        transform.LookAt(transform.parent);
        */
    }

    IEnumerator LookAtPlayerFor(float time)
    {
        yield return new WaitForSeconds(time);
        //started = true;
    }
}
