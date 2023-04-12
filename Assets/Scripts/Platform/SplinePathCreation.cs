using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplinePathCreation : MonoBehaviour
{
   [SerializeField] public GameObject start;
   [SerializeField] public GameObject end;
   [SerializeField] public float offset;
   private GameObject player;
   private Camera mainCam;
   private Camera splineCamera;
   

   private void Awake()
   {
      player = GameObject.FindGameObjectWithTag("Player");
      mainCam = Camera.main;
      splineCamera = GameObject.Find("SplineCamera").GetComponent<Camera>();
      splineCamera.enabled = false;
      splineCamera.GetComponent<AudioListener>().enabled = false;
   }

   private void Update()
   {
      CameraSwitch();
   }

   private void CameraSwitch()
   {
      //Z Direction camera switch
      if (player.transform.position.z > start.transform.position.z && player.transform.position.z < end.transform.position.z
          && player.transform.position.x < start.transform.position.x + offset && player.transform.position.x > start.transform.position.x - offset )
      {
         // Change camera
         mainCam.enabled = false;
         splineCamera.enabled = true;
         splineCamera.GetComponent<AudioListener>().enabled = true;
         mainCam.GetComponent<AudioListener>().enabled = false;
      }

      else
      {
         mainCam.enabled = true;
         splineCamera.enabled = false;
         splineCamera.GetComponent<AudioListener>().enabled = false;
         mainCam.GetComponent<AudioListener>().enabled = true;
      }

      if (splineCamera.enabled == true)
      {
         // Set position of 2.5D Camera
         splineCamera.transform.position = new Vector3(player.transform.position.x + 7, player.transform.position.y + 3,
            player.transform.position.z);
         splineCamera.transform.eulerAngles = new Vector3(0,-90,0);
      }
   }
}
