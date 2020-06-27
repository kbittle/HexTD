using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    public float projectileSpeed = 0.2f;
    private float projectileDamage = 0;

    GameObject  targetObj;
    Vector3     targetPos;
    enemyScript enemyInstance;

    bool isShooting = false;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //target = GameObject.FindGameObjectWithTag("Enemy");

        // Disable inital rendering, so object doesnt sit in middle of screen.
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void init(Vector3 startPosition, GameObject target, float damage, bool flipImage)
    {
        //Debug.Log(string.Format("Co-ords of arrow target is [X: {0} Y: {0}]", target.transform.position.x, target.transform.position.y));
        // Start pos
        transform.position = startPosition;
        // Target pos
        targetPos = target.transform.position;

        targetObj = target;
        projectileDamage = damage;
        
        // Get access to enemy functions
        enemyInstance = target.GetComponent<enemyScript>();

        if (flipImage)
            gameObject.GetComponent<SpriteRenderer>().flipX = flipImage;

        isShooting = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShooting)
        {
            // Enable rendering
            if (gameObject != null)
            {
                gameObject.GetComponent<Renderer>().enabled = true;
            }

            // Update target pos incase target moves
            if (targetObj != null)
            {
                targetPos = targetObj.transform.position;
            }

            // Compute the next position -- straight flight
            Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, projectileSpeed * Time.deltaTime);

            // Rotate to face the next position, and then move there
            transform.rotation = LookAt2D(nextPos - transform.position);
            transform.position = nextPos;

            // Do something when we reach the target
            if (nextPos == targetPos)
                projectileHit();
        }
    }

    void projectileHit()
    {
        // If the enemy is still alive
        if (enemyInstance != null)
        {
            // Update creep that tower projectile has hit it
            enemyInstance.takeDamage(projectileDamage);
        }

        //Debug.Log("destroy projectile instance");
        Destroy(gameObject);
    }

    /// 
	/// This is a 2D version of Quaternion.LookAt; it returns a quaternion
	/// that makes the local +X axis point in the given forward direction.
	/// 
	/// forward direction
	/// Quaternion that rotates +X to align with forward
	Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - 90);
    }
}
