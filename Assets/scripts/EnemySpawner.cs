using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemySpawnPositions;

    public GameObject enemyObject1;
    public GameObject enemyObject2;

    float randomValue;
    float extraValue;


    private void Start()
    {
        randomValue = Random.value;

        for (int i = 0; i < enemySpawnPositions.Length; i++)
        {
            if (randomValue < 0.5f || extraValue < 0.5f)
            {
                Instantiate(enemyObject1, enemySpawnPositions[i].transform.position, Quaternion.identity);
                extraValue = 0.75f;
            }

            else if (randomValue >= 0.5f || extraValue >= 0.5f)
            {
                Instantiate(enemyObject2, enemySpawnPositions[i].transform.position, Quaternion.identity);
                extraValue = 0.25f;
            }
        }
    }


}
