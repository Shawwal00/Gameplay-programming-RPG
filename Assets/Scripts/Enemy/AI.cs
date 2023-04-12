using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    // Create a circle where if the player steps into the circle The cube will go to the player, Use the Unity pathfinding 
    // If the square reaches the player then attack based on a parameter
    // If the player runs away then the enemy returns to its location
    
    // Later try to develop my own pathfinding if I have time
    
    // If the Cube is hit 3 times then it breaks into 2 smaller cubes then again with 2 hits then 1 hits
    
    // Add in patrolling with the ability to make multiple points on the map.
    // Add a Lock on for the camera
    // Add multiple cubes that communicate with one another so they do not all attack the player.
    
    // Have the player Be able to be killed and respawn at a location
    // Add visual effects for clear emphasis
    
    
    [SerializeField] public float XBoundry;
    [SerializeField] public float YBoundry;
    private GameObject player;
    private enum EnemyState
    {
        Passive,
        Hunting,
        Attacking,
    }
    EnemyState enemyState;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyState = EnemyState.Passive;
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Passive:
                FollowPath();
                break;
            case EnemyState.Hunting:
                MoveToPlayer();
                break;
            case EnemyState.Attacking:
                AttackPlayer();
                break;
        }
    }

    private void FollowPath()
    {
        if (player.transform.position.x > transform.position.x + XBoundry)
        {
            
        }
    }
    
    private void MoveToPlayer()
    {
        
    }

    private void AttackPlayer()
    {
        
    }
}
