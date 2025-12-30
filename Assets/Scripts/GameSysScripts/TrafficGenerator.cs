using UnityEngine;
using System.Collections.Generic;

public class TrafficGenerator : MonoBehaviour
{
    [Header("Spawn Prefab")]
    public List<GameObject> npcPrefabs;

    [Header("Spawn Setting")]
    public int spawnCount = 2;
    public List<Transform> spawnPoints;
    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned || !other.CompareTag("Player")) return;

        SpawnTraffic();
        hasSpawned = true;

        gameObject.SetActive(false);
    }

    void SpawnTraffic()
    {
        if (npcPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        List<Transform> remainPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < spawnCount; i++)
        {
            if(remainPoints.Count == 0) break;

            GameObject prefabToSpawn = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
            int pointIndex = Random.Range(0, remainPoints.Count);
            Transform selectedPoint = remainPoints[pointIndex];

            //rotation은 프리팹의 고유값을 이용할 수 있게.
            GameObject npc = Instantiate(prefabToSpawn, selectedPoint.position, selectedPoint.rotation * prefabToSpawn.transform.rotation, null);

            npc.transform.localScale = prefabToSpawn.transform.localScale;

            remainPoints.RemoveAt(pointIndex);
        }
    }
    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        if (hasSpawned || npcPrefabs.Count == 0) return;

        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag(spawnPointTag))
            {
                spawnPoints.Add(child);
            }
        }

        if (spawnPoints.Count == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            int prefabIndex = Random.Range(0, npcPrefabs.Count);
            GameObject prefabToSpawn = npcPrefabs[prefabIndex];

            int pointIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[pointIndex];

            //현재 이런방식으로 세개의 인자만 주면 청크가 생성되기도 전에 미리 생성되는 문제가 발생함.. -> 네번째 인자로 transform을 주면? 설명을 보면 4번째 인자는 부모 트랜스폼을 의미한다라고 나와있음
            //이런식으로 생성하면 이 npc 오브젝트들은 chunk의 자식클래스로 생성되어, chunk가 지워질때 같이 사라지는 문제가 발생한다..
            //부모에서 독립시키면 되지 않을까? tranform 하위 함수를 보면 SetParent라는게 존재한다. 이를 null값으로 바꾸면 될 것같다.
            //하지만 이렇게 하면 다시 청크가 생성전에 미리 나오는 문제 발생... 그냥 쓰지맙시다.
            //GameObject npcObj = 
            Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation, transform);

            //npcObj.transform.SetParent(null);

            spawnPoints.RemoveAt(pointIndex);
            
            if (spawnPoints.Count == 0) break;
        }

        hasSpawned = true;
    }
    */
}
