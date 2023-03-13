using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    private Camera cam;
    private Camera mainCam;
    [SerializeField] public GameObject door;
    [SerializeField] public GameObject target;
    [SerializeField] public float endTime;
    private float time;
    private Door doorScript;
    private Vector3 endPosition;
    private float offset;
    private float cameraShakeTime;
    
    private void Awake()
    {
        offset = 3;
        mainCam = Camera.main;
        cam = GetComponent<Camera>();
        cam.enabled = false;
        cam.GetComponent<AudioListener>().enabled = false;
        doorScript = door.GetComponent<Door>();
        endPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + offset);
    }

    private void FixedUpdate()
    {
        if (doorScript.open == true)
        {
            cam.enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
            mainCam.enabled = false;
            mainCam.GetComponent<AudioListener>().enabled = false;

           // CameraShake();

            transform.position = Vector3.Lerp(transform.position, endPosition, 0.25f * Time.deltaTime);
            time += Time.deltaTime;
            if (time > endTime)
            {
                doorScript.open = false;
                cam.enabled = false;
                cam.GetComponent<AudioListener>().enabled = false;
                mainCam.enabled = true;
                mainCam.GetComponent<AudioListener>().enabled = true;
            }
        }
    }

    private void CameraShake()
    {
        cameraShakeTime += Time.deltaTime;

        if (cameraShakeTime > 0 && cameraShakeTime < 0.2)
        {
            transform.position = new Vector3(transform.position.x + 0.03f, transform.position.y, transform.position.z);
        }
        else if (cameraShakeTime > 0.2 && cameraShakeTime < 0.4)
        {
            transform.position = new Vector3(transform.position.x - 0.06f, transform.position.y, transform.position.z);
        }
        else if (cameraShakeTime > 0.4 && cameraShakeTime < 0.6)
        {
            transform.position = new Vector3(transform.position.x + 0.03f, transform.position.y, transform.position.z);
        }

        if (cameraShakeTime > 0.8)
        {
            Debug.Log("ssfasf");
            cameraShakeTime = 0;
        }
    }
}
