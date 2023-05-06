using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
   [SerializeField] public GameObject spawnPosition;
   private GameObject enemy;
   private GameObject enemyCopy;
   private GameObject enemyCopyOne;
   private GameObject enemyCopyTwo;
   private GameObject enemyCopyThree;
   private GameObject enemyCopyFour;
   private GameObject enemyCopyFive;
   private GameObject enemyCopySix;
   private AI enemyScript;
   private float health;
   private float size;
   private Vector3 enemyCopyLocation;
   private Vector3 enemyCopyLocationTwo;
   private Vector3 enemyCopyLocationThree;
   private int firstSpawnCount;
   private int secondSpawnCount;
   private int thirdSpawnCount;

   //Make a list add to it the enemies do a for and constantly check if they are null

    private void Awake()
    {
        enemy = GameObject.Find("EnemyPrefabCopy");
        enemyCopy = Instantiate(enemy, spawnPosition.transform.position, transform.rotation).GameObject();
        enemyScript = enemyCopy.GetComponent<AI>();
        enemyScript.setEnemy(10,10,10,3,4);
        firstSpawnCount = 0;
        secondSpawnCount = 0;
    }

    private void Update()
    {
        SpawningMinions();
    }

    public void SpawningMinions()
    {
        if (enemyCopy == null && firstSpawnCount < 2)
        {
            if (firstSpawnCount == 1)
            {
                enemyCopyOne = Instantiate(enemy, enemyCopyLocation , transform.rotation).GameObject();
                enemyScript = enemyCopyOne.GetComponent<AI>();
                enemyScript.setEnemy(10,10,10,2,3);
            }
            else
            {
                enemyCopyTwo = Instantiate(enemy, enemyCopyLocation , transform.rotation).GameObject();
                enemyScript = enemyCopyTwo.GetComponent<AI>();
                enemyScript.setEnemy(10,10,10,2,3);
            }
            firstSpawnCount += 1;
        }
        else if (enemyCopy != null)
        {
            enemyCopyLocation = enemyCopy.transform.position;
        }

        if (enemyCopyOne == null && firstSpawnCount == 2 && secondSpawnCount < 2)
        {
            enemyCopyThree = Instantiate(enemy, enemyCopyLocationTwo , transform.rotation).GameObject();
            enemyScript = enemyCopyThree.GetComponent<AI>();
            enemyScript.setEnemy(10,10,10,1,2);
            secondSpawnCount += 1;
        }
        else if (enemyCopyOne != null)
        {
            enemyCopyLocationTwo = enemyCopyOne.transform.position;
        }
        if (enemyCopyTwo == null && firstSpawnCount == 2 && thirdSpawnCount < 2)
        {
            enemyCopyThree = Instantiate(enemy, enemyCopyLocationThree, transform.rotation).GameObject();
            enemyScript = enemyCopyThree.GetComponent<AI>();
            enemyScript.setEnemy(10,10,10,1,2);
            thirdSpawnCount += 1;
        }
        else if (enemyCopyTwo != null)
        {
            enemyCopyLocationThree = enemyCopyTwo.transform.position;
        }
    }
}
