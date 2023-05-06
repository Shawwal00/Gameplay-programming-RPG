using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private BetterPlayerMovement playerScript;
    [SerializeField] public GameObject firstButton;
    private Vector3 openTransform;
    private GameObject player;
    [SerializeField] public float offset;
    private List<GameObject> buttons;
    [HideInInspector] public bool open = false;

    private void Awake()
    {
        buttons = new List<GameObject>();
 
        buttons.Add(firstButton);

        player = GameObject.Find("RPG-Character");
        openTransform = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);
        playerScript = player.GetComponent<BetterPlayerMovement>();
    }

    private void FixedUpdate()
    {
        foreach (var button in buttons)
        {
            if (player.GetComponent<Collider>().bounds.Intersects(button.GetComponent<Collider>().bounds))
            {
                if (playerScript.action > 0)
                {
                    open = true;
                }
            }
            
        }

        if (open == true)
        {
            transform.position = Vector3.Lerp(transform.position, openTransform, 1 * Time.deltaTime);
        }
    }
}
