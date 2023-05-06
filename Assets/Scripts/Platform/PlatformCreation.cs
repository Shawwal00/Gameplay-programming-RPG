using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class PlatformCreation : MonoBehaviour
{
    // Platform will fall after being walked on
    [SerializeField] public bool platformFall;
    [SerializeField] public float platformFallTime;
    [SerializeField] public float fallSpeed;
    [SerializeField] public bool destroyPlatformAfterDestroyTime;
    [SerializeField] public float destroyTime;
    private bool fall;
    
    // Platform will be destroyed after being attacked
    [SerializeField] public bool destroyPlatformOnHit;
    [SerializeField] public float destroyTimeAfterHit;
    private bool platformDestroy;
    
    //Platform will move from one location to another
    [SerializeField] public bool movingPlatform;
    [SerializeField] public bool continuous;
    //[SerializeField] public List<GameObject> moveLocations;
    [SerializeField] public GameObject startLocation;
    [SerializeField] public GameObject endLocation;
    [SerializeField] public float moveSpeed;
    private bool continousMovement;
    private bool once;
    private bool onTop;
    private GameObject nextLocation;
    private GameObject newParent;
    
    private GameObject player;
    private GameObject sword;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sword = GameObject.FindGameObjectWithTag("Sword");
        newParent = GameObject.Find("Parent"); // This should ideally be created in the script and deleted

        transform.position = startLocation.transform.position;
    }


    private void FixedUpdate()
    {    
        //Platform Fall Down
        if (fall == true)
        {
            platformFallTime -= Time.deltaTime;
            if (platformFallTime < 0)
            {
                transform.Translate(Vector3.down * (Time.deltaTime * fallSpeed));
            }
            
            destroyTime -= Time.deltaTime;
            if (destroyTime < 0)
            {
                if (destroyPlatformAfterDestroyTime == true)
                {
                    Destroy(gameObject);
                }
            }
        }

        //Destroy Platform after falling
        if (platformDestroy == true)
        {
            destroyTimeAfterHit -= Time.deltaTime;
            if (destroyTimeAfterHit < 0)
            {
                Destroy(gameObject);
            }
        }

        // Moving Platform
        if (movingPlatform == true)
        {

            if (continuous == true)
            {
                

                // Going Forwards
                if (Vector3.Distance(transform.position, endLocation.transform.position) < 1)
                {
                    continousMovement = true;
                }
                
                // Going Backwards
                else if (Vector3.Distance(transform.position, startLocation.transform.position) < 1)
                {
                    continousMovement = false;
                }

                // Going Backwards
                if (continousMovement == true)
                {
                    transform.position = Vector3.Lerp(transform.position, startLocation.transform.position, 
                        moveSpeed * Time.deltaTime);
                }

                // Going Forwards
                else if (continousMovement == false)
                {
                    transform.position = Vector3.Lerp(transform.position, endLocation.transform.position, 
                        moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                //Single Time
                if (once == true)
                {
                    transform.position = Vector3.Lerp(transform.position, endLocation.transform.position,
                        moveSpeed * Time.deltaTime);
                }
            }
        }

        // Making the character stay on top of the platform
        if (onTop == true)
        {
             newParent.transform.SetParent(transform);
             player.transform.SetParent(newParent.transform);
        }
        else
        {
            newParent.transform.parent = null;
            player.transform.parent = null;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (platformFall == true)
        {
            if (collision.gameObject == player)
            {
                if (player.transform.position.y > transform.position.y)
                {
                    fall = true;
                }
            }
        }

        if (collision.gameObject == player)
        {
            if (player.transform.position.y > transform.position.y)
            {
                once = true;
                onTop = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            if (player.transform.position.y > transform.position.y)
            {
                onTop = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (destroyPlatformOnHit == true)
        {

            if (other.gameObject == sword)
            {
                Debug.Log(sword);
                platformDestroy = true;
            }
        }
    }
}

