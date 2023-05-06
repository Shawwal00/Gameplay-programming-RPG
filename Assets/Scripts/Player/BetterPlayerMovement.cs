using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class BetterPlayerMovement : MonoBehaviour
{
    OwnPlayer controls;
    private Camera mainCam;
    [HideInInspector] public float health;
    
    // Movement
    private Vector2 move;
    private float speed = 10;
    private float acceleration = 1;
    private float playerRotation;
    private Rigidbody playerRB;
    Quaternion endRotation;
    private float movementBlend = 0;
    
    //Jump
    private float jump; 
    private float jumpDistance = 10;
    private bool jumping = false;
    private int jumpMax = 1;
    private float jumpCooldown;
    private int currentJump = 1;
    private bool jumpButton = true;
    private bool grounded = true;
    private float fallingDown = 5;

    //Interact
    [HideInInspector] public float action;
    private bool movementPU;
    private float movementPUTimer;

    //Camera
    private Vector2 cameraVector;
    private Camera followCamera;
    private Vector3 cameraDistance;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private float cameraRotation;
    private float offsetZ;
    private float offsetX;
    private float offsetY;
    private float rotationSpeed;
    private float joystickGap;
    private Vector3 xAxis;
    private Camera splineCamera;
    private float LockOnCam;
    private GameObject enemy;
    private bool activateLockOnCamera = false;
    private List <GameObject> allEnemies;
    private GameObject[] enemyArray;
    private int currentEnemy = 0;
    private bool LockOnZero = false;

    //Combat
    private float attacking;
   [HideInInspector] public bool doAttack;
    private float attackTimer;
    private float attackCooldown;
    [SerializeField] public GameObject spawn;
    
    private Animator playerAnimator;

    //0 Idle
    //1 Jumping
    //2 Die
    //3 Attack
    //4 Interact
    //5 Running
    //6 Damaged
    //7 Double Jump

    private void Awake()
    {
        allEnemies = new List<GameObject>();
        
        health = 100;
        
        attackCooldown = 0.6f;
        attackTimer = attackCooldown;

        splineCamera = GameObject.Find("SplineCamera").GetComponent<Camera>();
        mainCam = Camera.main;
        playerAnimator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody>();
        
        followCamera = Camera.main;
        offsetZ = -7;
        offsetY = 5;
        offsetX = 0;
        rotationSpeed = 70;
        joystickGap = 0.45f;

        //Using Unity's new Input System
        controls = new OwnPlayer();
        
        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

        controls.Player.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        controls.Player.Jump.canceled += ctx => jump = 0.0f;

        controls.Player.ActionInteract.performed += ctx => action = ctx.ReadValue<float>();
        controls.Player.ActionInteract.canceled += ctx => action = 0.0f;

        controls.Player.CameraMove.performed += ctx => cameraVector = ctx.ReadValue<Vector2>();
        controls.Player.CameraMove.canceled += ctx => cameraVector = Vector2.zero;
        
        controls.Player.Attack.performed += ctx => attacking = ctx.ReadValue<float>();
        controls.Player.Attack.canceled += ctx => attacking = 0.0f;
        
        controls.Player.LockOnCamera.performed += ctx => LockOnCam = ctx.ReadValue<float>();
        controls.Player.LockOnCamera.canceled += ctx => LockOnCam = 0.0f;
        
    }

    private void Start()
    {
        cameraDistance = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z + offsetZ);
    }

    private void FixedUpdate()
    {
        if (mainCam.enabled == true)
        {
            PlayerControlsUpdate();
            CameraFollow();
            PowerUps();
            Jumping();
            Attacking();
            Death();
        }

        else if (splineCamera.enabled)
        {
            PowerUps();
            Jumping();
            Attacking();
            PlayerControlsUpdate();
        }
    }

    private void Attacking()
    {
        if (attacking > 0 && doAttack == false)
        {
            doAttack = true;
        }

        if (doAttack == true)
        {
            attackTimer -= Time.deltaTime;
            playerAnimator.SetInteger("CurrentState", 3);
            if (attackTimer < 0)
            {
                doAttack = false;
                attackTimer = attackCooldown;
            }
        }
    }

    private void Death()
    {
        if (health <= 0)
        {
            transform.position = spawn.transform.position;
            health = 100;
        }
    }

    private void Jumping()
    {
        if (grounded == true && playerRB.velocity.y <= 0)
        {
            playerRB.velocity = new Vector3(playerRB.velocity.x, -fallingDown, 0);
        }

        if (currentJump == 0)
        {
            jumping = false;
        }

        if (jump == 1 && jumpButton == true)
        {
            if (currentJump > 0)
            {
                jumping = true;
            }

            jumpButton = false;
        }
        else if (jump == 0)
        {
            jumpButton = true;
            jumping = false;
        }
        
        if (jumping == true )
        {
            if (jumpMax == 1)
            {
                playerRB.velocity = new Vector3(playerRB.velocity.x, jumpDistance, 0);
                playerAnimator.SetInteger("CurrentState", 1);
                currentJump = 0;
                jumping = false;
            }
            else if (jumpMax == 2)
            {
                if (currentJump == 2)
                {
                    playerAnimator.SetInteger("CurrentState", 1);
                } 
                else if (currentJump == 1)
                {
                    playerAnimator.SetInteger("CurrentState", 7);
                }
                jumping = false;
                currentJump -= 1;
                playerRB.velocity = new Vector3(playerRB.velocity.x, jumpDistance, 0);
            }
        }
        
        if (grounded == false && jump == 0) 
        {
            currentJump = jumpMax;
        }
    }

    private void PowerUps()
    {
        if (action > 0)
        {
            //Debug.Log("Playing");
            playerAnimator.SetInteger("CurrentState", 4); 
        }

        if (movementPU == true)
        {
            movementPUTimer += Time.deltaTime;
            if (movementPUTimer > 5)
            {
                speed = speed/2;
                movementPU = false;
            }
        }
    }

    private void CameraFollow()
    {
        //Following Player
        xAxis = followCamera.transform.TransformDirection(cameraVector.y, 0.0f, 0.0f);
        
        cameraDistance = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 
            transform.position.z + offsetZ);
        followCamera.transform.position = cameraDistance;
      

        //Rotation
        if (cameraVector.x < -joystickGap || cameraVector.x > joystickGap)
        {
            followCamera.transform.RotateAround(transform.position,
                new Vector3(0.0f, cameraVector.x, 0.0f), rotationSpeed * Time.deltaTime);
        }
        
         if (-cameraVector.y < -joystickGap || -cameraVector.y > joystickGap)
         {
             if (followCamera.transform.eulerAngles.x > 5 &&
                 followCamera.transform.eulerAngles.x < 45)
             {
                 followCamera.transform.RotateAround(transform.position,
                     -xAxis, rotationSpeed * Time.deltaTime);
             }
             // Going Down
             else if (followCamera.transform.eulerAngles.x < 5 && -cameraVector.y > 0)
             {
                 followCamera.transform.RotateAround(transform.position,
                    -xAxis, rotationSpeed * Time.deltaTime);
             }
             //Going Up
             else if (followCamera.transform.eulerAngles.x > 45 && -cameraVector.y < 0)
             {
                 followCamera.transform.RotateAround(transform.position,
                    -xAxis, rotationSpeed * Time.deltaTime);
             }
         }

        offsetZ = followCamera.transform.position.z - transform.position.z;
        offsetX = followCamera.transform.position.x - transform.position.x;
        offsetY = followCamera.transform.position.y - transform.position.y;
        
        //Lock On Camera
        
        enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemyArray)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < 30)
            {
                allEnemies.Add(enemy);
            }
            else if (Vector3.Distance(enemy.transform.position, transform.position) > 30)
            {
                if (allEnemies.Contains(enemy))
                {
                    Debug.Log("working");
                    allEnemies.Remove(enemy);
                    activateLockOnCamera = false;
                }
            }
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i] == null)
            {
                allEnemies.Remove(allEnemies[i]);
            }
        }

        if (allEnemies.Count > 0)
        {
            if (LockOnCam == 0 && activateLockOnCamera == true)
            {
                LockOnZero = true;
            }

            else if (LockOnCam == 0 && activateLockOnCamera == false)
            {
                LockOnZero = false;
            }

            if (LockOnCam > 0 && activateLockOnCamera == false && LockOnZero == false)
            {
                activateLockOnCamera = true;

                if (currentEnemy < allEnemies.Count)
                {
                    currentEnemy += 1;
                }
                else if (currentEnemy == allEnemies.Count)
                {
                    currentEnemy = 0;
                }
            }

            else if (LockOnCam > 0 && activateLockOnCamera == true && LockOnZero == true)
            {
                activateLockOnCamera = false;
            }

            if (activateLockOnCamera == true)
            {
                Debug.DrawLine(allEnemies[currentEnemy].transform.position, transform.position, Color.green, 0.1f);
                followCamera.transform.LookAt(allEnemies[currentEnemy].transform);
            }
        }
    }

    private void PlayerControlsUpdate()
    {
        
        playerAnimator.SetFloat("Blend", movementBlend);
        if (move.magnitude > joystickGap && movementBlend < 1)
        {
            movementBlend += Time.deltaTime * 2;
        }
        else if (move.magnitude < joystickGap && movementBlend > 0)
        {
            movementBlend -= Time.deltaTime * 2;
        }

        // Raycast to see if the player is touching the ground
        Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Vector3 groundRaycast = transform.position + new Vector3(0, 0.5f, 0);

        if (Physics.Raycast(groundRay, out hit))
        {
            if (hit.distance < 1)
            {
                grounded = false;
            }
            else
            {
                grounded = true;
            }
        }
        
        // Joystick gap ensure that the player does not accidentally touch the joystick
        if (move.x < -joystickGap || move.x > joystickGap || move.y < -joystickGap || move.y > joystickGap)
        {
            // Ensuring it takes into account the camera when moving
            cameraForward = followCamera.transform.forward;//y axis
            cameraRight = followCamera.transform.right; //x axis

            cameraForward.y = 0;
            cameraRight.y = 0;

            Vector3 forwardMovement = move.x * cameraRight;
            Vector3 rightMovement = move.y * cameraForward;

            Vector3 moveDirection = forwardMovement + rightMovement;
            playerRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            if (playerRotation == 0)
            {

            }
            else
            {
                if (mainCam.enabled == true)
                {
                    endRotation = Quaternion.Euler(new Vector3(0.0f, playerRotation, 0.0f));
                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, endRotation, 6 * Time.deltaTime); // Ensure this happens
                    //Debug.Log(playerRotation);
                }
                else if (splineCamera.enabled)
                {
                    if (playerRotation < 100 && playerRotation > -120)
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0, 0.0f));
                    }
                   else 
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180, 0.0f));
                    }

                }

                // transform.rotation = Quaternion.Euler(new Vector3(0.0f, playerRotation, 0.0f)); // Lerp this     
            }

            if (grounded == false)
            {
                //Running
                playerAnimator.SetInteger("CurrentState", 0);   
            }
            else
            {
                movementBlend = 0;
            }

            if (mainCam.enabled == true)
            {
                if (acceleration < 1.6)
                {
                    acceleration += Time.deltaTime / 8;
                }

                Vector3 movement =
                    new Vector3(moveDirection.x, 0.0f, moveDirection.z) *
                    (speed * acceleration * Time.deltaTime); // Get current rotation
                transform.Translate(movement, Space.World);
            }
            else if (splineCamera.enabled == true)
            {
                if (acceleration < 1.6)
                {
                    acceleration += Time.deltaTime / 8;
                }
                
                Vector3 movement =
                    new Vector3(0.0f, 0.0f,  moveDirection.z) *
                    (speed * acceleration * Time.deltaTime); // Get current rotation
                transform.Translate(movement, Space.World);
            }
        }
        else 
        {
            //Idle
            playerAnimator.SetInteger("CurrentState", 0);
            acceleration = 1;
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    
    private void OnTriggerStay(Collider other)
    {
        //Checking for Power Ups
        if (other.tag == "JumpPU")
        {
            if (action > 0)
            {
                Destroy(other.gameObject);
                jumpMax = 2;
            }
        }

        if (other.tag == "MovementPU")
        {
            if (action > 0)
            {
                Destroy(other.gameObject);
                speed = speed * 2;
                movementPU = true;
            }
        }
    }
}
