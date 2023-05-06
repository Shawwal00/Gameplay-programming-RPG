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
    private Rigidbody playerRB;
    private NavMeshAgent agent;
    [SerializeField] public float scale;
    private Vector3 beforePosition;
    private float attackRange;
    private float attackTimer;
    [SerializeField] public float damage;
    private BetterPlayerMovement playerScript;
    private GameObject sword;
    [SerializeField] public float hitAmount;
    private bool takeDamage = false;
    private bool startDamage = false;
    private Material enemyMaterial;
    private Material enemyHitMaterial;
    private MeshRenderer enemyMesh;
    private SkinnedMeshRenderer playerMesh;
    private Material playerMaterial;
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
        beforePosition = transform.position;
        attackRange = 4;
        playerScript = player.GetComponent<BetterPlayerMovement>();
        sword = GameObject.FindGameObjectWithTag("Sword");
        playerRB = player.GetComponent<Rigidbody>();
        enemyHitMaterial = GameObject.Find("OpenDoor").GetComponent<MeshRenderer>().material;
        enemyMaterial = GetComponent<MeshRenderer>().material;
        enemyMesh = GetComponent<MeshRenderer>();
        playerMesh = GameObject.Find("RPG-Character-Mesh").GetComponent<SkinnedMeshRenderer>();
        playerMaterial = playerMesh.material;
    }

    private void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
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

    private void FixedUpdate()
    {
        if (hitAmount <= 0)
        {
            Destroy(transform.gameObject);
        }

        if (startDamage == true)
        {
            StartCoroutine(TakeDamage());
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
        Debug.Log("Working");
        if (player.transform.position.x < transform.position.x + attackRange  &&
            player.transform.position.x > transform.position.x - attackRange  &&
            player.transform.position.z < transform.position.z + attackRange  &&
            player.transform.position.z > transform.position.z - attackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 0.8)
            {
                playerRB.AddForce(transform.forward * 400);
               playerScript.health -= damage;
                attackTimer = 0;
                StartCoroutine(PlayerDamage());
            }
        }
        else
        {
            enemyState = EnemyState.Hunting;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (takeDamage == false)
        {
            if (other.gameObject == sword && playerScript.doAttack)
            {
                startDamage = true;
            }
        }
    }
    
    public void setEnemy(float xBoundry, float yBoundry, float enemyDamage, float health, float size)
    {
       XBoundry = xBoundry;
       YBoundry = yBoundry;
       damage = enemyDamage;
       hitAmount = health;
       scale = size;
    }

    IEnumerator TakeDamage()
    {
        takeDamage = true;
        startDamage = false;
        enemyMesh.material = enemyHitMaterial;
        yield return new WaitForSeconds(0.3f);
        hitAmount -= 1;
        takeDamage = false;
        yield return new WaitForSeconds(0.3f);
        enemyMesh.material = enemyMaterial;
    }

    IEnumerator PlayerDamage()
    {
        playerMesh.material = enemyHitMaterial;
        yield return new WaitForSeconds(0.6f);
        playerMesh.material = playerMaterial;
    }
}
