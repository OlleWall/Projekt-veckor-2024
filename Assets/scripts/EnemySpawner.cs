using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemySpawnPositions;

    public bool upperClass;
    public bool middleClass;
    public bool lowerClass;

    public GameObject enemyObject1;
    public GameObject enemyObject2;
    public GameObject enemyObject3;
    public GameObject enemyObject4;
    public GameObject enemyObject5;


    bool spawnOtherEnemy = false;
    float randomValue;
    float extraValue;


    private void Start()
    {
        randomValue = Random.value;

        for (int i = 0; i < enemySpawnPositions.Length; i++)
        {
            if (upperClass)
            {
                if (!spawnOtherEnemy)
                {
                    Instantiate(enemyObject1, enemySpawnPositions[i].transform.position, Quaternion.identity);
                    spawnOtherEnemy = true;
                }

                else if (spawnOtherEnemy)
                {
                    Instantiate(enemyObject2, enemySpawnPositions[i].transform.position, Quaternion.identity);
                    spawnOtherEnemy = false;
                }
            }

            if (middleClass)
            {
                if (!spawnOtherEnemy)
                {
                    Instantiate(enemyObject3, enemySpawnPositions[i].transform.position, Quaternion.identity);
                    spawnOtherEnemy = true;
                }

                else if (spawnOtherEnemy)
                {
                    Instantiate(enemyObject4, enemySpawnPositions[i].transform.position, Quaternion.identity);
                    spawnOtherEnemy = false;
                }
            }

            if (lowerClass)
            {
                if (randomValue < 0.5f || extraValue < 0.5f)
                {
                    Instantiate(enemyObject5, enemySpawnPositions[i].transform.position, Quaternion.identity);
                    extraValue = 0.75f;
                }

                else if (randomValue >= 0.5f || extraValue >= 0.5f)
                {
                    Instantiate(enemyObject5, enemySpawnPositions[i].transform.position, Quaternion.identity);
                    extraValue = 0.25f;
                }
            }
        }
    }


}
