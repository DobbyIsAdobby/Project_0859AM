using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashGenerator : MonoBehaviour
{
    [Header("Target")]
    public Transform playerTransform;

    [Header("Projectile Spawner")]
    public GameObject trashPrefab_Low;
    public GameObject trashPrefab_Medium;
    public GameObject trashPrefab_High;
    public GameObject keyPrefab;

    [Header("Spawn Setting")]
    [Tooltip("The distance from the player where the projectile will spawn(meter)")]
    public float spawnDistanceAhead = 50f;
    [Tooltip("Random left and right range where projectiles will spawn(meter)")]
    public float spawnRandomRangeX = 7f;

    [Header("Spawn Time Setting")]
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 7f;

    [Header("Projectiles Amount Setting")]
    public int projectilesQuantity = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(ProjectileSpawner());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ProjectileSpawner()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (playerTransform == null) continue;

            for(int i = 0; i < projectilesQuantity; i++)
            {
                float randomX = Random.Range(-spawnRandomRangeX, spawnRandomRangeX);
                Vector3 spawnPosition = new Vector3(
                    playerTransform.position.x + randomX,
                    playerTransform.position.y,
                    playerTransform.position.z + spawnDistanceAhead
            );

            GameObject prefabToSpawn = GetRandomProjectile();

            if (prefabToSpawn == null) continue;

            Instantiate(prefabToSpawn, spawnPosition, prefabToSpawn.transform.rotation);   
            }
        }
    }
    
    GameObject GetRandomProjectile()
    {
        float chance = Random.Range(0f, 10f);

        if (chance <= 0.1f && keyPrefab != null)
        {
            return keyPrefab;
        }

        float trashChance = Random.Range(0f, 10f);

        if (trashChance <= 6f)
        {
            return trashPrefab_Low;
        }
        else if (trashChance <= 9f)
        {
            return trashPrefab_Medium;
        }
        else
        {
            return trashPrefab_High;
        }
    }
}
