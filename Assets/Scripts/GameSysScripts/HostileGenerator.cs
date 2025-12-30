using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HostileGenerator : MonoBehaviour
{
    [Header("Player Setting")]
    public Transform playerTransform;
    public Rigidbody playerRb;

    [Header("Spawn Setting(Behind)")]
    [Tooltip("Behind Player")]
    public Transform behindHostile;
    public float behindSpawnDistance = 20f;
    public float minBehindSpawnInterval = 15f;
    public float maxBehindSpawnInterval = 25f;

    /*[Header("Spawn Setting(Front)")]
    [Tooltip("Front Player")]
    public Transform frontHostile;
    public float frontSpawnDistance = 200f;
    public float minFrontSpawnInterval = 15f;
    public float maxFrontSpawnInterval = 25f;*/

    [Header("Spawn Setting")]
    public float laneWidthRange = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if(playerObject != null)
            {
                playerTransform = playerObject.transform;
                playerRb = playerObject.GetComponent<Rigidbody>();
            }
            else
            {
                playerRb = playerTransform.GetComponent<Rigidbody>();
            }
        }

        StartCoroutine(SpawnBehindCoroutine());
        //StartCoroutine(SpawnFrontCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnBehindCoroutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBehindSpawnInterval, maxBehindSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (playerTransform == null || playerRb == null) continue;

            Vector3 spawnPos = playerTransform.position - (playerTransform.forward * behindSpawnDistance);

            float randomOffset = Random.Range(-laneWidthRange, laneWidthRange);
            spawnPos += Vector3.right * randomOffset;

            Transform npcTransform = Instantiate(behindHostile, spawnPos, behindHostile.transform.rotation);

            Rigidbody npcRb = npcTransform.GetComponent<Rigidbody>();

            if(npcRb != null)
            {
                npcRb.linearVelocity = playerRb.linearVelocity;
            }

            //Instantiate(behindHostile, spawnPos, behindHostile.transform.rotation);
        }
    }

    /*IEnumerator SpawnFrontCoroutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minFrontSpawnInterval, maxFrontSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (playerTransform == null) continue;

            Vector3 spawnPos = playerTransform.position + (playerTransform.forward * frontSpawnDistance);

            float randomOffset = Random.Range(-laneWidthRange, laneWidthRange);
            spawnPos += playerTransform.right * randomOffset;

            Instantiate(frontHostile, spawnPos, frontHostile.transform.rotation);
        }
    }*/
}
