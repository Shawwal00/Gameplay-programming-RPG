using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
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
    private NavMeshAgent agent;
    private float scale = 2;
    private Vector3 beforePosition;
    private float attackRange;
    private float attackTimer;
    private float damage = 50;
    private BetterPlayerMovement playerScript;
    private GameObject sword;
    private float hitAmount = 3;
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
        agent = GetComponent<NavMeshAgent>();
        transform.localScale = new Vector3(scale, scale, scale);
        beforePosition = transform.position;
        attackRange = 2;
        playerScript = player.GetComponent<BetterPlayerMovement>();
        sword = GameObject.FindGameObjectWithTag("Sword");
    }

    private void Update()
    {
        if (hitAmount <= 0)
        {
            Destroy(transform.gameObject);
        }

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
        agent.destination = beforePosition;

        if (player.transform.position.x < transform.position.x + XBoundry &&
            player.transform.position.x > transform.position.x - XBoundry &&
            player.transform.position.z < transform.position.z + YBoundry &&
            player.transform.position.z > transform.position.z - YBoundry)
        {
            enemyState = EnemyState.Hunting;
        }
        
    }
    
    private void MoveToPlayer()
    {
        attackTimer = 0;
        if (player.transform.position.x < transform.position.x + attackRange  &&
            player.transform.position.x > transform.position.x - attackRange  &&
            player.transform.position.z < transform.position.z + attackRange  &&
            player.transform.position.z > transform.position.z - attackRange)
        {
            enemyState = EnemyState.Attacking;
        }
        
        else if (player.transform.position.x < transform.position.x + XBoundry &&
            player.transform.position.x > transform.position.x - XBoundry &&
            player.transform.position.z < transform.position.z + YBoundry &&
            player.transform.position.z > transform.position.z - YBoundry)
        {
            agent.destination = player.transform.position;
        }
        else
        {
            enemyState = EnemyState.Passive;
        }
    }

    private void AttackPlayer()
    {
        if (player.transform.position.x < transform.position.x + attackRange  &&
            player.transform.position.x > transform.position.x - attackRange  &&
            player.transform.position.z < transform.position.z + attackRange  &&
            player.transform.position.z > transform.position.z - attackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 0.8)
            {
               //Play a particle effect
               playerScript.health -= damage;
                attackTimer = 0;
            }
        }
        
        else
        {
            enemyState = EnemyState.Hunting;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == sword && playerScript.doAttack)
        {
            // Change this so that it gets if the trigger is pressed
            Debug.Log("Hit");
                hitAmount -= 1;
        }
    }
}
